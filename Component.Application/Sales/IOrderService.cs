 using Component.Data.Entities;
using Component.ViewModels.Common;
using Component.ViewModels.Sales.Orders;

namespace Component.Application.Sales
{
    public interface IOrderService
    {
        Task<Order> Create(CheckoutRequest request); 
        Task<Order> GetById(int id);
        Task<CheckOrderResult<CheckOrderByCodeVm>> GetByCode(string code);
        Task<Order> GetLastestOrderId();
        Task<List<OrderVm>> GetAll(string keyword);
        Task<List<OrderVm>> GetAllOrderByOrderStatus(GetOrderByOrderStatusRequest request);
        Task<List<OrderVm>> GetUserOrderHistoryByOrderCode(GetUserOrderHistoryByOrderStatusRequest request);
        Task<CheckOrderResult<OrderDetailView>> GetOrderDetail(int id);
        Task<decimal> AccumulatedPoints(Guid uid, decimal price);
        Task<decimal> ReloadAccumylatedPoint(Guid uid, decimal price);
        Task<int> Vip(Guid uid, int point); 
        Task<decimal> TotalProfit (DateTime? startDate, DateTime? endDate);
        Task<int> CancelOrderRequest(CancelOrderRequest request);
        Task<int> RefundOrderRequest(CancelOrderRequest request);
        Task<int> OrderSuccess(int orderId);
        Task<int> OrderShipping(int orderId);
        Task<ApiResult<string>> ConfirmOrder(int orderId);
        Task<ApiResult<string>> InvoiceOrder(InvoiceOrderRequest request);
        Task<string> CreateVNPayPaymentUrlAsync(VNPayRequest request);
        //Task<BillHistoryDetailVM> GetBillById(int id);
        //Task<List<BillHistoryVM>> BillHistory(Guid id);
        //Task<PagedResult<OrderVm>> GetAllPaging(OrderPagingRequest request);
        //Task<PagedResult<OrderDetailView>> GetOrderDetailPagingRequest(OrderDetailPagingRequest request);
        //Task<int> UpdateStatus(UpdateStatusRequest request);
        //Task<decimal> PriceCalculator(decimal price, int quantity, string vip); // tinh gia san pham co discount
    }
}
