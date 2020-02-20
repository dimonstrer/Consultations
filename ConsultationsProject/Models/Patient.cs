using Microsoft.AspNetCore.Mvc;
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
        [Display(Name="Имя*")]
        [Required(ErrorMessage ="Не указано имя пациента")]
        public string FirstName { get; set; }
        [Display(Name = "Фамилия*")]
        [Required(ErrorMessage = "Не указана фамилия пациента")]
        public string LastName { get; set; }
        [Display(Name = "Отчество (если присутствует)")]
        public string Patronymic { get; set; }
        [Display(Name = "Дата рождения*")]
        [Required(ErrorMessage = "Не указана дата рождения пациента")]
        public DateTime BirthDate { get; set; }
        [Display(Name = "Пол*")]
        [Required(ErrorMessage = "Не указан пол пациента")]
        public string Gender { get; set; }
        [Display(Name = "СНИЛС*")]
        [Required(ErrorMessage = "Не указан СНИЛС пациента")]
        public string PensionNumber { get; set; }
        public List<Consultation> Consultations { get; set; }
    }
}
