using System.ComponentModel.DataAnnotations;

namespace BilgiYonetimSistemi.Models
{
    public class CourseQuotas
    {
        [Key]
        public int CourseId { get; set; }
        public int Quota { get; set; }
        public int RemainingQuota { get; set; }


        public virtual Course Course { get; set; }
    }
    public class CourseQuotaResponse
    {
        public int CourseId { get; set; }
        public int Quota { get; set; }
        public int RemainingQuota { get; set; }
        public Course Course { get; set; } // Bu alanda course verisi null geliyor, gerekirse daha fazla özelleştirebilirsiniz
    }

    public class CourseSelectionHistory
    {
        [Key]
        public int StudentID { get; set; }  // Öğrenci ID'si
        public DateTime SelectionDate { get; set; }  // Seçim tarihi

        // İlişkiler
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
}