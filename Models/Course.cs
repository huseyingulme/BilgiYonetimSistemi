using System.ComponentModel.DataAnnotations;

namespace BilgiYonetimSistemi.Models
{
    public class Course
    {
        [Key]
        public int CourseID { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public bool IsMandatory { get; set; }
        public int Credit { get; set; }
        public string Department { get; set; }

        // Change Advisors to Advisor (singular)
        public Advisors Advisor { get; set; }  // One-to-one relationship with Advisor

        public ICollection<StudentCourseSelections> StudentCourseSelections { get; set; }
        public virtual ICollection<NonConfirmedSelections> NonConfirmedSelections { get; set; }
    }
}
