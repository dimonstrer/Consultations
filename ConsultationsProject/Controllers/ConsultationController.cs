using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConsultationsProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConsultationsProject.Controllers
{
    public class ConsultationController : Controller
    {
        [HttpGet]
        public IActionResult Add(int patientId)
        {
            using (PatientsContext db = new PatientsContext())
            {
                var patient = db.Patients
                    .FromSqlRaw($"SELECT * FROM PATIENTS WHERE PatientId = {patientId}");
                if (patient.Count()==1)
                {
                    ViewBag.PatientId = patientId;
                    return View();
                }
                else
                    return NotFound();
            }
        }
        [HttpPost]
        public IActionResult Add(Consultation consultation)
        {
            using (PatientsContext db = new PatientsContext())
            {
                var patient = db.Patients.
                    FromSqlRaw($"SELECT * FROM PATIENTS WHERE PatientId = { consultation.PatientId}");
                if (patient.Count()==1)
                {
                    db.Consultations.Add(consultation);
                    db.SaveChanges();
                    return RedirectToAction("Get", "Patient", new { id = consultation.PatientId });
                }
                else
                    return NotFound();
            }
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            using (PatientsContext db = new PatientsContext())
            {
                var consultation = db.Consultations.FromSqlRaw($"SELECT * FROM CONSULTATIONS WHERE ConsultationId = {id}");
                if (consultation.Count()==1)
                {
                    return View(consultation.FirstOrDefault());
                }
                return NotFound();
            }
        }

        [HttpPost]
        public IActionResult Edit(Consultation consultation)
        {
            using (PatientsContext db = new PatientsContext())
            {
                var _consultation = db.Consultations.FromSqlRaw
                    ($"SELECT * FROM CONSULTATIONS WHERE ConsultationId = {consultation.ConsultationId}");
                if (_consultation.Count()==1)
                {
                    db.Entry(_consultation.FirstOrDefault()).CurrentValues.SetValues(consultation);
                    db.SaveChanges();
                    return RedirectToAction("Get", "Patient", new { id = consultation.PatientId });
                }
            }
            return NotFound();
        }


        [HttpDelete]
        public IActionResult Delete(int id)
        {
            using (PatientsContext db = new PatientsContext())
            {
                var consultation = db.Consultations.FromSqlRaw
                    ($"SELECT * FROM CONSULTATIONS WHERE ConsultationId = {id}");
                if (consultation.Count()==1)
                {
                    var patientId = consultation.FirstOrDefault().PatientId;
                    db.Consultations.Remove(consultation.FirstOrDefault());
                    db.SaveChanges();
                    return RedirectToAction("Get", "Patient", new { id = patientId });
                }
                return NotFound();
            }
        }
    }
}
