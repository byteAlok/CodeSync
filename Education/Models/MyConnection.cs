using Microsoft.EntityFrameworkCore;

namespace Education.Models
{
    public class MyConnection : DbContext
    {
        public MyConnection(DbContextOptions<MyConnection> options) : base(options) { }
        public DbSet<UserClass> User { get; set; }
        public DbSet<ContactClass> Contact { get; set; }
        public DbSet<FeedbackClass> Feedback { get; set; }
        public DbSet<CourseClass> Course { get; set; }
        public DbSet<UserCourseClass> UserCourse { get; set; }
        public DbSet<Admin> Admin { get; set; }


    }
}
