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
    public class PatientService : IPatientService
    {
        private readonly PatientsContext patientsContext;

        public PatientService(PatientsContext patientsContext)
        {
            this.patientsContext = patientsContext;
        }

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

        public Patient GetPatient(int id)
        {
            return patientsContext.Patients
                .Include(x => x.Consultations)
                .Where(x => x.PatientId == id)
                .FirstOrDefault();
        }

        public IQueryable<Patient> GetPatientsWhere(Expression<Func<Patient, bool>> expression)
        {
            return patientsContext.Patients
                .Include(x => x.Consultations)
                .Where(expression);
        }

        public IQueryable<Patient> GetPatients()
        {
            return patientsContext.Patients.Include(x => x.Consultations);
        }

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

        public Consultation GetConsultationWhere(Expression<Func<Consultation, bool>> expression)
        {
            return patientsContext.Consultations.Where(expression).FirstOrDefault();
        }

        public Consultation GetConsultation(int id)
        {
            return patientsContext.Consultations.Find(id);
        }

        public IQueryable<Consultation> GetConsultations(int patientId)
        {
            return patientsContext.Consultations.Where(x => x.PatientId == patientId);
        }

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
