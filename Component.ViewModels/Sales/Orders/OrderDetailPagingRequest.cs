﻿using Component.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.ViewModels.Sales.Orders
{
    public class OrderDetailPagingRequest : PagingRequestBase
    {
        public int OrderId { get; set; }
        // public string LanguageId { get; set; }
    }
}
