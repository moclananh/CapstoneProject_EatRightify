using Component.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.ViewModels.AI
{
    public class UpdateResultRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public ResultStatus Status { get; set; }
        
    }
}
