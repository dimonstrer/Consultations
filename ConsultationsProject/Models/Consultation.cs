using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultationsProject.Models
{
    public class Consultation
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ConsultationId { get; set; }
        [Display(Name ="Дата")]
        [Required]
        public DateTime Day { get; set; }
        [Display(Name = "Время")]
        [Required]
        public DateTime Time { get; set; }
        [Display(Name = "Симптомы")]
        public string Symptoms { get; set; }
        public int PatientId { get; set; }
        public Patient Patient { get; set; }
    }
}
