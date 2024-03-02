using Component.Data.Entities;
using Component.ViewModels.Common;
using Component.ViewModels.Sales.Bills;
using Component.ViewModels.Sales.Orders;
using Component.ViewModels.Utilities.Blogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.Application.Sales
{
    public interface IOrderService
    {
        Task<Order> Create(CheckoutRequest request); 

        Task<Order> GetById(int id);
        Task<CheckOrderResult<CheckOrderByCodeVm>> GetByCode(Guid code);
        Task<Order> GetLastestOrderId();
        Task<List<OrderVm>> GetAll(string keyword);
        Task<List<OrderVm>> GetAllOrderByOrderStatus(GetOrderByOrderStatusRequest request);
        Task<List<OrderVm>> GetUserOrderHistoryByOrderCode(GetUserOrderHistoryByOrderStatusRequest request);

        Task<PagedResult<OrderVm>> GetAllPaging(OrderPagingRequest request);
        Task<PagedResult<OrderDetailView>> GetOrderDetailPagingRequest(OrderDetailPagingRequest request);
        Task<List<OrderDetailView>> GetOrderDetail(int id);
        Task<BillHistoryDetailVM> GetBillById(int id);
        Task<List<BillHistoryVM>> BillHistory(Guid id);

        Task<int> UpdateStatus(UpdateStatusRequest request);
        Task<decimal> PriceCalculator(decimal price, int quantity, string vip); // tinh gia san pham co discount
        Task<decimal> AccumulatedPoints(string uid, decimal price); // cong diem tich luy
        Task<int> Vip(string uid, int point); // set trang thai vip

    }
}
