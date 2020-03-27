using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultationsProject.Models.DTO
{
    /// <summary>
    /// Модель передачи данных консультации пациента.
    /// </summary>
    public class ConsultationDTO
    {
        /// <summary>
        /// День консультации пациента.
        /// </summary>
        [Display(Name = "Дата*")]
        [Required(ErrorMessage = "Не указана дата консультации")]
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
    }
}
