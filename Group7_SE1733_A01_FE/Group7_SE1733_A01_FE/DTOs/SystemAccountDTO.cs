using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Group7_SE1733_A01_FE.Response
{
    public class SystemAccountDTO
    {
        public short AccountId { get; set; }

        public string AccountName { get; set; }

        public string AccountEmail { get; set; }

        public int? AccountRole { get; set; }
        public string AccountPassword { get; set; }


    }
    public class SystemAccountCreateDTO
    {

        [Required(ErrorMessage = "AccountName is required.")]
        public string AccountName { get; set; }

        [Required(ErrorMessage = "AccountEmail is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [MaxLength(100, ErrorMessage = "AccountEmail cannot exceed 100 characters.")]
        public string AccountEmail { get; set; }

        [Required(ErrorMessage = "AccountRole is required.")]
        [Range(1, 2, ErrorMessage = "AccountRole must be 1 for Staff or 2 for Lecturer.")]
        public int? AccountRole { get; set; }

        [Required(ErrorMessage = "AccountPassword is required.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "AccountPassword must be between 6 and 20 characters.")]
        public string AccountPassword { get; set; }


    }
    public class SystemAccountUpdateDTO
    {
        [Required(ErrorMessage = "AccountId is required.")]
        public short AccountId { get; set; }

        [Required(ErrorMessage = "AccountName is required.")]
        public string AccountName { get; set; }

        [Required(ErrorMessage = "AccountEmail is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [MaxLength(100, ErrorMessage = "AccountEmail cannot exceed 100 characters.")]
        public string AccountEmail { get; set; }

        [Required(ErrorMessage = "AccountRole is required.")]
        [Range(1, 2, ErrorMessage = "AccountRole must be 1 for Staff or 2 for Lecturer.")]
        public int? AccountRole { get; set; }

        [Required(ErrorMessage = "AccountPassword is required.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "AccountPassword must be between 6 and 20 characters.")]
        public string AccountPassword { get; set; }


    }
}
