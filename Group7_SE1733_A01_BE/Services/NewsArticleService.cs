using Group7_SE1733_A01_BE.Service.DTOs;
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
        Task<int> Update(string id, NewsArticleDTO dto);
        Task<bool> Delete(string id);
        Task<List<NewsArticle>> SearchByStatus(int status);
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
            var category = await _categoryRepository.GetByIdAsync((int)dto.CategoryId);
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

                    // Cập nhật lại danh sách ID đã dùng để tránh trùng
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



        public async Task<int> Update(string id, NewsArticleDTO dto)
        {
            var existing = await _newsArticleRepository.GetByIdAsync(id);
            if (existing == null)
                throw new InvalidOperationException("Article not found");

            if (string.IsNullOrWhiteSpace(dto.NewsTitle))
                throw new ArgumentException("NewsTitle is required");

            if (string.IsNullOrWhiteSpace(dto.Headline))
                throw new ArgumentException("Headline is required");

            if (string.IsNullOrWhiteSpace(dto.NewsContent))
                throw new ArgumentException("NewsContent is required");

            if (string.IsNullOrWhiteSpace(dto.NewsSource))
                throw new ArgumentException("NewsSource is required");

            // Validate Category
            var category = await _categoryRepository.GetByIdAsync((int)dto.CategoryId);
            if (category == null)
                throw new InvalidOperationException($"Category with ID {dto.CategoryId} does not exist.");

            if (dto.NewsStatus is not true and not false)
                throw new ArgumentException("NewsStatus must be either true (1) or false (0)");

            // Validate CreatedById (phải tồn tại trong bảng SystemAccount)
            var account = await _systemAccountRepository.GetByIdAsync((int)dto.UpdatedById);
            if (account == null)
                throw new InvalidOperationException($"SystemAccount with ID {dto.UpdatedById} does not exist.");

            existing.NewsTitle = dto.NewsTitle;
            existing.Headline = dto.Headline;
            existing.NewsContent = dto.NewsContent;
            existing.NewsSource = dto.NewsSource;
            existing.CategoryId = dto.CategoryId;
            existing.NewsStatus = dto.NewsStatus;
            existing.UpdatedById = dto.UpdatedById;
            existing.ModifiedDate = dto.ModifiedDate ?? DateTime.Now;

            // Xử lý Tags
            var newTags = new List<Tag>();
            foreach (var tagDto in dto.Tags)
            {
                var existingTag = await _tagRepository.FindByConditionAsync(t => t.TagName == tagDto.TagName);
                var tag = existingTag.FirstOrDefault();

                if (tag == null)
                {
                    tag = new Tag
                    {
                        TagName = tagDto.TagName,
                        Note = tagDto.Note
                    };
                    await _tagRepository.CreateAsync(tag);
                }

                newTags.Add(tag);
            }

            existing.Tags = newTags;

            return await _newsArticleRepository.UpdateAsync(existing);
        }


        public async Task<bool> Delete(string id)
        {
            var item = await _newsArticleRepository.GetByIdAsync(id);
            if (item == null) return false;

            return await _newsArticleRepository.RemoveAsync(item);
        }

        public async Task<List<NewsArticle>> SearchByStatus(int status)
        {
            if (status != 0 && status != 1) throw new ArgumentException("Status must be 0 or 1");
            return await _newsArticleRepository.SearchByStatus(status == 1);
        }
    }
}