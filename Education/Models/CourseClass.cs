using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Education.Models
{
    public class CourseClass
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CourseId { get; set; }

        //[Required(ErrorMessage = "Enter Your Course Name!")]
        [StringLength(50, ErrorMessage = "Full Name cannot exceed 50 characters.")]
        public string CourseName { get; set; } = string.Empty;

        public int CoursePrice { get; set; }

        public string? CourseStartDate {  get; set; }
        public string? CourseEndDate { get; set; }
        public string? CourseDuration {  get; set; }
        public string? CourseLanguage {  get; set; }
        public string? CourseImage {  get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string? CourseLaunchDate {  get; set; }
    }
}
