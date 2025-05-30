using Microsoft.EntityFrameworkCore;
using Repositories.Base;
using Repositories.Models;
using System.Linq.Expressions;

public class TagRepository : GenericRepository<Tag>
{
    public TagRepository() { }

    public async Task<List<Tag>> GetAll()
    {
        return await _context.Tags.ToListAsync();
    }

    public async Task<List<Tag>> FindByConditionAsync(Expression<Func<Tag, bool>> predicate)
    {
        return await _context.Tags.Where(predicate).ToListAsync();
    }

    public async Task<int> GetMaxTagIdAsync()
    {
        // Nếu bảng Tag chưa có bản ghi nào thì trả về 0
        return await _context.Tags.AnyAsync()
            ? await _context.Tags.MaxAsync(t => t.TagId)
            : 0;
    }

}
