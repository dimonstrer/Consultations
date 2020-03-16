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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var patients = new Patient[20];
            for (var i = 0; i < 20; i++)
                patients[i] = new Patient {
                    PatientId = 5000 + i,
                    FirstName = $"Patient{i}",
                    LastName = "Patient",
                    BirthDate = DateTime.Now,
                    Gender = (i % 2 == 0 ? "Мужской" : "Женский"),
                    PensionNumber = $"{i * 100000}"
                };
            modelBuilder.Entity<Patient>().HasData(patients);
        }
        /// <summary>
        /// Метод конфигурации контекста.
        /// </summary>
        /// <param name="optionsBuilder">Настройщик конфигурации.</param>
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
