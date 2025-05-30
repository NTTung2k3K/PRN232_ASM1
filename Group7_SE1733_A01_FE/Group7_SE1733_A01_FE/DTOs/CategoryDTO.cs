namespace Group7_SE1733_A01_FE.DTOs
{
    public class CategoryDTO
    {
        public short CategoryId { get; set; }

        public string CategoryName { get; set; }

        public string CategoryDesciption { get; set; }

        public short? ParentCategoryId { get; set; }

        public bool? IsActive { get; set; }

        public virtual ICollection<CategoryDTO> InverseParentCategory { get; set; } = new List<CategoryDTO>();


        public virtual CategoryDTO ParentCategory { get; set; }
    }

    public class CategoryCreateDTO
    {

        public string CategoryName { get; set; }

        public string CategoryDesciption { get; set; }

        public short? ParentCategoryId { get; set; }

        public bool? IsActive { get; set; }

    }

    public class CategoryUpdateDTO
    {

        public string CategoryName { get; set; }

        public string CategoryDesciption { get; set; }

        public short? ParentCategoryId { get; set; }

        public bool? IsActive { get; set; }

    }
}
