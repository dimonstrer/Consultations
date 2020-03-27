using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultationsProject.Models.DTO
{
    /// <summary>
    /// Модель передачи данных пациента.
    /// </summary>
    public class PatientDTO
    {
        /// <summary>
        /// Имя пациента.
        /// </summary>
        [Display(Name = "Имя*")]
        [Required(ErrorMessage = "Не указано имя пациента")]
        public string FirstName { get; set; }

        /// <summary>
        /// Фамилия пациента.
        /// </summary>
        [Display(Name = "Фамилия*")]
        [Required(ErrorMessage = "Не указана фамилия пациента")]
        public string LastName { get; set; }

        /// <summary>
        /// Отчество пациента.
        /// </summary>
        [Display(Name = "Отчество (если присутствует)")]
        public string Patronymic { get; set; }

        /// <summary>
        /// Дата рождения пациента.
        /// </summary>
        [Display(Name = "Дата рождения*")]
        [Required(ErrorMessage = "Не указана дата рождения пациента")]
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// Пол пациента.
        /// </summary>
        [Display(Name = "Пол*")]
        [Required(ErrorMessage = "Не указан пол пациента")]
        public string Gender { get; set; }

        /// <summary>
        /// СНИЛС пациента.
        /// </summary>
        [Display(Name = "СНИЛС*")]
        [Required(ErrorMessage = "Не указан СНИЛС пациента")]
        public string PensionNumber { get; set; }
    }
}
