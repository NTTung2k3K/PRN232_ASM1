using System.ComponentModel.DataAnnotations;

namespace Group7_SE1733_A01_FE.DTOs
{
    public class SystemAccountProfileDTO
    {
        [Required(ErrorMessage = "AccountId is required.")]
        public short AccountId { get; set; }
        [Required(ErrorMessage = "AccountName is required.")]
        public string AccountName { get; set; }

        [Required(ErrorMessage = "AccountEmail is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [MaxLength(100, ErrorMessage = "AccountEmail cannot exceed 100 characters.")]
        public string AccountEmail { get; set; }


        [Required(ErrorMessage = "AccountPassword is required.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "AccountPassword must be between 6 and 20 characters.")]
        public string AccountPassword { get; set; }
    }
}
