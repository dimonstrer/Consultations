using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ConsultationsProject.Models.Interfaces
{
    public interface IPatientService
    {
        public IQueryable<Patient> GetPatients();
        public IQueryable<Patient> GetPatientsWhere(Expression<Func<Patient, bool>> expression);
        public Patient GetPatient(int id);
        public void AddPatient(Patient patient);
        public void UpdatePatient(Patient patient);
        public void DeletePatient(int id);

        public IQueryable<Consultation> GetConsultations(int patientId);
        public Consultation GetConsultation(int id);
        public Consultation GetConsultationWhere(Expression<Func<Consultation, bool>> expression);
        public void AddConsultation(Consultation consultation);
        public void UpdateConsultation(Consultation consultation);
        public void DeleteConsultation(int id);
    }
}
