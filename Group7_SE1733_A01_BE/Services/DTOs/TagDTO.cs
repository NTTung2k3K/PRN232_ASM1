using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs
{
    public class TagDTO
    {
        public int TagId { get; set; }

        public string TagName { get; set; }

        public string Note { get; set; }
    }

    public class TagCreateDTO
    {
        public int TagId { get; set; }

        public string TagName { get; set; }

        public string Note { get; set; }
    }

    public class TagUpdateDTO
    {
        public string TagName { get; set; }

        public string Note { get; set; }
    }
}
