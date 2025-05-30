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

        Task<int> Create(CategoryCreateDTO categoryDto);

        Task<int> Update(short categoryId, CategoryUpdateDTO categoryDto);
        Task<bool> Delete(short categoryId);

        Task<List<Category>> Search(string? CategoryName, string? CategoryDesciption);

    }

    public class CategoryService: ICategoryService
    {
        private readonly CategoryRepository _categoryRepository;
        private readonly NewsArticleRepository _newsArticleRepository;

        public CategoryService()
        {
            _categoryRepository = new CategoryRepository();
            _newsArticleRepository = new NewsArticleRepository();
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

        public async Task<int> Create(CategoryCreateDTO categoryDto)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(categoryDto.CategoryName))
                throw new ArgumentException("CategoryName cannot be null or empty.");

            if (categoryDto.CategoryName.Length > 100)
                throw new ArgumentException("CategoryName cannot exceed 100 characters.");

            if (!string.IsNullOrWhiteSpace(categoryDto.CategoryDesciption) && categoryDto.CategoryDesciption.Length > 255)
                throw new ArgumentException("CategoryDesciption cannot exceed 255 characters.");


            // Validate ParentCategoryId (optional: only if you want to check if it exists)
            if (categoryDto.ParentCategoryId.HasValue)
            {
                var parentCategory = await _categoryRepository.GetByIdAsync(categoryDto.ParentCategoryId.Value);
                if (parentCategory == null)
                    throw new ArgumentException("ParentCategoryId does not exist.");
            }

            // Create entity
            var categoryEntity = new Category
            {
                CategoryName = categoryDto.CategoryName,
                CategoryDesciption = categoryDto.CategoryDesciption,
                ParentCategoryId = categoryDto.ParentCategoryId,
                IsActive = categoryDto.IsActive ?? true
            };

            try
            {
                return await _categoryRepository.CreateAsync(categoryEntity);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner Exception: " + ex.InnerException.Message);
                }

                throw; // Rethrow để không nuốt lỗi
            }

        }

        public async Task<int> Update(short categoryId, CategoryUpdateDTO categoryDto)
        {
            var existingCategory = await _categoryRepository.GetByIdAsync(categoryId);
            if (existingCategory == null)
            {
                throw new InvalidOperationException("Category does not exist.");
            }

            if (string.IsNullOrWhiteSpace(categoryDto.CategoryName))
                throw new ArgumentException("CategoryName cannot be null or empty.");

            if (categoryDto.CategoryName.Length > 100)
                throw new ArgumentException("CategoryName cannot exceed 100 characters.");

            if (!string.IsNullOrWhiteSpace(categoryDto.CategoryDesciption) && categoryDto.CategoryDesciption.Length > 255)
                throw new ArgumentException("CategoryDesciption cannot exceed 255 characters.");

            if (categoryDto.ParentCategoryId.HasValue)
            {
                if (categoryDto.ParentCategoryId == categoryId)
                    throw new ArgumentException("ParentCategoryId cannot be the same as CategoryId.");

                var parentCategory = await _categoryRepository.GetByIdAsync(categoryDto.ParentCategoryId.Value);
                if (parentCategory == null)
                    throw new ArgumentException("ParentCategoryId does not exist.");
            }

            // Cập nhật các trường
            existingCategory.CategoryName = categoryDto.CategoryName;
            existingCategory.CategoryDesciption = categoryDto.CategoryDesciption;
            existingCategory.ParentCategoryId = categoryDto.ParentCategoryId;
            existingCategory.IsActive = categoryDto.IsActive ?? existingCategory.IsActive;

            return await _categoryRepository.UpdateAsync(existingCategory);
        }

        public async Task<bool> Delete(short categoryId)
        {
            // Kiểm tra category có bài viết liên quan không
            var articles = await _newsArticleRepository.GetAll();
            if (articles.Any(a => a.CategoryId == categoryId))
            {
                throw new InvalidOperationException("Cannot delete category because it is associated with existing articles.");
            }

            // Kiểm tra category có category con không
            var allCategories = await _categoryRepository.GetAll();
            if (allCategories.Any(c => c.ParentCategoryId == categoryId))
            {
                throw new InvalidOperationException("Cannot delete category because it has child categories.");
            }

            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
            {
                return false;
            }

            return await _categoryRepository.RemoveAsync(category);
        }

        public async Task<List<Category>> Search(string? CategoryName, string? CategoryDesciption)
        {
            return await _categoryRepository.Search(CategoryName, CategoryDesciption);
        }


    }
}
