using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Education.Models
{
    public class ContactClass
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ContactId { get; set; }

        [Required(ErrorMessage = "Enter Your Email")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string ContactEmail { get; set; }


        [Required(ErrorMessage = "Write your message")]
        public string ContactMessage { get; set; }

        // This User Id have forein key relation from user table 
        [Required(ErrorMessage = "Enter UserId")]
        public int UserId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string ContactDate { get; set; }
    }
}
