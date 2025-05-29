using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs
{
    public class NewsArticleDTO
    {
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

        public List<TagDTO> Tags { get; set; }
    }

    public class NewsArticleCreateDTO
    {
        public int NewsArticleId { get; set; }
        public string NewsTitle { get; set; }
        public string Headline { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string NewsContent { get; set; }
        public string NewsSource { get; set; }
        public short? CategoryId { get; set; }
        public bool? NewsStatus { get; set; }
        public short? CreatedById { get; set; }

        public List<TagCreateDTO> Tags { get; set; }
    }

    public class TagCreateDTO
    {
        public string TagName { get; set; }
        public string Note { get; set; }
    }

    public class TagDTO
    {
        public int TagId { get; set; }
        public string TagName { get; set; }
        public string Note { get; set; }
    }

}
