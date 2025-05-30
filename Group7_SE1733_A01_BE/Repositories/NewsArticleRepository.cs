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

        public async Task<int> AddTagAsync(Tag tag)
        {
            _context.Tags.Add(tag);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateNewsArticleAsync(NewsArticle entity)
        {
            _context.NewsArticles.Update(entity);
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
            return  _context.NewsArticles
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
                .Where(i => (i.NewsTitle.ToString().Contains(NewsTitle) || string.IsNullOrEmpty(NewsTitle))
                && (i.Headline.ToString().Contains(Headline) || string.IsNullOrEmpty(Headline))
                && (i.NewsSource.ToString().Contains(NewsSource) || string.IsNullOrEmpty(NewsSource)))
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
            // Nếu bảng Tag chưa có bản ghi nào thì trả về 0
            return await _context.Tags.AnyAsync()
                ? await _context.Tags.MaxAsync(t => t.TagId)
                : 0;
        }
    }
}