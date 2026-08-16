using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Education.Models
{
    public class UserCourseClass
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserCourseId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string? EnrollDate { get; set; }

        public int UserId {  get; set; }
        public int CourseId {  get; set; }
    }
}
