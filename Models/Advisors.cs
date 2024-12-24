using System.ComponentModel.DataAnnotations;

namespace BilgiYonetimSistemi.Models
{
    public class Advisors
    {
        [Key]
        public int AdvisorID { get; set; }
        public string FullName { get; set; }
        public string Title { get; set; }
        public string Department { get; set; }
        public string Email { get; set; }

        public ICollection<Student>? Students { get; set; }
    }
}
