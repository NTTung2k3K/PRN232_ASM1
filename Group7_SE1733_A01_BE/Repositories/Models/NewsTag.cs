using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Models
{
    public class NewsTag
    {
        public string NewsArticleId { get; set; }
        public int TagId { get; set; }

        public NewsArticle NewsArticle { get; set; }
        public Tag Tag { get; set; }
    }

}
