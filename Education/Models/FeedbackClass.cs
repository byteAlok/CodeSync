using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Education.Models
{
    public class FeedbackClass
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FeedbackId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public string? FeedbackEmail { get; set; }

        [Required]
        public string? FeedbackMessage { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string? FeedbackDate { get; set; }

    }
}
