using Component.ViewModels.Common;
using Component.ViewModels.Statistical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.Application.Statistical
{
    public interface IStatisticalService
    {
        Task<PagedResult<StatisticalVm>> Statistical(StatisticalRequest request);
    }
}
