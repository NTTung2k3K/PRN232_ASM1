using Group7_SE1733_A01_BE.Service.DTOs;
using Group7_SE1733_A01_FE.DTOs;
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
        Task<List<NewsArticle>> SearchActiveStatus(string NewsTitle, string Headline, string NewsSource);
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

            var existingArticle = await _newsArticleRepository.GetByIdAsync(dto.NewsArticleId.ToString());
            if (existingArticle != null)
                throw new InvalidOperationException($"A NewsArticle with ID {dto.NewsArticleId} already exists.");

            var category = await _categoryRepository.GetByIdAsync((short)dto.CategoryId);
            if (category == null)
                throw new InvalidOperationException($"Category with ID {dto.CategoryId} does not exist.");

            var account = await _systemAccountRepository.GetByIdAsync((int)dto.CreatedById);
            if (account == null)
                throw new InvalidOperationException($"SystemAccount with ID {dto.CreatedById} does not exist.");

            // Kiểm tra tồn tại của từng TagId, nhưng không tạo đối tượng Tag
            if (dto.TagIds != null && dto.TagIds.Any())
            {
                foreach (var tagId in dto.TagIds.Distinct())
                {
                    var tag = await _newsArticleRepository.GetTagByIdAsync(tagId);
                    if (tag == null)
                        throw new InvalidOperationException($"Tag with ID {tagId} does not exist.");
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
                CreatedById = dto.CreatedById
                // Không gán Tags ở đây!
            };

            // Gọi hàm mới trong Repository để xử lý TagIds
            return await _newsArticleRepository.CreateAsync(article, dto.TagIds ?? new List<int>());
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

            var existing = await _newsArticleRepository.GetByIdWithTrackingAsync(id);
            if (existing == null)
                throw new InvalidOperationException("Article not found");

            // Kiểm tra Category
            if (dto.CategoryId.HasValue)
            {
                var category = await _categoryRepository.GetByIdAsync(dto.CategoryId.Value);
                if (category == null)
                    throw new InvalidOperationException($"Category with ID {dto.CategoryId} does not exist.");
            }

            // Kiểm tra SystemAccount
            if (dto.UpdatedById.HasValue)
            {
                var account = await _systemAccountRepository.GetByIdAsync(dto.UpdatedById.Value);
                if (account == null)
                    throw new InvalidOperationException($"SystemAccount with ID {dto.UpdatedById} does not exist.");
            }

            // Cập nhật các thuộc tính chính
            existing.NewsTitle = dto.NewsTitle;
            existing.Headline = dto.Headline;
            existing.NewsContent = dto.NewsContent;
            existing.NewsSource = dto.NewsSource;
            existing.CategoryId = dto.CategoryId;
            existing.NewsStatus = dto.NewsStatus;
            existing.UpdatedById = dto.UpdatedById;
            existing.ModifiedDate = dto.ModifiedDate ?? DateTime.UtcNow;

            // Validate TagIds tồn tại trước khi gọi repository xử lý
            if (dto.TagIds != null && dto.TagIds.Any())
            {
                foreach (var tagId in dto.TagIds.Distinct())
                {
                    var tag = await _tagRepository.GetByIdAsync(tagId);
                    if (tag == null)
                        throw new InvalidOperationException($"Tag with ID {tagId} does not exist.");
                }
            }

            // Gọi repository method mới để update NewsArticle và xử lý TagIds
            return await _newsArticleRepository.UpdateAsync(existing, dto.TagIds ?? new List<int>());
        }


        public async Task<bool> Delete(string id)
        {
            return await _newsArticleRepository.DeleteWithRelationsAsync(id);
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

        public async Task<List<NewsArticle>> SearchActiveStatus(string NewsTitle, string Headline, string NewsSource)
        {
            return await _newsArticleRepository.SearchActiveStatus(NewsTitle, Headline, NewsSource);
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