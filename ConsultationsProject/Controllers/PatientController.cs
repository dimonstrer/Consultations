using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ConsultationsProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ConsultationsProject.Controllers
{
    public class PatientController : Controller
    {
        private readonly ILogger logger;
        public PatientController(ILogger<PatientController> logger)
        {
            this.logger = logger;
        }
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
                    if (patient == null)
                    {
                        logger.LogError("При добавлении пациента произошла ошибка связывания модели.");
                        return View("Error",
                            new ErrorViewModel { Message = "При добавлении пациента произошла ошибка связывания модели" });
                    }

                    if(patient.BirthDate<DateTime.Parse("01/01/1880")||
                        patient.BirthDate > DateTime.Now.AddYears(1))
                    {
                        logger.LogError($"При добавлении нового пациента произошла ошибка: " +
                            $"Недопустимая дата: {patient.BirthDate}");
                        ModelState.AddModelError("BirthDate", $"Дата рождения должна быть в промежутке" +
                            $" от {DateTime.Parse("01/01/1880").ToString("d")} до {DateTime.Now.AddYears(1).ToString("d")}");
                        return View(patient);
                    }

                    patient.PensionNumber = Regex.Replace(patient.PensionNumber, "[^0-9]", "");
                    var result = db.Patients
                        .Where(x => x.PensionNumber == patient.PensionNumber)
                        .FirstOrDefault();
                    if (result == null)
                    {
                        db.Patients.Add(patient);
                        db.SaveChanges();
                        logger.LogInformation($"Добавлен новый пациент в базу данных. СНИЛС: {patient.PensionNumber}.");
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        logger.LogError($"При добавлении нового пациента " +
                            $"обнаружен пациент с идентичным СНИЛС = {patient.PensionNumber}.");
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
                {
                    logger.LogInformation($"Запрос {HttpContext.Request.Query} вернул пациента с id {id}");
                    return View(patient);
                }
                else
                {
                    logger.LogError($"При получении страницы пациента произошла ошибка:" +
                        $" не найден пациент с id = {id}. Запрос: {HttpContext.Request.Query}.");
                    return View("Error",
                        new ErrorViewModel { Message = $"При получении страницы пациента произошла ошибка:" +
                        $" не найден пациент с id = {id}"});
                }
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
                logger.LogInformation($"Поисковой запрос {HttpContext.Request.Query} вернул {result.Count} кол-во пациентов.");
                return PartialView(result);
            }
        }

        public IActionResult Edit(int id)
        {
            using (PatientsContext db = new PatientsContext())
            {
                var patient = db.Patients.Find(id);
                if (patient != null)
                {
                    logger.LogInformation($"Пациент с id = {id} был изменен.");
                    return View(patient);
                }
                logger.LogError($"При попытке изменения пациент с id = {id} был не найден в базе данных");
                return View("Error",
                    new ErrorViewModel { Message = $"При попытке изменения пациент с id = {id} был не найден в базе данных" });
            }
        }

        [HttpPost]
        public IActionResult Edit(int id, Patient patient)
        {
            using (PatientsContext db = new PatientsContext())
            {
                if (patient == null)
                {
                    logger.LogError($"При изменении пациента с id = {id} произошла ошибка связывания модели");
                    return View("Error",
                        new ErrorViewModel { Message = $"При изменении пациента с id = {id} произошла ошибка связывания модели"});
                }
                var _patient = db.Patients.Find(id);
                if (_patient != null)
                {
                    if (patient.BirthDate < DateTime.Parse("01/01/1880") ||
                        patient.BirthDate > DateTime.Now.AddYears(1))
                    {
                        logger.LogError($"При изменения пациента с id = {id} произошла ошибка: " +
                            $"Недопустимая дата: {patient.BirthDate}");
                        ModelState.AddModelError("BirthDate", $"Дата рождения должна быть в промежутке" +
                            $" от {DateTime.Parse("01/01/1880").ToString("d")} до {DateTime.Now.AddYears(1).ToString("d")}");
                        return View(patient);
                    }

                    patient.PensionNumber = Regex.Replace(patient.PensionNumber, "[^0-9]", "");
                    var pensionCheck = db.Patients
                        .Where(x => x.PensionNumber == patient.PensionNumber)
                        .FirstOrDefault();
                    if (pensionCheck == null||pensionCheck.PatientId==id)
                    {
                        db.Entry(_patient).CurrentValues.SetValues(patient);
                        db.SaveChanges();
                        logger.LogInformation($"Пациент с id = {id} был изменен");
                        return RedirectToAction("Get", "Patient", new { id = patient.PatientId });
                    }
                    else
                    {
                        logger.LogWarning($"При изменении пациента с id = {id} произошла ошибка: " +
                            $"Был обнаружен пациент с таким же СНИЛС = {patient.PensionNumber}");
                        ModelState.AddModelError("PensionNumber", "Пациент с таким СНИЛС уже существует");
                        return View(patient);
                    }
                }
                logger.LogError($"При попытке изменения пациент с id = {id} был не найден в базе данных");
                return View("Error",
                    new ErrorViewModel { Message = $"При попытке изменения пациент с id = {id} был не найден в базе данных" });
            }
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            using (PatientsContext db = new PatientsContext())
            {
                var patient = db.Patients.Find(id);
                if (patient != null)
                {
                    logger.LogInformation($"Пациент с id = {id} был удален из базы данных");
                    db.Remove(patient);
                    db.SaveChanges();
                    return Json(new { success = "true" });
                }
                logger.LogError($"При попытке удаления пациент с id = {id} был не найден в базе данных");
                return Json(new { success = "false", message = $"При попытке удаления пациент с id = {id} был не найден в базе данных" });
            }
        }
    }
}