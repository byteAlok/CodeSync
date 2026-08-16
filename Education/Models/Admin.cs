using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.CodeAnalysis.Completion;

namespace Education.Models
{
    public class Admin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AdminId { get; set; }
     
        [Required(ErrorMessage = "Enter Adminname")]
        [StringLength(30, ErrorMessage = "Adminname can't exceed 30 characters")]
        public string AdminName { get; set; } = string.Empty;
        public string? AdminDOB { get; set; }

        [Required(ErrorMessage = "Enter Email")]
        [EmailAddress(ErrorMessage = "Enter Valid Email")]
        public string AdminEmail { get; set; } = string.Empty;
        [Required(ErrorMessage = "Enter Password")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 20")]
        public string? AdminPassword { get; set; }
        [NotMapped]
        [DataType(DataType.Password)]
        [Compare("AdminPassword", ErrorMessage = "Password and Confirm Password no matched")]
        public string? AdminConfirmPassword { get; set; }
        public string? AdminImage { get; set; }
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]

        [Required]
        public string? AdminPhone { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string? AdminRegisterDate { get; set; }
    

    }
}
