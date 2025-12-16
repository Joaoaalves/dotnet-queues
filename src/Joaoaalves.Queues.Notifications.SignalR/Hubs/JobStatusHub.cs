using Microsoft.AspNetCore.SignalR;

namespace Joaoaalves.Queues.Notifications.SignalR.Hubs
{
    public sealed class JobStatusHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;

            var groupName = GetGroupName(connectionId);

            await Groups.AddToGroupAsync(connectionId, groupName);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionId = Context.ConnectionId;

            var groupName = GetGroupName(connectionId);
            await Groups.RemoveFromGroupAsync(connectionId, groupName);

            await base.OnDisconnectedAsync(exception);
        }

        public static string GetGroupName(string connectionId) => $"job:{connectionId}";
    }
}