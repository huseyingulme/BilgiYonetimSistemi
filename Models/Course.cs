using System.ComponentModel.DataAnnotations;

namespace BilgiYonetimSistemi.Models
{
    public class Course
    {
        [Key]
        public int CourseID { get; set; }
        public string CourseCode{get; set;}
        public string CourseName{get; set;}
        public bool IsMandatory { get; set; }
        public int Credit { get; set; }
        public string Department{get; set;}

        public ICollection<CourseSelection> CourseSelection { get; set; } = new List<CourseSelection>();
        public  ICollection<NonConfirmedSelections> NonConfirmedSelections { get; set; }
    }
}
