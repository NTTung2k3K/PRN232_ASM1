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

        public async Task<List<NewsArticle>> GetAll()
        {
            return await _context.NewsArticles
                .Include(x => x.Tags)
                .OrderBy(x => x.CreatedDate)
                .ToListAsync();
        }

        public async Task<NewsArticle> GetByIdAsync(string id)
        {
            return await _context.NewsArticles
                .Include(x => x.Tags)
                .FirstOrDefaultAsync(x => x.NewsArticleId == id);
        }

        public async Task<List<NewsArticle>> SearchByStatus(bool? status)
        {
            return await _context.NewsArticles
                .Include(x => x.Tags)
                .Where(x => x.NewsStatus == status)
                .OrderBy(x => x.CreatedDate)
                .ToListAsync();
        }
    }
}