using Repositories;
using Repositories.Models;
using Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAll();
        Task<Category> GetById(short id);

    }

    public class CategoryService: ICategoryService
    {
        private readonly CategoryRepository _categoryRepository;

        public CategoryService()
        {
            _categoryRepository = new CategoryRepository();
        }

        public async Task<List<Category>> GetAll()
        {
            return await _categoryRepository.GetAll();
        }

        public async Task<Category> GetById(short id)
        {
            var article = await _categoryRepository.GetByIdAsync(id);
            if (article == null)
                throw new InvalidOperationException($"No Category found with ID {id}");
            return article;
        }
    }
}
