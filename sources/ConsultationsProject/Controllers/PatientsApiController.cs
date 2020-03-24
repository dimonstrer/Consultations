using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConsultationsProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ConsultationsProject.Controllers
{
    [Route("api/patient-management/patients")]
    [ApiController]
    public class PatientsApiController : ControllerBase
    {
        /// <summary>
        /// Контекст БД с пациентами и консультациями.
        /// </summary>
        private readonly PatientsContext patientContext;

        /// <summary>
        /// Логгер
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Конструктор контроллера.
        /// </summary>
        /// <param name="patientContext">Контекст БД с пациентами и консультациями.</param>
        public PatientsApiController(PatientsContext patientContext,ILogger<PatientsApiController> logger)
        {
            this.patientContext = patientContext;
            this.logger = logger;
        }

        /// <summary>
        /// Метод, возвращающий всех пациентов из БД.
        /// </summary>
        /// <returns>HTTP ответ, содержащий статус код и пациентов.</returns>
        /// <response code="200">Возвращает всех пациентов.</response>
        [HttpGet]
        public ActionResult<IEnumerable<Patient>> GetAll()
        {
            var patients = patientContext.Patients.ToList();
            logger.LogInformation($"Запрос вернул {patients.Count} пациентов.");
            return Ok(patients);
        }

        /// <summary>
        /// Метод, возвращающий пациента из БД.
        /// </summary>
        /// <param name="id">Уникальный id пациента.</param>
        /// <returns>HTTP ответ, содержащий статус код и пациента, или только статус код.</returns>
        /// <response code="200">Возвращает пациента.</response>
        /// <response code="404">Возвращает ошибку.</response>
        [HttpGet("{id}")]
        public ActionResult<Patient> Get(int id)
        {
            var patient = patientContext.Patients.Find(id);
            if (patient != null)
            {
                logger.LogInformation($"Запрос вернул пациента с id = {id}.");
                return Ok(patient);
            }
            logger.LogError($"Пациент с id = {id} не найден в базе данных.");
            return NotFound();
        }

        /// <summary>
        /// Метод, добавляющий пациента в БД.
        /// </summary>
        /// <param name="patient">Данные новом пациенте.</param>
        /// <returns>HTTP ответ, содержащий ответ сервиса.</returns>
        /// <response code="201">Возвращает ответ сервиса.</response>
        /// <response code="400">Возвращает ответ сервиса.</response>
        [HttpPost]
        public ActionResult Add(Patient patient)
        {
            if (ModelState.IsValid)
            {
                patientContext.Add(patient);
                patientContext.SaveChanges();

                logger.LogInformation($"Добавлен новый пациент с id = {patient.PatientId}.");
                return StatusCode(201, new { isSuccess = true, ErrorMessage = "", StatusCode = 201, Result = patient.PatientId });
            }
            logger.LogError("При добавлении нового пациента данные не были получены от клиента, или они не прошли валидацию.");
            return BadRequest(new { isSucces = false, ErrorMessage = "Полученные данные не прошли валидацию.", StatusCode = 400, Result = "" });
        }

        /// <summary>
        /// Метод, изменяющий данные пациента в БД.
        /// </summary>
        /// <param name="id">Уникальный id пациента.</param>
        /// <param name="patient">Измененные данные пациента.</param>
        /// <returns>HTTP ответ, содержащий ответ сервиса.</returns>
        /// <response code="200">Возвращает ответ сервиса.</response>
        /// <response code="400">Возвращает ответ сервиса.</response>
        /// <response code="404">Возвращает ответ сервиса.</response>
        [HttpPut("{id}")]
        public ActionResult Edit(int id, Patient patient)
        {
            if (ModelState.IsValid)
            {
                var _patient = patientContext.Patients.Find(id);
                if (_patient != null)
                {
                    patient.PatientId = id;
                    patientContext.Entry(_patient).CurrentValues.SetValues(patient);
                    patientContext.SaveChanges();

                    logger.LogInformation($"Изменен пациент с id = {id}.");
                    return Ok(new { isSuccess = true, ErrorMessage = "", StatusCode = 201, Result = id });
                }
                logger.LogError($"Пациент с id = {id} не найден в базе данных.");
                return NotFound(new { isSucces = false, ErrorMessage = $"Пациент с id = {id} не найден в базе данных", StatusCode = 404, Result = "" });
            }
            logger.LogError($"При изменении данных пациента с id = {id} данные не были получены от клиента, или они не прошли валидацию.");
            return BadRequest(new { isSucces = false, ErrorMessage = "Полученные данные не прошли валидацию.", StatusCode = 400, Result = "" });
        }

        /// <summary>
        /// Метод, удаляющий пациента из БД.
        /// </summary>
        /// <param name="id">Уникальный id пациента.</param>
        /// <returns>HTTP ответ, содержащий статус код.</returns>
        /// <response code="200">Возвращает ответ об успешном удалении.</response>
        /// <response code="400">Возвращает ошибку.</response>
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var patient = patientContext.Patients.Find(id);
            if (patient != null)
            {
                patientContext.Patients.Remove(patient);
                patientContext.SaveChanges();

                logger.LogInformation($"Пациент с id = {id} успешно удален из базы данных.");
                return Ok();
            }
            logger.LogError($"Пациент с id = {id} не найден в базе данных.");
            return NotFound();
        }
    }
}