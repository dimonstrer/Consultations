using ConsultationsProject.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsultationsProject.Models.Services
{
    /// <summary>
    /// Класс, инкапсулирующий бизнес логику взаимодействия с пациентами и их консультациями.
    /// </summary>
    public class PatientService : IPatientService
    {
        /// <summary>
        /// Контекст БД с пациентами и их консультациями.
        /// </summary>
        private readonly PatientsContext patientsContext;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="patientsContext">Объект БД с пациентами и их консультациями.</param>
        public PatientService(PatientsContext patientsContext)
        {
            this.patientsContext = patientsContext;
        }

        /// <summary>
        /// Метод добавления пациента в БД с проверкой на уникальность по СНИЛСу.
        /// </summary>
        /// <param name="patient">Данные пациента.</param>
        /// <returns>
        /// TRUE, если пациент успешно добавлен в БД.
        /// FALSE, если найден пациент с таким же СНИЛС.
        /// </returns>
        public bool AddPatient(Patient patient)
        {
            patient.PensionNumber = Regex.Replace(patient.PensionNumber, "[^0-9]", "");

            var pensionCheck = patientsContext.Patients
                        .Where(x => x.PensionNumber == patient.PensionNumber)
                        .FirstOrDefault();

            if ((pensionCheck == null) || (pensionCheck.PatientId == patient.PatientId))
            {
                patientsContext.Patients.Add(patient);
                patientsContext.SaveChanges();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Метод, возвращающий пациента по id.
        /// </summary>
        /// <param name="id">Уникальный id пациента.</param>
        /// <returns>
        /// Возвращает сущность найденного пациента.
        /// NULL, если пациент не найден.
        /// </returns>
        public Patient GetPatient(int id)
        {
            return patientsContext.Patients
                .Include(x => x.Consultations)
                .Where(x => x.PatientId == id)
                .FirstOrDefault();
        }

        /// <summary>
        /// Метод, возвращающий IQueryable список пациентов с консультациями по заданному условию.
        /// </summary>
        /// <param name="expression">Условие, по которому требуется искать пациентов.</param>
        /// <returns>
        /// IQueryable список пациентов с консультациями удовлетворяющих условию.
        /// </returns>
        public IQueryable<Patient> GetPatientsWhere(Expression<Func<Patient, bool>> expression)
        {
            return patientsContext.Patients
                .Include(x => x.Consultations)
                .Where(expression);
        }

        /// <summary>
        /// Метод, возвращающий IQueryable список всех пациентов и их консультации.
        /// </summary>
        /// <returns>
        /// IQueryable список всех пациентов и их консультации.
        /// </returns>
        public IQueryable<Patient> GetPatients()
        {
            return patientsContext.Patients.Include(x => x.Consultations);
        }

        /// <summary>
        /// Метод изменяющий данные пациента в БД.
        /// </summary>
        /// <param name="patient">Обновленные данные пациента.</param>
        /// <returns>
        /// TRUE, если пациент с таким же id найден в базе данных и изменен.
        /// FALSE, если пациент с таким же id не найден или найден пациент с таким же СНИЛС.
        /// </returns>
        public bool UpdatePatient(Patient patient)
        {
            patient.PensionNumber = Regex.Replace(patient.PensionNumber, "[^0-9]", "");

            var prevPatient = patientsContext.Patients.Find(patient.PatientId);

            var pensionCheck = patientsContext.Patients
                        .Where(x => x.PensionNumber == patient.PensionNumber)
                        .FirstOrDefault();

            if ((pensionCheck == null) || (pensionCheck.PatientId == patient.PatientId))
            {
                patientsContext.Entry(prevPatient).CurrentValues.SetValues(patient);
                patientsContext.SaveChanges();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Метод, удаляющий пациента из БД.
        /// </summary>
        /// <param name="id">Уникальный id пациента.</param>
        public void DeletePatient(int id)
        {
            var patient = patientsContext.Patients.Find(id);
            if (patient != null)
            {
                patientsContext.Patients.Remove(patient);
                patientsContext.SaveChanges();
            }
            else
                throw new ArgumentNullException();
        }


        /// <summary>
        /// Метод, добавляющий консультацию в БД.
        /// </summary>
        /// <param name="consultation">Данные консультации.</param>
        /// <returns>
        /// TRUE, если консультация успешно добавлена пациенту.
        /// FALSE, если не найден пациент, которому должна быть добавлена консультация.
        /// </returns>
        public bool AddConsultation(Consultation consultation)
        {
            var patient = patientsContext.Patients.Find(consultation.PatientId);

            if (patient != null)
            {
                patientsContext.Consultations.Add(consultation);
                patientsContext.SaveChanges();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Метод, возвращающий консультацию по условию. 
        /// </summary>
        /// <param name="expression">Условие, по которому требуется искать консультацию.</param>
        /// <returns>
        /// Возвращает сущность консультации.
        /// NULL, если консультация не найдена.
        /// </returns>
        public Consultation GetConsultationWhere(Expression<Func<Consultation, bool>> expression)
        {
            return patientsContext.Consultations.Where(expression).FirstOrDefault();
        }

        /// <summary>
        /// Метод, возвращающий консультацию по id.
        /// </summary>
        /// <param name="id">Уникальный id консультации.</param>
        /// <returns>
        /// Возвращает сущность консультации.
        /// NULL, если консультация не найдена.
        /// </returns>
        public Consultation GetConsultation(int id)
        {
            return patientsContext.Consultations.Find(id);
        }

        /// <summary>
        /// Метод, возвращающий IQueryable список консультаций по id пациента.
        /// </summary>
        /// <param name="patientId">Уникальный id пациента.</param>
        /// <returns>
        /// IQueryable список консультаций.
        /// </returns>
        public IQueryable<Consultation> GetConsultations(int patientId)
        {
            return patientsContext.Consultations.Where(x => x.PatientId == patientId);
        }

        /// <summary>
        /// Метод, изменяющий данные консультации в БД.
        /// </summary>
        /// <param name="consultation">Обновленные данные консультации.</param>
        /// <returns>
        /// TRUE, если данные консультации успешно изменены.
        /// FALSE, если не найден пациент, у которого изменяется консультация.
        /// FALSE, если не найдена изменяемая консультация.
        /// </returns>
        public bool UpdateConsultation(Consultation consultation)
        {
            var patient = patientsContext.Patients.Find(consultation.PatientId);

            if (patient != null)
            {
                var prevCons = patientsContext.Consultations.Find(consultation.ConsultationId);

                if (prevCons != null)
                {
                    patientsContext.Entry(prevCons).CurrentValues.SetValues(consultation);
                    patientsContext.SaveChanges();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Метод, удаляющий консультацию из БД.
        /// </summary>
        /// <param name="id">Уникальный id консультации.</param>
        public void DeleteConsultation(int id)
        {
            var consultation = patientsContext.Consultations.Find(id);
            if (consultation != null)
            {
                patientsContext.Consultations.Remove(consultation);
                patientsContext.SaveChanges();
            }
            else
                throw new ArgumentNullException();
        }

    }
}
