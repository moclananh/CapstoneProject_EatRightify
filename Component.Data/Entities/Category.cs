using Component.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.Data.Entities
{
    public class Category
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public Status Status { set; get; }
        public List<ProductInCategory> ProductInCategories { get; set; }
    }
}
