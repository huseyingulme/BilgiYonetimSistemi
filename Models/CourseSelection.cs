using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BilgiYonetimSistemi.Models
{
    public class CourseSelection
    {
        [Key]
        public int SelectionID { get; set; }
        public int CourseID { get; set; }
        [Column(TypeName = "date")]
        public DateTime SelectionDate { get; set; }
        public bool IsApproved { get; set; }
        public int StudentID { get; set; }


        [ForeignKey("StudentID")]
        public virtual Student? Student { get; set; }

        [ForeignKey("CourseID")]
        public virtual Course? Course { get; set; }

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

    public class CourseSelectionHistory
    {
        [Key]
        public int StudentID { get; set; }
        public DateTime SelectionDate { get; set; }


        public virtual Student? Student { get; set; }
    }
}
