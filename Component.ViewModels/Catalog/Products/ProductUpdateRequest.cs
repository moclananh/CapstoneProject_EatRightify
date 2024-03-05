using Component.Data.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.ViewModels.Catalog.Products
{
    public class ProductUpdateRequest
    {
        public int Id { get; set; }
        public string Name { set; get; }
        public string Description { set; get; }
        public string Details { set; get; }
        public string SeoDescription { set; get; }
        public string SeoTitle { set; get; }
        public string SeoAlias { get; set; }
        public string LanguageId { set; get; }
        public Status Status { set; get; }
        public bool? IsFeatured { get; set; }
        public string ThumbnailImage { get; set; }
        public decimal Price { set; get; }
        public decimal Cost { set; get; }
        public decimal OriginalPrice { set; get; }
        public int Stock { set; get; }
    }
}
