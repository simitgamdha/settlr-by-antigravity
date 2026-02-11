using Settlr.Models.Entities;

namespace Settlr.Data.IRepositories;

public interface IGroupRepository : IGenericRepository<Group>
{
    Task<IEnumerable<Group>> GetUserGroupsAsync(int userId);
    Task<Group?> GetGroupWithMembersAsync(int groupId);
}
