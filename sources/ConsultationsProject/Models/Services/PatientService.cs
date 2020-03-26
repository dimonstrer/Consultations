using ConsultationsProject.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public void AddPatient(Patient patient)
        {
            patientsContext.Patients.Add(patient);
            patientsContext.SaveChanges();
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

        public void UpdatePatient(Patient patient)
        {
            patientsContext.Update(patient);
            patientsContext.SaveChanges();
        }

        public void DeletePatient(int id)
        {
            var patient = patientsContext.Patients.Find(id);
            if (patient != null)
                patientsContext.Patients.Remove(patient);
            else
                throw new ArgumentNullException();
        }



        public void AddConsultation(Consultation consultation)
        {
            patientsContext.Consultations.Add(consultation);
            patientsContext.SaveChanges();
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

        public void UpdateConsultation(Consultation consultation)
        {
            patientsContext.Update(consultation);
            patientsContext.SaveChanges();
        }

        public void DeleteConsultation(int id)
        {
            var consultation = patientsContext.Consultations.Find(id);
            if (consultation != null)
                patientsContext.Consultations.Remove(consultation);
            else
                throw new ArgumentNullException();
        }

    }
}
