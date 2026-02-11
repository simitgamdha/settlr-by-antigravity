using Settlr.Models.Entities;

namespace Settlr.Data.IRepositories;

public interface IGroupMemberRepository : IGenericRepository<GroupMember>
{
    Task<bool> IsUserMemberOfGroupAsync(int userId, int groupId);
    Task<IEnumerable<GroupMember>> GetGroupMembersAsync(int groupId);
    Task<GroupMember?> GetMembershipAsync(int userId, int groupId);
}
