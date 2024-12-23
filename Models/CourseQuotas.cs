using System.ComponentModel.DataAnnotations;

namespace BilgiYonetimSistemi.Models
{
    public class CourseQuotas
    {
        [Key]
        public int CourseID { get; set; }
        public int Quota { get; set; }
        public int RemainingQuota { get; set; }


        public virtual Course Course { get; set; }
    }

    public class CourseQuotaResponse
    {
        public int CourseID { get; set; }
        public int Quota { get; set; }
        public int RemainingQuota { get; set; }
        public Course Course { get; set; }  
    }

    public class CourseSelectionHistory
    {
        [Key]
        public int StudentID { get; set; }   
        public DateTime SelectionDate { get; set; }   
 
        public virtual Student? Student { get; set; }
    }

    public class NonConfirmedSelections
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime SelectedAt { get; set; }


        public virtual Student? Student { get; set; }
        public virtual Course? Course { get; set; }

    }

    public class CourseWithQuotaViewModel
    {
        public int CourseID { get; set; }
        public string CourseName { get; set; }
        public string CourseCode { get; set; }
        public int Credit { get; set; }
        public string Quota { get; set; }  
        public string RemainingQuota { get; set; }  
    }
}