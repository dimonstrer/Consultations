using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultationsProject.Models
{
    /// <summary>
    /// Сущность консультации пациента.
    /// </summary>
    public class Consultation
    {
        /// <summary>
        /// ID консультации пациента в БД (ПК).
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ConsultationId { get; set; }

        /// <summary>
        /// День консультации пациента.
        /// </summary>
        [Display(Name ="Дата*")]
        [Required(ErrorMessage ="Не указана дата консультации")]
        public DateTime Day { get; set; }

        /// <summary>
        /// Время консультации пациента.
        /// </summary>
        [Display(Name = "Время*")]
        [Required(ErrorMessage = "Не указано время консультации")]
        public DateTime Time { get; set; }

        /// <summary>
        /// Симптомы пациента, записанные на консультации.
        /// </summary>
        [Display(Name = "Симптомы")]
        public string Symptoms { get; set; }

        /// <summary>
        /// ID пациента в БД (ВК).
        /// </summary>
        public int PatientId { get; set; }

        /// <summary>
        /// Пациент (навигационное св-во).
        /// </summary>
        public Patient Patient { get; set; }
    }
}
