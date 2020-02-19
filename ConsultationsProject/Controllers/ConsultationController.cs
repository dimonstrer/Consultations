using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConsultationsProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ConsultationsProject.Controllers
{
    public class ConsultationController : Controller
    {
        private readonly ILogger logger;
        public ConsultationController(ILogger<ConsultationController> logger)
        {
            this.logger = logger;
        }
        [HttpGet]
        public IActionResult Add(int patientId)
        {
            using (PatientsContext db = new PatientsContext())
            {
                var patient = db.Patients
                    .FromSqlRaw($"SELECT * FROM PATIENTS WHERE PatientId = {patientId}");
                if (patient.Count() == 1)
                {
                    logger.LogInformation("Добавление консультации для пациента с id = {0}.", patientId);
                    ViewBag.PatientId = patientId;
                    return View();
                }
                logger.LogError("При добавлении консультации произошла ошибка: " +
                    "Пациент с id = {0} не найден в базе данных", patientId);
                return View("Error",
                    new ErrorViewModel { Message = $"Пациент с id = {patientId} не найден в базе данных." });
            }
        }
        [HttpPost]
        public IActionResult Add(int patientId, Consultation consultation)
        {
            using (PatientsContext db = new PatientsContext())
            {
                if (consultation == null)
                {
                    logger.LogError("При добавлении консультации пациенту с id {0} произошла ошибка связывания модели.", patientId);
                    return View("Error",
                        new ErrorViewModel { Message = "Произошла ошибка связывания модели при добавлении новой консультации." });
                }
                var patient = db.Patients.
                    FromSqlRaw($"SELECT * FROM PATIENTS WHERE PatientId = { consultation.PatientId}");
                if (patient.Count() == 1)
                {
                    db.Consultations.Add(consultation);
                    db.SaveChanges();
                    logger.LogInformation("Пациенту с id = {0} была добавлена новая консультация.", patientId);
                    return RedirectToAction("Get", "Patient", new { id = consultation.PatientId });
                }
                else
                {
                    logger.LogError("При добавлении консультации произошла ошибка: " +
                       "Пациент с id = {0} не найден в базе данных", patientId);
                    return View("Error",
                        new ErrorViewModel { Message = $"Пациент с id = {consultation.PatientId} не найден в базе данных." });
                }
            }
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            using (PatientsContext db = new PatientsContext())
            {
                var consultation = db.Consultations.FromSqlRaw($"SELECT * FROM CONSULTATIONS WHERE ConsultationId = {id}");
                if (consultation.Count() == 1)
                {
                    return View(consultation.FirstOrDefault());
                }
                logger.LogError("При изменении консультации произошла ошибка: " +
                    "Консультация с id = {0} не найдена в базе данных", id);
                return View("Error",
                        new ErrorViewModel { Message = $"Консультация с id = {id} не найдена в базе данных." });
            }
        }

        [HttpPost]
        public IActionResult Edit(int id, Consultation consultation)
        {
            using (PatientsContext db = new PatientsContext())
            {
                if (consultation == null)
                {
                    logger.LogError("При изменении консультации с id {0} произошла ошибка связывания модели.", id);
                    return View("Error",
                        new ErrorViewModel { Message = $"Произошла ошибка связывания модели при изменении консультации с id = {id}." });
                }
                var consultationId = consultation.ConsultationId;
                var _consultation = db.Consultations.FromSqlRaw
                    ($"SELECT * FROM CONSULTATIONS WHERE ConsultationId = {id}");
                if (_consultation.Count() == 1)
                {
                    db.Entry(_consultation.FirstOrDefault()).CurrentValues.SetValues(consultation);
                    db.SaveChanges();
                    logger.LogInformation("У пациента с id = {0} была изменена консультация с id = {1}",
                        consultation.PatientId, id);
                    return RedirectToAction("Get", "Patient", new { id = consultation.PatientId });
                }
                logger.LogError("При изменении консультации произошла ошибка: " +
                    "Консультация с id = {0} не найдена в базе данных", id);
                return View("Error",
                        new ErrorViewModel { Message = $"Консультация с id = {consultationId} не найдена в базе данных." });
            }
        }


        [HttpDelete]
        public IActionResult Delete(int id)
        {
            using (PatientsContext db = new PatientsContext())
            {
                var consultation = db.Consultations.FromSqlRaw
                    ($"SELECT * FROM CONSULTATIONS WHERE ConsultationId = {id}");
                if (consultation.Count() == 1)
                {
                    var patientId = consultation.FirstOrDefault().PatientId;
                    db.Consultations.Remove(consultation.FirstOrDefault());
                    db.SaveChanges();
                    logger.LogInformation("У пациента с id = {0} была удалена консультация с id = {1}",
                        patientId, id);
                    return Json(new { code = "success" });
                }
                logger.LogError("При удалении консультации произошла ошибка: " +
                    "Консультация с id = {0} не найдена в базе данных", id);
                return Json(new { code = "fail", message = $"Ошибка! Консультация с id = {id} была не найдена в базе данных" });
            }
        }
    }
}
