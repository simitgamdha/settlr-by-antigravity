using Settlr.Models.Entities;

namespace Settlr.Data.IRepositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email);
}
