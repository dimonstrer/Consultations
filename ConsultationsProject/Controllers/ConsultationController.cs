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
                if (patient.Count() != 0)
                {
                    logger.LogInformation($"Добавление консультации для пациента с id = {patientId}.");
                    ViewBag.PatientId = patientId;
                    return View();
                }
                logger.LogError($"При добавлении консультации произошла ошибка: " +
                    $"Пациент с id = {patientId} не найден в базе данных");
                return View("Error",
                    new ErrorViewModel { Message = $"При добавлении консультации произошла ошибка: " +
                    $"Пациент с id = {patientId} не найден в базе данных." });
            }
        }
        [HttpPost]
        public IActionResult Add(Consultation consultation)
        {
            using (PatientsContext db = new PatientsContext())
            {
                ViewBag.PatientId = consultation.PatientId;
                if (consultation == null)
                {
                    logger.LogError($"При добавлении консультации пациенту с id {consultation.PatientId} произошла ошибка связывания модели.");
                    return View("Error",
                        new ErrorViewModel { Message = $"При добавлении консультации пациенту с id {consultation.PatientId}" +
                        $" произошла ошибка связывания модели." });
                }
                var patient = db.Patients.
                    FromSqlRaw($"SELECT * FROM PATIENTS WHERE PatientId = { consultation.PatientId}");
                if (patient.Count() != 0)
                {
                    if (consultation.Day < DateTime.Parse("01/01/1880") ||
                        consultation.Day > DateTime.Now.AddYears(1))
                    {
                        logger.LogError($"При добавлении новой консультации пациенту с id = {consultation.PatientId} произошла ошибка: " +
                            $"Недопустимая дата: {consultation.Day}");
                        ModelState.AddModelError("Day", $"День консультации должен быть в промежутке" +
                            $" от {DateTime.Parse("01/01/1880").ToString("d")} до {DateTime.Now.AddYears(1).ToString("d")}");
                        return View(consultation);
                    }

                    db.Consultations.Add(consultation);
                    db.SaveChanges();
                    logger.LogInformation($"Пациенту с id = {consultation.PatientId} была добавлена новая консультация.");
                    return RedirectToAction("Get", "Patient", new { id = consultation.PatientId });
                }
                else
                {
                    logger.LogError($"При добавлении консультации произошла ошибка: " +
                       $"Пациент с id = {consultation.PatientId} не найден в базе данных");
                    return View("Error",
                        new ErrorViewModel { Message = $"При добавлении консультации произошла ошибка: " +
                       $"Пациент с id = {consultation.PatientId} не найден в базе данных"});
                }
            }
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            using (PatientsContext db = new PatientsContext())
            {
                var consultation = db.Consultations.FromSqlRaw($"SELECT * FROM CONSULTATIONS WHERE ConsultationId = {id}");
                if (consultation.Count() != 0)
                {
                    return View(consultation.FirstOrDefault());
                }
                logger.LogError($"При изменении консультации произошла ошибка: " +
                    $"Консультация с id = {id} не найдена в базе данных");
                return View("Error",
                        new ErrorViewModel { Message = $"При изменении консультации произошла ошибка: " +
                    $"Консультация с id = {id} не найдена в базе данных"});
            }
        }

        [HttpPost]
        public IActionResult Edit(int id, Consultation consultation)
        {
            using (PatientsContext db = new PatientsContext())
            {
                if (consultation == null)
                {
                    logger.LogError($"При изменении консультации с id {id} произошла ошибка связывания модели.");
                    return View("Error",
                        new ErrorViewModel { Message = $"При изменении консультации с id {id} произошла ошибка связывания модели." });
                }
                var consultationId = consultation.ConsultationId;
                var _consultation = db.Consultations.FromSqlRaw
                    ($"SELECT * FROM CONSULTATIONS WHERE ConsultationId = {id}");
                if (_consultation.Count() != 0)
                {
                    if (consultation.Day < DateTime.Parse("01/01/1880") ||
                        consultation.Day > DateTime.Now.AddYears(1))
                    {
                        logger.LogError($"При изменении пациенту с id = {consultation.PatientId}" +
                            $" консультации с id = {id} произошла ошибка: Недопустимая дата: {consultation.Day}");
                        ModelState.AddModelError("Day", $"День консультации должен быть в промежутке" +
                            $" от {DateTime.Parse("01/01/1880").ToString("d")} до {DateTime.Now.AddYears(1).ToString("d")}");
                        return View(consultation);
                    }

                    db.Entry(_consultation.FirstOrDefault()).CurrentValues.SetValues(consultation);
                    db.SaveChanges();
                    logger.LogInformation($"У пациента с id = {consultation.PatientId}" +
                        $" была изменена консультация с id = {id}");
                    return RedirectToAction("Get", "Patient", new { id = consultation.PatientId });
                }
                logger.LogError($"При изменении консультации произошла ошибка: " +
                    $"Консультация с id = {id} не найдена в базе данных");
                return View("Error",
                        new ErrorViewModel { Message = $"При изменении консультации произошла ошибка: " +
                    $"Консультация с id = {id} не найдена в базе данных"});
            }
        }


        [HttpDelete]
        public IActionResult Delete(int id)
        {
            using (PatientsContext db = new PatientsContext())
            {
                var consultation = db.Consultations.FromSqlRaw
                    ($"SELECT * FROM CONSULTATIONS WHERE ConsultationId = {id}");
                if (consultation.Count() != 0)
                {
                    var patientId = consultation.FirstOrDefault().PatientId;
                    db.Consultations.Remove(consultation.FirstOrDefault());
                    db.SaveChanges();
                    logger.LogInformation($"У пациента с id = {patientId} была удалена консультация с id = {id}");
                    return Json(new { success = "true" });
                }
                logger.LogError($"При удалении консультации произошла ошибка: " +
                    $"Консультация с id = {id} не найдена в базе данных");
                return Json(new { code = "false", message = $"При удалении консультации произошла ошибка: " +
                    $"Консультация с id = {id} не найдена в базе данных"});
            }
        }
    }
}
