using Component.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.ViewModels.Utilities.Blogs
{
    public class BlogVm
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { set; get; }
        public string Url { set; get; }
        public string Image { get; set; }
        public int SortOrder { get; set; }
        public DateTime DateCreate { get; set; }
        public Status Status { set; get; }
    }
}
