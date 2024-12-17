using System.ComponentModel.DataAnnotations;

namespace BilgiYonetimSistemi.Models
{
    public class CourseCapacity
    {
        [Key]
        public int CourseID { get; set; }
        public int Capacity { get; set; }
        public int RemainingCapacity { get; set; }


        public Course Course { get; set; }
    }

    public class CourseWithCapacityModel
    {
        public int CourseID { get; set; }
        public string CourseName { get; set; }
        public string CourseCode { get; set; }
        public int Credit { get; set; }
        public string Capacity { get; set; }  
        public string RemainingCapacity { get; set; }  
    }
}
