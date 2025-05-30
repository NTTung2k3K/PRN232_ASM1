using System.ComponentModel.DataAnnotations;

namespace Group7_SE1733_A01_FE.DTOs
{
    public class TagDTO
    {
        public int TagId { get; set; }

        public string TagName { get; set; }

        public string Note { get; set; }

    }

    public class TagCreateDTO
    {
        [Required(ErrorMessage = "TagName is required.")]
        public string TagName { get; set; }
        [MaxLength(500, ErrorMessage = "Note cannot exceed 500 characters.")]
        public string Note { get; set; }
    }
}
