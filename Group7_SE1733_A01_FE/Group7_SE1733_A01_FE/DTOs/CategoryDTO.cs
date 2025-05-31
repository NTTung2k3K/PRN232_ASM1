using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Group7_SE1733_A01_FE.DTOs
{
    public class CategoryDTO
    {
        public short CategoryId { get; set; }

        public string CategoryName { get; set; }

        public string CategoryDesciption { get; set; }

        public short? ParentCategoryId { get; set; }

        public bool? IsActive { get; set; }
        [JsonIgnore]
        public virtual ICollection<CategoryDTO> InverseParentCategory { get; set; } = new List<CategoryDTO>();


        public virtual CategoryDTO ParentCategory { get; set; }
    }

    public class CategoryDisplayDTO
    {
        public short CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryDesciption { get; set; } = string.Empty;
        public short? ParentCategoryId { get; set; }
        public bool? IsActive { get; set; }
        public CategoryDisplayDTO? ParentCategory { get; set; }
    }

    public class CategoryCreateDTO
    {
        [Required(ErrorMessage = "CategoryName is required.")]
        public string CategoryName { get; set; }

        [Required(ErrorMessage = "CategoryDesciption is required.")]
        public string CategoryDesciption { get; set; }

        [Required(ErrorMessage = "ParentCategoryId is required.")]
        public short? ParentCategoryId { get; set; }

        [Required(ErrorMessage = "IsActive is required.")]
        public bool? IsActive { get; set; }

    }

    public class CategoryUpdateDTO
    {
        [Required(ErrorMessage = "CategoryName is required.")]
        public string CategoryName { get; set; }

        [Required(ErrorMessage = "CategoryDesciption is required.")]
        public string CategoryDesciption { get; set; }

        [Required(ErrorMessage = "ParentCategoryId is required.")]
        public short? ParentCategoryId { get; set; }

        [Required(ErrorMessage = "IsActive is required.")]
        public bool? IsActive { get; set; }

    }
}
