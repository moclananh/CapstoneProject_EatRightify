﻿using Component.Data.Entities;
using Component.ViewModels.Common;
using Component.ViewModels.Sales.Bills;
using Component.ViewModels.Sales.Orders;
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

        Task<Order> GetLastestOrderId();

        Task<PagedResult<OrderVm>> GetAllPaging(OrderPagingRequest request);
        Task<PagedResult<OrderDetailView>> GetOrderDetailPagingRequest(OrderDetailPagingRequest request);
        Task<BillHistoryDetailVM> GetBillById(int id);
        Task<List<BillHistoryVM>> BillHistory(Guid id);

        Task<int> UpdateStatus(UpdateStatusRequest request);

    }
}
