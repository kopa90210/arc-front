using Myshop.Application.Dtos;

namespace Myshop.Application.Interfaces
{
    public interface IOrderNotifier
    {
         Task NotifyVendorNewOrder(string vendorId, OrderNotificationDto notification);
        Task NotifyCustomerOrderUpdate(string userId, OrderNotificationDto notification);
    }
}