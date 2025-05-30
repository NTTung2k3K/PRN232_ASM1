using Group7_SE1733_A01_FE.Response;
using System.ComponentModel.DataAnnotations;

namespace Group7_SE1733_A01_FE.DTOs
{
    public class NewsArticleDTO
    {
        public string NewsArticleId { get; set; }

        public string NewsTitle { get; set; }

        public string Headline { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string NewsContent { get; set; }

        public string NewsSource { get; set; }

        public short? CategoryId { get; set; }

        public bool? NewsStatus { get; set; }

        public short? CreatedById { get; set; }

        public short? UpdatedById { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public virtual CategoryDTO Category { get; set; }

        public virtual SystemAccountDTO? CreatedBy { get; set; }

        public List<int> TagIds { get; set; } = new List<int>();

        public List<TagDTO> Tags { get; set; } = new List<TagDTO>();
    }

    public class NewsArticleCreateDTO
    {
        public int NewsArticleId { get; set; }

        [Required(ErrorMessage = "NewsTitle is required.")]
        public string NewsTitle { get; set; }

        [Required(ErrorMessage = "Headline is required.")]
        public string Headline { get; set; }

        public DateTime? CreatedDate { get; set; }

        [Required(ErrorMessage = "NewsContent is required.")]
        public string NewsContent { get; set; }

        [Required(ErrorMessage = "NewsSource is required.")]
        public string NewsSource { get; set; }

        [Required(ErrorMessage = "CategoryId is required.")]
        public short? CategoryId { get; set; }

        [Required(ErrorMessage = "NewsStatus is required.")]
        public bool? NewsStatus { get; set; }

        public short? CreatedById { get; set; }

        public List<int> TagIds { get; set; } = new List<int>();
    }

    public class NewsArticleUpdateDTO
    {
        public string NewsArticleId { get; set; }

        [Required(ErrorMessage = "NewsTitle is required.")]
        public string NewsTitle { get; set; }

        [Required(ErrorMessage = "Headline is required.")]
        public string Headline { get; set; }

        [Required(ErrorMessage = "NewsContent is required.")]
        public string NewsContent { get; set; }

        [Required(ErrorMessage = "NewsSource is required.")]
        public string NewsSource { get; set; }

        [Required(ErrorMessage = "CategoryId is required.")]
        public short? CategoryId { get; set; }

        [Required(ErrorMessage = "NewsStatus is required.")]
        public bool? NewsStatus { get; set; }

        public short? UpdatedById { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public List<TagDTO>? Tags { get; set; }

        public List<int> TagIds { get; set; } = new List<int>();
    }
}
