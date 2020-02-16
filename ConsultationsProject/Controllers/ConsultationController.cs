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
        public IActionResult Add(int id)
        {
            using (PatientsContext db = new PatientsContext())
            {
                var patient = db.Patients
                    .FromSqlRaw($"SELECT * FROM PATIENTS WHERE PatientId = {id}")
                    .FirstOrDefault();
                if (patient != null)
                {
                    ViewBag.PatientId = id;
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
                    FromSqlRaw($"SELECT * FROM PATIENTS WHERE PatientId = { consultation.PatientId}")
                    .FirstOrDefault();
                if (patient != null)
                {
                    db.Consultations.Add(consultation);
                    db.SaveChanges();
                    return RedirectToAction("Get", "Patient", new { id = consultation.PatientId });
                }
                else
                    return NotFound();
            }
        }
    }
}