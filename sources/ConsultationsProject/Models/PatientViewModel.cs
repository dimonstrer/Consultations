using ConsultationsProject.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultationsProject.Models
{   /// <summary>
    /// Модель отображения списка всех пациентов.
     /// </summary>
    public class PatientViewModel
    {
        /// <summary>
        /// Сущность пациента.
        /// </summary>
        public PatientDTO Patient { get; set; }

        /// <summary>
        /// Модель пагинации для данного списка.
        /// </summary>
        public PageViewModel PageViewModel { get; set; }
    }
}
