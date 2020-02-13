using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultationsProject.Models
{
    public class Patient
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PatientId { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }
        [Required]
        public string PensionNumber { get; set; }
        public List<Consultation> Consultations { get; set; }
    }
}
