using Group7_SE1733_A01_FE.Response;

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

        public virtual CategoryDTO? Category { get; set; }

        public virtual SystemAccountDTO? CreatedBy { get; set; }

        public virtual ICollection<TagDTO> Tags { get; set; } = new List<TagDTO>();
    }
}
