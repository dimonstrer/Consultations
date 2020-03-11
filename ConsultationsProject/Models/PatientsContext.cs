using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultationsProject.Models
{
    /// <summary>
    /// Контекст БД с пациентами и их консультациями.
    /// </summary>
    public class PatientsContext : DbContext
    {
        /// <summary>
        /// Конструктор контекста.
        /// </summary>
        public PatientsContext()
        {
        }

        /// <summary>
        /// Метод конфигурации контекста.
        /// </summary>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ConsultationsProjectDB;Trusted_Connection=True;");
        }

        /// <summary>
        /// Таблица пациентов.
        /// </summary>
        public DbSet<Patient> Patients { get; set; }
        /// <summary>
        /// Таблица консультаций пациентов.
        /// </summary>
        public DbSet<Consultation> Consultations { get; set; }
    }
}
