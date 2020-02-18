using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ConsultationsProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConsultationsProject.Controllers
{
    public class PatientController : Controller
    {
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(Patient patient)
        {
            if (ModelState.IsValid)
            {
                using (PatientsContext db = new PatientsContext())
                {
                    patient.PensionNumber = Regex.Replace(patient.PensionNumber, "[^0-9]", "");
                    var result = db.Patients.FromSqlInterpolated
                        ($"SELECT TOP 1 * FROM PATIENTS WHERE PensionNumber = {patient.PensionNumber}").FirstOrDefault();
                    if (result == null)
                    {
                        db.Patients.Add(patient);
                        db.SaveChanges();
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("PensionNumber", "Пациент с таким СНИЛС уже существует");
                    }
                }
            }
            return View(patient);
        }
        public IActionResult Get(int id)
        {
            using (PatientsContext db = new PatientsContext())
            {
                var patient = db.Patients
                    .Include(x => x.Consultations)
                    .Where(x => x.PatientId == id)
                    .FirstOrDefault();
                if (patient != null)
                    return View(patient);
                else
                    return NotFound();
            }
        }

        public IActionResult List(string name, string pension)
        {
            using (PatientsContext db = new PatientsContext())
            {
                var patients = db.Patients.AsEnumerable();
                if (!String.IsNullOrEmpty(name))
                {
                    patients = patients.Where(x => EF.Functions.Like
                    (String.Concat(x.FirstName, " ", x.LastName, " ", x.Patronymic), "%" + name + "%"));
                }
                if (!String.IsNullOrEmpty(pension))
                {
                    patients = patients.Where(x => EF.Functions.Like(x.PensionNumber, pension + "%"));
                }
                var result = patients.ToList();
                return PartialView(result);
            }
        }

        public IActionResult Edit(int id)
        {
            using(PatientsContext db = new PatientsContext())
            {
                var patient = db.Patients.Find(id);
                if (patient != null)
                {
                    return View(patient);
                }
                return NotFound();
            }
        }

        [HttpPost]
        public IActionResult Edit(Patient patient)
        {
            using(PatientsContext db = new PatientsContext())
            {
                var _patient = db.Patients.Find(patient.PatientId);
                if (_patient != null)
                {
                    db.Patients.Update(patient);
                    db.SaveChanges();
                    return RedirectToAction("Get", "Patient", new { id = patient.PatientId });
                }
                return NotFound();
            }
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            using(PatientsContext db = new PatientsContext())
            {
                var patient = db.Patients.Find(id);
                if (patient != null)
                {
                    db.Remove(patient);
                    db.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
                return NotFound();
            }
        }
    }
}