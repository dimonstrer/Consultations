using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConsultationsProject.Models;
using ConsultationsProject.Models.Interfaces;
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
        /// Логгер
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Сервис пациентов и консультаций.
        /// </summary>
        private readonly IPatientService patientService;

        /// <summary>
        /// Конструктор контроллера.
        /// </summary>
        /// <param name="patientService">Сервис, ответственный за бизнес логику в работе с пациентами и консультациями.</param>
        /// <param name="logger">Логгер.</param>
        public PatientsApiController(IPatientService patientService,ILogger<PatientsApiController> logger)
        {
            this.patientService = patientService;
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
            var patients = patientService.GetPatients().ToList();
            logger.LogInformation($"Запрос вернул {patients.Count()} пациентов.");
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
            var patient = patientService.GetPatient(id);
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

                if (patientService.AddPatient(patient))
                {
                    logger.LogInformation($"Добавлен новый пациент с id = {patient.PatientId}.");
                    return StatusCode(201, new { isSuccess = true, ErrorMessage = "", StatusCode = 201, Result = patient.PatientId });
                }
                else
                {
                    logger.LogError($"При добавлении нового пациента " +
                            $"обнаружен пациент с идентичным СНИЛС = {patient.PensionNumber}.");
                    return BadRequest(new { isSucces = false, ErrorMessage = "При добавлении нового пациента " +
                            $"обнаружен пациент с идентичным СНИЛС = {patient.PensionNumber}.", StatusCode = 400, Result = "" });
                }
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
                var _patient = patientService.GetPatient(id);
                if (_patient != null)
                {
                    patient.PatientId = id;
                    if (patientService.UpdatePatient(patient))
                    {
                        logger.LogInformation($"Изменен пациент с id = {id}.");
                        return Ok(new { isSuccess = true, ErrorMessage = "", StatusCode = 201, Result = id });
                    }
                    else
                    {
                        logger.LogError($"При добавлении нового пациента " +
                            $"обнаружен пациент с идентичным СНИЛС = {patient.PensionNumber}.");
                        return BadRequest(new
                        {
                            isSucces = false,
                            ErrorMessage = "При добавлении нового пациента " +
                                $"обнаружен пациент с идентичным СНИЛС = {patient.PensionNumber}.",
                            StatusCode = 400,
                            Result = ""
                        });
                    }
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
            var patient = patientService.GetPatient(id);
            if (patient != null)
            {
                patientService.DeletePatient(id);

                logger.LogInformation($"Пациент с id = {id} успешно удален из базы данных.");
                return Ok();
            }
            logger.LogError($"Пациент с id = {id} не найден в базе данных.");
            return NotFound();
        }
    }
}