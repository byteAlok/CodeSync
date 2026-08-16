using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Education.Models
{
    public class UserClass
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Enter Your Full Name!")]
        [StringLength(50, ErrorMessage = "Full Name cannot exceed 50 characters.")]
        public string UserFullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter User Name!")]
        [StringLength(30, ErrorMessage = "User Name cannot exceed 30 characters.")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter Your Email!")]
        [EmailAddress(ErrorMessage = "Invalid Email Address!")]
        public string UserEmail { get; set; } = string.Empty;
        public string? UserDOB { get; set; }

        [Required(ErrorMessage = "Enter Password!")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 20 characters!")]
        public string UserPassword { get; set; } = string.Empty;

        [NotMapped]
        [DataType(DataType.Password)]
        [Compare("UserPassword", ErrorMessage = "Password and Confirm Password do not match!")]
        public string? UserConfirmPassword { get; set; }  // nullable since not stored in DB

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string? UserRegisterDate { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public bool? UserExist { get; set; }     // may be NULL from DB

        public string? UserImage { get; set; }            // may be NULL if user didn’t upload image
    }
}
