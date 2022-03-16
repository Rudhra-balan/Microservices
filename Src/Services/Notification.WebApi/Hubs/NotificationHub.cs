

namespace Notification.WebApi.Hubs
{
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var accountNumber = Context.User.Claims.FirstOrDefault(c => c.Type == ClaimType.Accountnumber);
            await Groups.AddToGroupAsync(Context.ConnectionId, accountNumber.Value);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
             var accountNumber = Context.User.Claims.FirstOrDefault(c => c.Type == ClaimType.Accountnumber);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, accountNumber.Value);
            await base.OnDisconnectedAsync(ex);
        }
    }
}
