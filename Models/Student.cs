using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BilgiYonetimSistemi.Models
{
    public class Student
    {
        [Key]
        public int StudentID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int? AdvisorID { get; set; }

        [ForeignKey("AdvisorID")]
        public Advisors Advisors { get; set; }
        public DateTime EnrollmentDate { get; set; }

        public ICollection<StudentCourseSelections> StudentCourseSelections { get; set; }
        public ICollection<NonConfirmedSelections> NonConfirmedSelections { get; set; }


    }
}
