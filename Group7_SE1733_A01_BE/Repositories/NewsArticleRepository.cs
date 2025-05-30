using Microsoft.EntityFrameworkCore;
using Repositories.Base;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class NewsArticleRepository : GenericRepository<NewsArticle>
    {
        public NewsArticleRepository() { }

        public async Task<NewsArticle> GetByIdWithTrackingAsync(string id)
        {
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
            return await _context.NewsArticles
                .Include(x => x.Tags)
                .Include(x => x.Category)
                .Include(x => x.CreatedBy)
                .FirstOrDefaultAsync(x => x.NewsArticleId == id);
        }

        public async Task<Tag> GetTagByIdAsync(int tagId)
        {
            return await _context.Tags.FindAsync(tagId);
        }

        public async Task<int> UpdateAsync(NewsArticle article, List<int> tagIds)
        {
            // Xóa tag cũ
            article.Tags.Clear();

            // Thêm tag mới theo tagIds
            if (tagIds.Any())
            {
                var tags = await _context.Tags.Where(t => tagIds.Contains(t.TagId)).ToListAsync();
                foreach (var tag in tags)
                {
                    article.Tags.Add(tag);
                }
            }

            _context.NewsArticles.Update(article);
            return await _context.SaveChangesAsync();
        }


        public async Task<List<NewsArticle>> GetAll()
        {
            return await _context.NewsArticles
                .Include(x => x.Category)
                .Include(x => x.Tags)
                .Include(x => x.CreatedBy)
                .OrderBy(x => x.CreatedDate)
                .ToListAsync();
        }

        public async Task<IQueryable<NewsArticle>> GetAllAsQueryable()
        {
            return _context.NewsArticles
                .Include(x => x.Category)
                .Include(x => x.Tags)
                .Include(x => x.CreatedBy)
                .AsQueryable();
        }

        public async Task<NewsArticle> GetByIdAsync(string id)
        {
            return await _context.NewsArticles
                .Include(x => x.Tags)
                .Include(x => x.Category)
                .Include(x => x.CreatedBy)
                .OrderBy(x => x.CreatedDate)
                .FirstOrDefaultAsync(x => x.NewsArticleId == id);
        }

        public async Task<List<NewsArticle>> Search(string NewsTitle, string Headline, string NewsSource)
        {
            var items = await _context.NewsArticles
                .Include(t => t.Tags)
                .Include(x => x.Category)
                .Where(i => (i.NewsTitle.Contains(NewsTitle) || string.IsNullOrEmpty(NewsTitle))
                    && (i.Headline.Contains(Headline) || string.IsNullOrEmpty(Headline))
                    && (i.NewsSource.Contains(NewsSource) || string.IsNullOrEmpty(NewsSource)))
                .ToListAsync();

            return items;
        }

        public async Task<List<NewsArticle>> ViewHistory(
            short createdById,
            string NewsTitle,
            string Headline,
            string NewsSource)
        {
            var items = await _context.NewsArticles
                .Include(t => t.Tags)
                .Where(i =>
                    i.CreatedById == createdById &&
                    (i.NewsTitle.Contains(NewsTitle) || string.IsNullOrEmpty(NewsTitle)) &&
                    (i.Headline.Contains(Headline) || string.IsNullOrEmpty(Headline)) &&
                    (i.NewsSource.Contains(NewsSource) || string.IsNullOrEmpty(NewsSource))
                )
                .OrderByDescending(i => i.CreatedDate)
                .ToListAsync();

            return items;
        }

        public async Task<int> GetMaxTagIdAsync()
        {
            return await _context.Tags.AnyAsync()
                ? await _context.Tags.MaxAsync(t => t.TagId)
                : 0;
        }

        public async Task<int> CreateAsync(NewsArticle article, List<int> tagIds)
        {
            if (tagIds != null && tagIds.Any())
            {
                article.Tags = new List<Tag>();

                foreach (var tagId in tagIds.Distinct())
                {
                    var tag = new Tag { TagId = tagId };
                    _context.Attach(tag);
                    article.Tags.Add(tag);
                }
            }

            _context.NewsArticles.Add(article);
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteWithRelationsAsync(string id)
        {
            // Xóa bản ghi trong bảng trung gian trước
            var existingRelations = await _context.NewsTag
                .Where(nt => nt.NewsArticleId == id)
                .ToListAsync();

            if (existingRelations.Any())
            {
                _context.NewsTag.RemoveRange(existingRelations);
                await _context.SaveChangesAsync();
            }

            // Sau đó mới xóa bài viết
            var article = await _context.NewsArticles
                .FirstOrDefaultAsync(a => a.NewsArticleId == id);

            if (article == null) return false;

            _context.NewsArticles.Remove(article);
            await _context.SaveChangesAsync();

            return true;
        }


    }
}
