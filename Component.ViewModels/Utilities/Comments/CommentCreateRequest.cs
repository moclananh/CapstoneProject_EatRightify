using System.ComponentModel.DataAnnotations;


namespace Component.ViewModels.Utilities.Comments
{
    public class CommentCreateRequest
    {
        public Guid UserId { get; set; }
        public int ProductId { get; set; }
        public string Content { get; set; }
        [Range(1,5)]
        public int Grade { get; set; }
    }
}
