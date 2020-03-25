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
    /// <summary>
    /// Контроллер, ответственный за обработку запросов с консультациями пациентов.
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("consultation-management")]
    public class ConsultationController : Controller
    {
        /// <summary>
        /// Логгер
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Контекст БД с пациентами и консультациями.
        /// </summary>
        private readonly PatientsContext patientContext;

        /// <summary>
        /// Конструктор контроллера.
        /// </summary>
        /// <param name="logger">Логгер.</param>
        /// <param name="patientContext">Контекст БД с пациентами и консультациями.</param>
        public ConsultationController(ILogger<ConsultationController> logger, PatientsContext patientContext)
        {
            this.logger = logger;
            this.patientContext = patientContext;
        }

        /// <summary>
        /// Метод, возвращающий представление для добавления новой консультации пациенту.
        /// </summary>
        /// <param name="patientId">Уникальный id пациента.</param>
        /// <returns>
        /// Страницу с ошибкой, если пациент не найден в БД.
        /// Представление с добавлением новой консультации.
        /// </returns>
        [HttpGet("patient/{patient-id}/consultations")]
        public IActionResult Add([FromRoute(Name = "patient-id")]int patientId)
        {
            try
            {
                var patient = patientContext.Patients
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
                    new ErrorViewModel
                    {
                        Message = $"При добавлении консультации произошла ошибка: " +
                    $"Пациент с id = {patientId} не найден в базе данных."
                    });
            }
            catch(Exception e)
            {
                logger.LogCritical($"Произошла ошибка при получении пациента с id  = {patientId} и его консультаций.", e);
                return View("Error",
                            new ErrorViewModel
                            {
                                Message = $"Произошла ошибка. Не удалось получить данные пациента с id  = {patientId}" +
                                $" и его консультации. Обратитесь к администратору."
                            });
            }
        }

        /// <summary>
        /// Метод, ответственный за добавление новой консультации пациента в БД.
        /// </summary>
        /// <param name="consultation">Данные новой консультации пациента.</param>
        /// <returns>
        /// Страницу с ошибкой, если не было данных о консультации пациента в запросе.
        /// Страницу с ошибкой, если пациент не найден в БД.
        /// Представление со страницей добавления консультации пациента с введенными ранее данными, если день консультации не пройдет валидацию.
        /// Представление с информацией о пациенте с сообщением об успешном добавлении консультации.
        /// </returns>
        [HttpPost("consultations")]
        public IActionResult Add(Consultation consultation)
        {
            try
            {
                ViewBag.PatientId = consultation.PatientId;
                if (consultation == null)
                {
                    logger.LogError($"При добавлении консультации пациенту с id {consultation.PatientId} произошла ошибка связывания модели.");
                    return View("Error",
                        new ErrorViewModel
                        {
                            Message = $"При добавлении консультации пациенту с id {consultation.PatientId}" +
                        $" произошла ошибка связывания модели."
                        });
                }
                var patient = patientContext.Patients.
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

                    patientContext.Consultations.Add(consultation);
                    patientContext.SaveChanges();
                    logger.LogInformation($"Пациенту с id = {consultation.PatientId} была добавлена новая консультация.");
                    return RedirectToAction("Get", "Patient",
                        new { id = consultation.PatientId, message = "Консультация успешно добавлена" });
                }
                else
                {
                    logger.LogError($"При добавлении консультации произошла ошибка: " +
                       $"Пациент с id = {consultation.PatientId} не найден в базе данных");
                    return View("Error",
                        new ErrorViewModel
                        {
                            Message = $"При добавлении консультации произошла ошибка: " +
                       $"Пациент с id = {consultation.PatientId} не найден в базе данных"
                        });
                }
            }
            catch(Exception e)
            {
                logger.LogCritical($"При добавлении консультации пациенту с id = {consultation.PatientId} произошла ошибка. ", e);
                return View("Error",
                            new ErrorViewModel
                            {
                                Message = $"Произошла ошибка. Не удалось добавить консультацию пациенту" +
                                $" с id = {consultation.PatientId} в базу данных. Обратитесь к администратору."
                            });
            }
        }

        /// <summary>
        /// Метод, возвращающий представление для редактирования данных консультации пациента.
        /// </summary>
        /// <param name="id">Уникальный id консультации пациента.</param>
        /// <returns>
        /// Страницу с ошибкой, если консультация не найдена в БД.
        /// Представление с информацией о консультации пациента для редактирования.
        /// </returns>
        [HttpGet("consultations/{id}")]
        public IActionResult Edit(int id)
        {
            try
            {
                var consultation = patientContext.Consultations.FromSqlRaw($"SELECT * FROM CONSULTATIONS WHERE ConsultationId = {id}");
                if (consultation.Count() != 0)
                {
                    return View(consultation.FirstOrDefault());
                }
                logger.LogError($"При изменении консультации произошла ошибка: " +
                    $"Консультация с id = {id} не найдена в базе данных");
                return View("Error",
                        new ErrorViewModel
                        {
                            Message = $"При изменении консультации произошла ошибка: " +
                    $"Консультация с id = {id} не найдена в базе данных"
                        });
            }
            catch(Exception e)
            {
                logger.LogCritical($"Произошла ошибка при получении из базы данных консультации с id  = {id}", e);
                return View("Error",
                            new ErrorViewModel
                            {
                                Message = $"Произошла ошибка. Не удалось получить консультацию с id  = {id}." +
                                " Обратитесь к администратору."
                            });
            }
        }

        /// <summary>
        /// Метод, ответственный за редактирование данных консультации пациента в БД.
        /// </summary>
        /// <param name="id">Уникальный id консультации пациента.</param>
        /// <param name="consultation">Измененные данные консультации пациента.</param>
        /// <returns>
        /// Страницу с ошибкой, если не было данных о консультации пациента в запросе.
        /// Страницу с ошибкой, если пациент не найден в БД.
        /// Представление со страницей изменения консультации пациента с введенными ранее данными, если день консультации не пройдет валидацию.
        /// Представление с информацией о пациенте с сообщением об успешном редактировании консультации.
        /// </returns>
        [HttpPost("consultations/{id}")]
        public IActionResult Edit(int id, Consultation consultation)
        {
            try
            {
                if (consultation == null)
                {
                    logger.LogError($"При изменении консультации с id {id} произошла ошибка связывания модели.");
                    return View("Error",
                        new ErrorViewModel { Message = $"При изменении консультации с id {id} произошла ошибка связывания модели." });
                }
                var consultationId = consultation.ConsultationId;
                var _consultation = patientContext.Consultations.FromSqlRaw
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

                    patientContext.Entry(_consultation.FirstOrDefault()).CurrentValues.SetValues(consultation);
                    patientContext.SaveChanges();
                    logger.LogInformation($"У пациента с id = {consultation.PatientId}" +
                        $" была изменена консультация с id = {id}");
                    return RedirectToAction("Get", "Patient",
                        new { id = consultation.PatientId, message = "Консультация успешно изменена" });
                }
                logger.LogError($"При изменении консультации произошла ошибка: " +
                    $"Консультация с id = {id} не найдена в базе данных");
                return View("Error",
                        new ErrorViewModel
                        {
                            Message = $"При изменении консультации произошла ошибка: " +
                    $"Консультация с id = {id} не найдена в базе данных"
                        });
            }
            catch (Exception e)
            {
                logger.LogCritical($"Произошла ошибка при обновлении данных консультации с id = {id}" +
                    $" пациента с id  = {consultation.PatientId}", e);
                return View("Error",
                            new ErrorViewModel
                            {
                                Message = $"Произошла ошибка. Не удалось обновить данные консультации с id = {id}" +
                                $" пациента с id  = {consultation.PatientId}. Обратитесь к администратору."
                            });
            }
        }

        /// <summary>
        /// Метод, ответственный за удаление консультации пациента из БД.
        /// </summary>
        /// <param name="id">Уникальный id консультации пациента.</param>
        /// <returns>
        /// JSON со статусом false и сообщением об ошибке, если консультация не найдена в БД.
        /// JSON со статусом true и сообщением об успешном удалении.
        /// </returns>
        [HttpDelete("consultations/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var consultation = patientContext.Consultations.FromSqlRaw
                    ($"SELECT * FROM CONSULTATIONS WHERE ConsultationId = {id}");
                if (consultation.Count() != 0)
                {
                    var patientId = consultation.FirstOrDefault().PatientId;
                    patientContext.Consultations.Remove(consultation.FirstOrDefault());
                    patientContext.SaveChanges();
                    logger.LogInformation($"У пациента с id = {patientId} была удалена консультация с id = {id}");
                    return Json(new { success = "true", message = "Консультация успешно удалена" });
                }
                logger.LogError($"При удалении консультации произошла ошибка: " +
                    $"Консультация с id = {id} не найдена в базе данных");
                return Json(new
                {
                    code = "false",
                    message = $"При удалении консультации произошла ошибка: " +
                    $"Консультация с id = {id} не найдена в базе данных"
                });
            }
            catch(Exception e)
            {
                logger.LogCritical($"Произошла ошибка при удалении из базы данных консультации с id  = {id}", e);
                return Json(new
                {
                    success = "false",
                    message = $"Произошла ошибка. Не удалось удалить консультацию с id  = {id}. Обратитесь к администратору."
                });
            }
        }
    }
}
