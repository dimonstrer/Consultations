using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultationsProject.Models
{
    /// <summary>
    /// Модель отображения списка всех пациентов.
    /// </summary>
    public class IndexViewModel
    {
        /// <summary>
        /// Список всех пациентов.
        /// </summary>
        public IEnumerable<Patient> Patients { get; set; }

        /// <summary>
        /// Модель пагинации для данного списка.
        /// </summary>
        public PageViewModel PageViewModel { get; set; }
    }
}
