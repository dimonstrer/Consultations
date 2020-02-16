﻿using System;
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
                using(PatientsContext db = new PatientsContext())
                {
                    patient.PensionNumber = Regex.Replace(patient.PensionNumber, "[^0-9]", "");
                    var result = db.Patients.FromSqlInterpolated
                        ($"SELECT TOP 1 * FROM PATIENTS WHERE PensionNumber = {patient.PensionNumber}");
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
            using(PatientsContext db = new PatientsContext())
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
    }
}