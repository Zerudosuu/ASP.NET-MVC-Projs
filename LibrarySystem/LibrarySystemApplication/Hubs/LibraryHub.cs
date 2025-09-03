using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

namespace LibrarySystemApplication.Hubs
{
    public class LibraryHub : Hub
    {
        // Send notification to all connected librarians
        public async Task NotifyLibrarian(string message)
        {
            await Clients.Group("Librarians").SendAsync("ReceiveNotification", message);
        }

        // Send notification to a specific user (e.g., Member)
        public async Task NotifyMember(string memberId, string message)
        {
            await Clients.User(memberId).SendAsync("ReceiveNotification", message);
        }

        public override async Task OnConnectedAsync()
        {
            var user = Context.User; // ✅ Correct
            if (user != null && user.IsInRole("Librarian"))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Librarians");
            }
            await base.OnConnectedAsync();
        }
    }
}
