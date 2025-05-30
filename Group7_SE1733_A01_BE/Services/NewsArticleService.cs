using Group7_SE1733_A01_BE.Service.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Repositories;
using Repositories.Models;
using Services.DTOs;

namespace Services
{
    public interface INewsArticleService
    {
        Task<List<NewsArticle>> GetAll();
        Task<NewsArticle> GetById(string id);
        Task<int> Create(NewsArticleCreateDTO dto);
        Task<int> Update(string id, NewsArticleUpdateDTO dto);
        Task<bool> Delete(string id);
        Task<List<NewsArticle>> GetNewsHistoryByUser(short createdById, string NewsTitle, string Headline, string NewsSource);
        Task<List<NewsArticle>> Search(string NewsTitle, string Headline, string NewsSource);
        Task<List<NewsArticle>> GetReportAsync(DateTime? startDate, DateTime? endDate);

    }

    public class NewsArticleService : INewsArticleService
    {
        private readonly NewsArticleRepository _newsArticleRepository;
        private readonly TagRepository _tagRepository;
        private readonly CategoryRepository _categoryRepository;
        private readonly SystemAccountRepository _systemAccountRepository;

        public NewsArticleService()
        {
            _newsArticleRepository = new NewsArticleRepository();
            _tagRepository = new TagRepository();
            _categoryRepository = new CategoryRepository();
            _systemAccountRepository = new SystemAccountRepository();
        }

        public async Task<List<NewsArticle>> GetAll()
        {
            return await _newsArticleRepository.GetAll();
        }

        public async Task<NewsArticle> GetById(string id)
        {
            var article = await _newsArticleRepository.GetByIdAsync(id);
            if (article == null)
                throw new InvalidOperationException($"No NewsArticle found with ID {id}");
            return article;
        }

        public async Task<int> Create(NewsArticleCreateDTO dto)
        {
            if (dto.NewsArticleId <= 0)
                throw new ArgumentException("NewsArticleId must be a positive integer");

            if (string.IsNullOrWhiteSpace(dto.NewsTitle))
                throw new ArgumentException("NewsTitle is required");

            if (string.IsNullOrWhiteSpace(dto.Headline))
                throw new ArgumentException("Headline is required");

            if (string.IsNullOrWhiteSpace(dto.NewsContent))
                throw new ArgumentException("NewsContent is required");

            if (string.IsNullOrWhiteSpace(dto.NewsSource))
                throw new ArgumentException("NewsSource is required");

            // Check duplicate ID
            var existingArticle = await _newsArticleRepository.GetByIdAsync(dto.NewsArticleId.ToString());
            if (existingArticle != null)
                throw new InvalidOperationException($"A NewsArticle with ID {dto.NewsArticleId} already exists.");

            // Validate Category
            var category = await _categoryRepository.GetByIdAsync((short)dto.CategoryId);
            if (category == null)
                throw new InvalidOperationException($"Category with ID {dto.CategoryId} does not exist.");
                
            if (dto.NewsStatus is not true and not false)
                throw new ArgumentException("NewsStatus must be either true (1) or false (0)");

            // Validate CreatedById (phải tồn tại trong bảng SystemAccount)
            var account = await _systemAccountRepository.GetByIdAsync((int)dto.CreatedById);
            if (account == null)
                throw new InvalidOperationException($"SystemAccount with ID {dto.CreatedById} does not exist.");

            // Xử lý tags
            var tagsToAdd = new List<Tag>();

            if (dto.Tags != null)
            {
                // Lấy tất cả các TagId hiện có
                var existingTagsAll = await _tagRepository.GetAllAsync();
                var existingIds = existingTagsAll.Select(t => t.TagId).ToHashSet();
                int nextAvailableId = existingIds.Count > 0 ? existingIds.Max() + 1 : 1;

                foreach (var tagDto in dto.Tags)
                {

                    // Gán ID tự tăng
                    while (existingIds.Contains(nextAvailableId))
                    {
                        nextAvailableId++;
                    }

                    var newTag = new Tag
                    {
                        TagId = nextAvailableId,
                        TagName = tagDto.TagName,
                        Note = tagDto.Note
                    };

                    existingIds.Add(nextAvailableId);
                    nextAvailableId++;

                    tagsToAdd.Add(newTag);
                }
            }

            var article = new NewsArticle
            {
                NewsArticleId = dto.NewsArticleId.ToString(), 
                NewsTitle = dto.NewsTitle,
                Headline = dto.Headline,
                CreatedDate = dto.CreatedDate ?? DateTime.Now,
                NewsContent = dto.NewsContent,
                NewsSource = dto.NewsSource,
                CategoryId = dto.CategoryId,
                NewsStatus = dto.NewsStatus,
                CreatedById = dto.CreatedById,
                Tags = tagsToAdd
            };

            return await _newsArticleRepository.CreateAsync(article);
        }



        public async Task<int> Update(string id, NewsArticleUpdateDTO dto)
        {
            // Validate input (business rule)
            if (string.IsNullOrWhiteSpace(dto.NewsTitle))
                throw new ArgumentException("NewsTitle is required");

            if (string.IsNullOrWhiteSpace(dto.Headline))
                throw new ArgumentException("Headline is required");

            if (string.IsNullOrWhiteSpace(dto.NewsContent))
                throw new ArgumentException("NewsContent is required");

            if (string.IsNullOrWhiteSpace(dto.NewsSource))
                throw new ArgumentException("NewsSource is required");

            // Lấy bài viết có tracking từ repo
            var existing = await _newsArticleRepository.GetByIdWithTrackingAsync(id);
            if (existing == null)
                throw new InvalidOperationException("Article not found");

            // Lấy Category từ repo
            var category = await _categoryRepository.GetByIdAsync((short)dto.CategoryId);
            if (category == null)
                throw new InvalidOperationException($"Category with ID {dto.CategoryId} does not exist.");

            // Lấy SystemAccount từ repo
            var account = await _systemAccountRepository.GetByIdAsync((int)dto.UpdatedById);
            if (account == null)
                throw new InvalidOperationException($"SystemAccount with ID {dto.UpdatedById} does not exist.");

            // Update các thuộc tính
            existing.NewsTitle = dto.NewsTitle;
            existing.Headline = dto.Headline;
            existing.NewsContent = dto.NewsContent;
            existing.NewsSource = dto.NewsSource;
            existing.CategoryId = dto.CategoryId;
            existing.NewsStatus = dto.NewsStatus;
            existing.UpdatedById = dto.UpdatedById;
            existing.ModifiedDate = dto.ModifiedDate ?? DateTime.UtcNow;

            // Xử lý Tags
            existing.Tags.Clear();

            if (dto.Tags != null && dto.Tags.Any())
            {
                int maxTagId = await _tagRepository.GetMaxTagIdAsync();

                foreach (var tagDto in dto.Tags)
                {
                    if (tagDto.TagId < 0)
                        throw new ArgumentException("TagId must be zero or a positive integer.");

                    if (tagDto.TagId > 0)
                    {
                        var existingTag = await _newsArticleRepository.GetTagByIdAsync(tagDto.TagId);
                        if (existingTag != null)
                        {
                            existingTag.TagName = tagDto.TagName;
                            existingTag.Note = tagDto.Note;

                            existing.Tags.Add(existingTag);
                        }
                        else
                        {
                            throw new ArgumentException($"Tag with ID {tagDto.TagId} does not exist.");
                        }
                    }
                    else
                    {
                        maxTagId++;
                        var newTag = new Tag
                        {
                            TagId = maxTagId,
                            TagName = tagDto.TagName,
                            Note = tagDto.Note
                        };
                        await _newsArticleRepository.AddTagAsync(newTag);
                        existing.Tags.Add(newTag);
                    }
                }
            }

            // Update toàn bộ NewsArticle
            return await _newsArticleRepository.UpdateNewsArticleAsync(existing);
        }


        public async Task<bool> Delete(string id)
        {
            var article = await _newsArticleRepository
                .GetQueryable()
                .Include(a => a.Tags)
                .FirstOrDefaultAsync(a => a.NewsArticleId == id);

            if (article == null) return false;

            var relatedTags = article.Tags.ToList();

            // Xóa bài viết
            var deleted = await _newsArticleRepository.RemoveAsync(article);
            if (!deleted) return false;

            foreach (var tag in relatedTags)
            {
                var stillUsed = await _newsArticleRepository
                    .GetQueryable()
                    .AnyAsync(a => a.Tags.Any(t => t.TagId == tag.TagId));

                if (!stillUsed)
                {
                    await _tagRepository.RemoveAsync(tag);
                }
            }

            return true;
        }

        public async Task<List<NewsArticle>> GetNewsHistoryByUser(
            short createdById,
            string NewsTitle,
            string Headline,
            string NewsSource)
        {
            return await _newsArticleRepository.ViewHistory(createdById, NewsTitle, Headline, NewsSource);
        }




        public async Task<List<NewsArticle>> Search(string NewsTitle, string Headline, string NewsSource)
        {
            return await _newsArticleRepository.Search(NewsTitle,Headline,NewsSource);
        }


        public async Task<List<NewsArticle>> GetReportAsync(DateTime? startDate, DateTime? endDate)
        {
            var query = await _newsArticleRepository.GetAllAsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(n => n.CreatedDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(n => n.CreatedDate <= endDate.Value);
            }

            return await query
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }
    }
}