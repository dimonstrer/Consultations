using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ConsultationsProject.Models
{
    /// <summary>
    /// Сущность пациента.
    /// </summary>
    public class Patient
    {
        /// <summary>
        /// Id пациента в БД (ПК).
        /// </summary>
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PatientId { get; set; }

        /// <summary>
        /// Имя пациента.
        /// </summary>
        [Display(Name="Имя*")]
        [Required(ErrorMessage ="Не указано имя пациента")]
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

        /// <summary>
        /// Список консультаций пациента (навигационное св-во).
        /// </summary>
        [JsonIgnore]
        public List<Consultation> Consultations { get; set; }
    }
}
