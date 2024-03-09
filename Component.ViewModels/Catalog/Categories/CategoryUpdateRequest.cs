using Component.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.ViewModels.Catalog.Categories
{
    public class CategoryUpdateRequest
    {
        public int Id { get; set; }
        public string Name { set; get; }
        public Status Status { set; get; }
    }
}
