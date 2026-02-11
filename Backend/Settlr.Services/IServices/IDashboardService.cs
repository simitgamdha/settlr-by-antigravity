using Settlr.Common.Response;
using Settlr.Models.Dtos.ResponseDtos;

namespace Settlr.Services.IServices;

public interface IDashboardService
{
    Task<Response<DashboardResponseDto>> GetDashboardDataAsync(int userId);
}
