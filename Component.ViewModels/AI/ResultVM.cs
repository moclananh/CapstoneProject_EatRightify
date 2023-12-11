using Component.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.ViewModels.AI
{
    public class ResultVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public DateTime ResultDate { get; set; }
        public ResultStatus Status { get; set; }
        public bool IsSend { get; set; }
    }
}
