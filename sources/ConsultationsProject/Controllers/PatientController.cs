using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ConsultationsProject.Models;
using ConsultationsProject.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ConsultationsProject.Controllers
{
    /// <summary>
    /// Контроллер, ответственный за обработку запросов с пациентами.
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("patient-management/patients")]
    public class PatientController : Controller
    {
        /// <summary>
        /// Логгер.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Конфигурация.
        /// </summary>
        private readonly IConfiguration config;

        /// <summary>
        /// Количество консультаций на страницу у пациента.
        /// </summary>
        private readonly int ConsultationsPageSize;

        /// <summary>
        /// Количество пациентов на страницу.
        /// </summary>
        private readonly int PatientsPageSize;

        /// <summary>
        /// Сервис пациентов и консультаций.
        /// </summary>
        private readonly IPatientService patientService;

        /// <summary>
        /// Конструктор контроллера.
        /// </summary>
        /// <param name="logger">Логгер.</param>
        /// <param name="config">Конфигурация.</param>
        /// <param name="patientService">Сервис, ответственный за бизнес логику в работе с пациентами и консультациями.</param>
        public PatientController(ILogger<PatientController> logger, IConfiguration config, IPatientService patientService)
        {
            this.logger = logger;
            this.config = config;
            this.patientService = patientService;
            PatientsPageSize = config.GetValue<int>("PaginationSettings:PatientsPageSize");
            ConsultationsPageSize = config.GetValue<int>("PaginationSettings:ConsultationsPageSize");
        }

        /// <summary>
        /// Метод, возвращающий представление для добавления нового пациента.
        /// </summary>
        /// <returns>
        /// Представление для добавления нового пациента.
        /// </returns>
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        /// <summary>
        /// Метод, ответственный за добавление нового пациента в БД.
        /// </summary>
        /// <param name="patient">Данные пациента.</param>
        /// <returns>
        /// Страницу с ошибкой, если не было данных о пациенте в запросе.
        /// Представление со страницей добавления пациента с введенными ранее данными, если дата рождения не пройдет валидацию.
        /// Представление главной страницы с сообщением об успешном добавлении пациента.
        /// </returns>
        [HttpPost]
        public IActionResult Add(Patient patient)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (patient == null)
                    {
                        logger.LogError("При добавлении пациента произошла ошибка связывания модели.");
                        return View("Error",
                            new ErrorViewModel { Message = "При добавлении пациента произошла ошибка связывания модели" });
                    }

                    if (patient.BirthDate < DateTime.Parse("01/01/1880") ||
                        patient.BirthDate > DateTime.Now.AddYears(1))
                    {
                        logger.LogError($"При добавлении нового пациента произошла ошибка: " +
                            $"Недопустимая дата: {patient.BirthDate}");
                        ModelState.AddModelError("BirthDate", $"Дата рождения должна быть в промежутке" +
                            $" от {DateTime.Parse("01/01/1880").ToString("d")} до {DateTime.Now.AddYears(1).ToString("d")}");
                        return View(patient);
                    }
                    
                    if (patientService.AddPatient(patient) == true)
                    {
                        logger.LogInformation($"Добавлен новый пациент в базу данных. СНИЛС: {patient.PensionNumber}.");
                        return RedirectToAction("Index", "Home", new { message = "Пациент успешно добавлен" });
                    }
                    else
                    {
                        logger.LogError($"При добавлении нового пациента " +
                            $"обнаружен пациент с идентичным СНИЛС = {patient.PensionNumber}.");
                        ModelState.AddModelError("PensionNumber", "Пациент с таким СНИЛС уже существует");
                    }
                }
                return View(patient);
            }
            catch (Exception e)
            {
                logger.LogCritical($"При добавлении пациента произошла ошибка. ", e);
                return View("Error",
                            new ErrorViewModel
                            {
                                Message = "Произошла ошибка. Не удалось добавить пациента в базу данных." +
                                " Обратитесь к администратору."
                            });
            }
        }

        /// <summary>
        /// Метод, возвращающий представление с информацией о пациенте.
        /// </summary>
        /// <param name="id">Уникальный id пациента.</param>
        /// <param name="message">Сообщение об успешном добавлении/редактировании/удалении консультации пациента.</param>
        /// <param name="page">Текущая страница списка консультаций.</param>
        /// <returns>
        /// Страницу с ошибкой, если пациент не найден в БД.
        /// Представление с информацией о пациенте.
        /// </returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id, int page = 1, string message = "")
        {
            try
            {

                ViewBag.Message = message;

                var patient = patientService.GetPatient(id);
                if (patient != null)
                {
                    logger.LogInformation($"Запрос {HttpContext.Request.Query} вернул пациента с id {id}");

                    var count = patientService.GetConsultations(id).Count();
                    var pageViewModel = new PageViewModel(count, page, ConsultationsPageSize);

                    if (page <= 0 || page > pageViewModel.TotalPages)
                        page = 1;

                    patient.Consultations = patientService.GetConsultations(id)
                        .Where(x => x.PatientId == id)
                        .Skip((page - 1) * ConsultationsPageSize)
                        .Take(ConsultationsPageSize)
                        .ToList();

                    var result = new PatientViewModel { PageViewModel = pageViewModel, Patient = patient };
                    return View(result);
                }
                else
                {
                    logger.LogError($"При получении страницы пациента произошла ошибка:" +
                        $" не найден пациент с id = {id}. Запрос: {HttpContext.Request.Query}.");
                    return View("Error",
                        new ErrorViewModel
                        {
                            Message = $"При получении страницы пациента произошла ошибка:" +
                        $" не найден пациент с id = {id}"
                        });
                }
            }
            catch (Exception e)
            {
                logger.LogCritical($"Произошла ошибка при получении из базы данных пациента с id  = {id}", e);
                return View("Error",
                            new ErrorViewModel
                            {
                                Message = $"Произошла ошибка. Не удалось получить пациента с id  = {id}." +
                                " Обратитесь к администратору."
                            });
            }
        }

        /// <summary>
        /// Метод, использующийся для поиска пациентов в БД по имени или СНИЛС.
        /// </summary>
        /// <param name="name">ФИО пациента.</param>
        /// <param name="pension">СНИЛС пациента.</param>
        /// <param name="page">Текущая страница списка пациентов.</param>
        /// <returns>
        /// Частичное представление со списком пациентов, которые удовлетворяют заданным поисковым критериям.
        /// </returns>
        [HttpGet("search")]
        public IActionResult List(string name, string pension, int page = 1)
        {
            try
            {
                var patients = patientService.GetPatients().AsEnumerable();
                if (!String.IsNullOrEmpty(name))
                {
                    patients = patients.Where(x => EF.Functions.Like
                    (String.Concat(x.FirstName, " ", x.LastName, " ", x.Patronymic), "%" + name + "%"));
                }
                if (!String.IsNullOrEmpty(pension))
                {
                    patients = patients.Where(x => EF.Functions.Like(x.PensionNumber, pension + "%"));
                }

                var count = patients.Count();
                var pageViewModel = new PageViewModel(count, page, PatientsPageSize);

                if (page <= 0 || page > pageViewModel.TotalPages)
                    page = 1;

                var _patients = patients
                    .Skip((page - 1) * ConsultationsPageSize)
                    .Take(ConsultationsPageSize)
                    .ToList();

                var result = new IndexViewModel { PageViewModel = pageViewModel, Patients = _patients };

                logger.LogInformation($"Поисковой запрос {HttpContext.Request.Query} вернул {count} кол-во пациентов.");
                return PartialView(result);
            }
            catch (Exception e)
            {
                logger.LogCritical($"Произошла ошибка при поиске пациентов в базе данных." +
                    $" ФИО: {name}, СНИЛС: {pension}, страница: {page}", e);
                return Json(new
                {
                    success = "false",
                    message = $"Произошла ошибка. Не удалось найти пациентов. " +
                                "Обратитесь к администратору."
                });
            }
        }

        /// <summary>
        /// Метод, возвращающий представление с данными пациента для редактирования.
        /// </summary>
        /// <param name="id">Уникальный id пациента.</param>
        /// <returns>
        /// Представление с информацией о пациенте для редактирования.
        /// </returns>
        [HttpGet("{id}")]
        public IActionResult Edit(int id)
        {
            try
            {
                var patient = patientService.GetPatient(id);
                if (patient != null)
                {
                    logger.LogInformation($"Пациент с id = {id} был изменен.");
                    return View(patient);
                }
                logger.LogError($"При попытке изменения пациент с id = {id} был не найден в базе данных");
                return View("Error",
                    new ErrorViewModel { Message = $"При попытке изменения пациент с id = {id} был не найден в базе данных" });
            }
            catch (Exception e)
            {
                logger.LogCritical($"Произошла ошибка при получении из базы данных пациента с id  = {id}", e);
                return View("Error",
                            new ErrorViewModel
                            {
                                Message = $"Произошла ошибка. Не удалось получить пациента с id  = {id}." +
                                " Обратитесь к администратору."
                            });
            }
        }

        /// <summary>
        /// Метод, ответственный за редактирование данных пациента в БД.
        /// </summary>
        /// <param name="id">Уникальный id пациента.</param>
        /// <param name="patient">Измененные данные пациента.</param>
        /// <returns>
        /// Страницу с ошибкой, если не было данных о пациенте в запросе.
        /// Страницу с ошибкой, если пациент был удален из БД.
        /// Представление со страницей добавления пациента с введенными ранее данными, если дата рождения не пройдет валидацию.
        /// Представление со страницей добавления пациента с введенными ранее данными, если СНИЛС не является уникальным.
        /// Представление с информацией о пациенте с сообщением об успешном изменении.
        /// </returns>
        [HttpPost("{id}")]
        public IActionResult Edit(int id, Patient patient)
        {
            try
            {
                if (patient == null)
                {
                    logger.LogError($"При изменении пациента с id = {id} произошла ошибка связывания модели");
                    return View("Error",
                        new ErrorViewModel { Message = $"При изменении пациента с id = {id} произошла ошибка связывания модели" });
                }

                if (patient.BirthDate < DateTime.Parse("01/01/1880") ||
                        patient.BirthDate > DateTime.Now.AddYears(1))
                {
                    logger.LogError($"При изменения пациента с id = {id} произошла ошибка: " +
                        $"Недопустимая дата: {patient.BirthDate}");
                    ModelState.AddModelError("BirthDate", $"Дата рождения должна быть в промежутке" +
                        $" от {DateTime.Parse("01/01/1880").ToString("d")} до {DateTime.Now.AddYears(1).ToString("d")}");
                    return View(patient);
                }

                var _patient = patientService.GetPatient(id);
                if (_patient != null)
                {
                    if (patientService.UpdatePatient(patient) == true)
                    {
                        logger.LogInformation($"Пациент с id = {id} был изменен");
                        return RedirectToAction("Get", "Patient",
                            new { id = patient.PatientId, message = "Пациент успешно изменен" });
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
            catch (Exception e)
            {
                logger.LogCritical($"Произошла ошибка при обновлении данных пациента с id  = {id}", e);
                return View("Error",
                            new ErrorViewModel
                            {
                                Message = $"Произошла ошибка. Не удалось обновить данные пациента с id  = {id}." +
                                " Обратитесь к администратору."
                            });
            }
        }

        /// <summary>
        /// Метод, ответственный за удаление пациента из БД.
        /// </summary>
        /// <param name="id">Уникальный id пациента.</param>
        /// <returns>
        /// JSON со статусом false и сообщением об ошибке, если пациент не найден в БД.
        /// JSON со статусом true и сообщением об успешном удалении.
        /// </returns>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var patient = patientService.GetPatient(id);
                if (patient != null)
                {
                    logger.LogInformation($"Пациент с id = {id} был удален из базы данных");
                    patientService.DeletePatient(id);
                    return Json(new { success = "true", message = "Пациент успешно удален" });
                }
                logger.LogError($"При попытке удаления пациент с id = {id} был не найден в базе данных");
                return Json(new { success = "false", message = $"При попытке удаления пациент с id = {id} был не найден в базе данных" });
            }
            catch (Exception e)
            {
                logger.LogCritical($"Произошла ошибка при удалении из базы данных пациента с id  = {id}", e);
                return Json(new
                {
                    success = "false",
                    message = $"Произошла ошибка. Не удалось удалить пациента с id  = {id}. Обратитесь к администратору."
                });
            }
        }
    }
}