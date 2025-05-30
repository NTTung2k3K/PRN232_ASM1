using Repositories;
using Repositories.Models;
using Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public interface ITagService
    {
        Task<List<Tag>> GetAll();
        Task<Tag> GetById(int id);

        Task<int> Create(TagCreateDTO tagDto);

        Task<int> Update(int tagId, TagUpdateDTO tagDto);
        Task<bool> Delete(int tagId);

        Task<List<Tag>> Search(string? tagName, string? note);
    }

    public class TagService : ITagService
    {
        private readonly TagRepository _tagRepository;
        private readonly NewsArticleRepository _newsArticleRepository;

        public TagService()
        {
            _tagRepository = new TagRepository();
            _newsArticleRepository = new NewsArticleRepository();
        }

        public async Task<List<Tag>> GetAll()
        {
            return await _tagRepository.GetAll();
        }

        public async Task<Tag> GetById(int id)
        {
            var tag = await _tagRepository.GetByIdAsync(id);
            if (tag == null)
                throw new InvalidOperationException($"No Tag found with ID {id}");
            return tag;
        }

        public async Task<int> Create(TagCreateDTO tagDto)
        {
            // Validate TagId - phải do người dùng gán, không auto tăng
            if (tagDto.TagId <= 0)
                throw new ArgumentException("TagId must be a positive integer.");

            // Kiểm tra tồn tại TagId
            var existingTag = await _tagRepository.GetByIdAsync(tagDto.TagId);
            if (existingTag != null)
                throw new ArgumentException("TagId already exists.");

            // Validate TagName
            if (string.IsNullOrWhiteSpace(tagDto.TagName))
                throw new ArgumentException("TagName cannot be null or empty.");

            if (tagDto.TagName.Length > 50)
                throw new ArgumentException("TagName cannot exceed 50 characters.");

            if (!string.IsNullOrWhiteSpace(tagDto.Note) && tagDto.Note.Length > 400)
                throw new ArgumentException("Note cannot exceed 400 characters.");

            var tagEntity = new Tag
            {
                TagId = tagDto.TagId,
                TagName = tagDto.TagName,
                Note = tagDto.Note
            };

            try
            {
                return await _tagRepository.CreateAsync(tagEntity);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (ex.InnerException != null)
                    Console.WriteLine("Inner Exception: " + ex.InnerException.Message);
                throw;
            }
        }

        public async Task<int> Update(int tagId, TagUpdateDTO tagDto)
        {
            var existingTag = await _tagRepository.GetByIdAsync(tagId);
            if (existingTag == null)
                throw new InvalidOperationException("Tag does not exist.");

            if (string.IsNullOrWhiteSpace(tagDto.TagName))
                throw new ArgumentException("TagName cannot be null or empty.");

            if (tagDto.TagName.Length > 50)
                throw new ArgumentException("TagName cannot exceed 50 characters.");

            if (!string.IsNullOrWhiteSpace(tagDto.Note) && tagDto.Note.Length > 400)
                throw new ArgumentException("Note cannot exceed 400 characters.");

            // Cập nhật các trường
            existingTag.TagName = tagDto.TagName;
            existingTag.Note = tagDto.Note;

            return await _tagRepository.UpdateAsync(existingTag);
        }

        public async Task<bool> Delete(int tagId)
        {
            // Kiểm tra Tag có liên kết bài viết không
            var articles = await _newsArticleRepository.GetAll();
            if (articles.Any(a => a.Tags.Any(t => t.TagId == tagId)))
            {
                throw new InvalidOperationException("Cannot delete tag because it is associated with existing articles.");
            }

            var tag = await _tagRepository.GetByIdAsync(tagId);
            if (tag == null)
                return false;

            return await _tagRepository.RemoveAsync(tag);
        }

        public async Task<List<Tag>> Search(string? tagName, string? note)
        {
            return await _tagRepository.Search(tagName, note);
        }
    }
}
