using Microsoft.AspNetCore.SignalR;
using KarnelTravels.API.DTOs;

namespace KarnelTravels.API.Hubs;

public interface INotificationClient
{
    Task ReceiveBookingNotification(BookingNotificationDto notification);
    Task ReceiveContactNotification(UnreadContactDto contact);
    Task ReceivePromotionUpdate(PromotionUpdateDto promotion);
}

public class NotificationHub : Hub<INotificationClient>
{
    public async Task JoinAdminGroup()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "AdminGroup");
    }

    public async Task JoinUserGroup()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "UserGroup");
    }

    public async Task LeaveAdminGroup()
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "AdminGroup");
    }

    public async Task LeaveUserGroup()
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "UserGroup");
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}

// Service to send notifications from controllers
public interface INotificationService
{
    Task SendBookingNotification(BookingNotificationDto notification);
    Task SendContactNotification(UnreadContactDto contact);
    Task SendPromotionUpdate(PromotionUpdateDto promotion);
}

public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;

    public NotificationService(IHubContext<NotificationHub, INotificationClient> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendBookingNotification(BookingNotificationDto notification)
    {
        await _hubContext.Clients.Group("AdminGroup").ReceiveBookingNotification(notification);
    }

    public async Task SendContactNotification(UnreadContactDto contact)
    {
        await _hubContext.Clients.Group("AdminGroup").ReceiveContactNotification(contact);
    }

    public async Task SendPromotionUpdate(PromotionUpdateDto promotion)
    {
        // Send to all users (both admin and regular users)
        await _hubContext.Clients.Group("AdminGroup").ReceivePromotionUpdate(promotion);
        await _hubContext.Clients.Group("UserGroup").ReceivePromotionUpdate(promotion);
        // Also broadcast to all connected clients
        await _hubContext.Clients.All.ReceivePromotionUpdate(promotion);
    }
}
