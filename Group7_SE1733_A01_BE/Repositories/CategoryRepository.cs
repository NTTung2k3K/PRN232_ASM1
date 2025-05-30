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
    public class CategoryRepository: GenericRepository<Category>
    {
        public CategoryRepository() { }
        public async Task<List<Category>> GetAll()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category> GetByIdAsync(short id)
        {
            return await _context.Categories.FirstOrDefaultAsync(x => x.CategoryId == id);
        }

        public async Task<List<Category>> Search(string CategoryName, string CategoryDesciption)
        {
            var items = await _context.Categories
                .Where(i => (i.CategoryName.ToString().Contains(CategoryName) || string.IsNullOrEmpty(CategoryName))
                && (i.CategoryDesciption.ToString().Contains(CategoryDesciption) || string.IsNullOrEmpty(CategoryDesciption)))
                .ToListAsync();

            return items;
        }

       

    }


}
