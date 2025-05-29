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

    public async Task<Tag> GetByIdAsync(int id)
    {
        return await _context.Tags.FirstOrDefaultAsync(x => x.TagId == id);
    }

    public async Task<List<Tag>> FindByConditionAsync(Expression<Func<Tag, bool>> predicate)
    {
        return await _context.Tags.Where(predicate).ToListAsync();
    }
}
