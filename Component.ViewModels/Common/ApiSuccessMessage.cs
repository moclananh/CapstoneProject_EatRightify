using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.ViewModels.Common
{
    public class ApiSuccessMessage<T> : ApiResult<T>
    {
        public ApiSuccessMessage() { }
        public ApiSuccessMessage(string message)
        {
            IsSuccessed= true;
            Message = message;
        }

    }
}
