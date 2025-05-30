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

    public async Task<List<Tag>> Search(string? TagName, string? Note)
    {
        var items = await _context.Tags
            .Where(i => (i.TagName.ToString().Contains(TagName) || string.IsNullOrEmpty(TagName))
            && (i.Note.ToString().Contains(Note) || string.IsNullOrEmpty(Note)))
            .ToListAsync();

        return items;
    }
}
