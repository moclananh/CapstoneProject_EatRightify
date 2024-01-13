using Component.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.ViewModels.Utilities.Comments
{
    public class CommentUpdateRequest
    {
        public int Id { get; set; }
        public string Content { get; set; }
        [Range(1,5)]
        public int Grade { get; set; }

    }
}
