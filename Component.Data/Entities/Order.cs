﻿using Component.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.Data.Entities
{
    public class Order
    {
        public int Id { set; get; }
        public DateTime OrderDate { set; get; }
        public Guid UserId { set; get; }
        public string ShipName { set; get; }
        public string ShipAddress { set; get; }
        public string ShipEmail { set; get; }
        public string ShipPhoneNumber { set; get; }
        public OrderStatus Status { set; get; }
        public decimal TotalPriceOfOrder { set; get; }
        public string OrderCode { set; get; }
        public string? CancelDescription {  set; get; }
        public string? RefundDescription { set; get; }
        public DateTime? RefundDate { set; get; }
        public DateTime? ReceivedDate { set; get; }
        public int OrderMethod {  set; get; }
        public string? InvoiceLink { set; get; }
        public List<OrderDetail> OrderDetails { get; set; }
        public AppUser AppUser { get; set; }

    }
}
