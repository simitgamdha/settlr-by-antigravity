using Microsoft.EntityFrameworkCore;
using Settlr.Data.IRepositories;
using Settlr.Models.Entities;

namespace Settlr.Data.Repositories;

public class GroupMemberRepository : GenericRepository<GroupMember>, IGroupMemberRepository
{
    public GroupMemberRepository(DbContext.ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> IsUserMemberOfGroupAsync(int userId, int groupId)
    {
        return await _dbSet.AnyAsync(gm => gm.UserId == userId && gm.GroupId == groupId);
    }

    public async Task<IEnumerable<GroupMember>> GetGroupMembersAsync(int groupId)
    {
        return await _dbSet
            .Where(gm => gm.GroupId == groupId)
            .Include(gm => gm.User)
            .ToListAsync();
    }

    public async Task<GroupMember?> GetMembershipAsync(int userId, int groupId)
    {
        return await _dbSet.FirstOrDefaultAsync(gm => gm.UserId == userId && gm.GroupId == groupId);
    }
}
