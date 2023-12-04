using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.Utilities.Constants
{
    public class SystemConstants
    {
        public const string MainConnectionString = "DefaultConnection";
        public const string CartSession = "CartSession";

        public class AppSettings
        {
            public const string DefaultLanguageId = "DefaultLanguageId";
            public const string Token = "Token";
            public const string ID = "ID";
            public const string BaseAddress = "BaseAddress";
            public const string UserBaseAddress = "UserBaseAddress";
        }

        public class ProductSettings
        {
            public const int NumberOfFeaturedProducts = 4; //stupid skip() func not understand distince. no hieu rang neu assign mutil cate thi bay nhiu product, nen skip mat cac sp cu
            public const int NumberOfLatestProducts = 6;
        }

       /* public class ProductConstants
        {
            public const string NA = "N/A";
        }*/
    }
}
