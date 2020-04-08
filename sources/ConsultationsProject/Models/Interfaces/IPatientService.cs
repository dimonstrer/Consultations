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
        public Patient GetPatientWithConsultations(int id);
        public bool AddPatient(Patient patient);
        public bool UpdatePatient(Patient patient);
        public void DeletePatient(int id);

        public IQueryable<Consultation> GetConsultations(int patientId);
        public Consultation GetConsultation(int id);
        public Consultation GetConsultationWhere(Expression<Func<Consultation, bool>> expression);
        public bool AddConsultation(Consultation consultation);
        public bool UpdateConsultation(Consultation consultation);
        public void DeleteConsultation(int id);
    }
}
