using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Settlr.Web.Hubs;

public class SettlrHub : Hub
{
    // Clients can join a group to receive updates specific to that group
    public async Task JoinGroup(string groupId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
    }

    // Clients can leave a group
    public async Task LeaveGroup(string groupId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId);
    }
}
