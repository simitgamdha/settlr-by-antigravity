using Microsoft.EntityFrameworkCore;
using Settlr.Data.IRepositories;
using Settlr.Models.Entities;

namespace Settlr.Data.Repositories;

public class GroupRepository : GenericRepository<Group>, IGroupRepository
{
    public GroupRepository(DbContext.ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Group>> GetUserGroupsAsync(int userId)
    {
        return await _context.GroupMembers
            .Where(gm => gm.UserId == userId)
            .Include(gm => gm.Group)
                .ThenInclude(g => g.CreatedBy)
            .Include(gm => gm.Group)
                .ThenInclude(g => g.Members)
                    .ThenInclude(m => m.User)
            .Select(gm => gm.Group)
            .ToListAsync();
    }

    public async Task<Group?> GetGroupWithMembersAsync(int groupId)
    {
        return await _dbSet
            .Include(g => g.CreatedBy)
            .Include(g => g.Members)
                .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(g => g.Id == groupId);
    }
}
