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
    [Route("api/consultation-management")]
    [ApiController]
    public class ConsultationsApiController : ControllerBase
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
        /// <param name="logger">Логгер.</param>
        public ConsultationsApiController(PatientsContext patientContext, ILogger<ConsultationsApiController> logger)
        {
            this.patientContext = patientContext;
            this.logger = logger;
        }

        /// <summary>
        /// Метод, возвращающий все консультации пациента из БД.
        /// </summary>
        /// <param name="patientId">Уникальный id пациента.</param>
        /// <returns>HTTP ответ, содержащий статус код и консультации.</returns>
        /// <response code="200">Возвращае все консультации.</response>
        [HttpGet("patient/consultations/{patient-id}")]
        public ActionResult<IEnumerable<Consultation>> GetAll([FromRoute(Name = "patient-id")]int patientId)
        {
            var consultations = patientContext.Consultations
                .Where(x => x.PatientId == patientId).ToList();
            logger.LogInformation($"Запрос вернул {consultations.Count} консультаций пациента с id = {patientId}.");
            return Ok(consultations);
        }

        /// <summary>
        /// Метод, возвращающий консультацию пациента из БД.
        /// </summary>
        /// <param name="id">Уникальный id консультации.</param>
        /// <returns>HTTP ответ, содержащий статус код и консультацию, или только статус код.</returns>
        /// <response code="200">Возвращает консультацию.</response>
        /// <response code="404">Возвращает ошибку.</response>
        [HttpGet("consultations/{id}")]
        public ActionResult<Consultation> Get(int id)
        {
            var consultation = patientContext.Consultations.Find(id);
            if (consultation != null)
            {
                logger.LogInformation($"Запрос вернул консультацию с id = {id}.");
                return Ok(consultation);
            }
            logger.LogError($"Консультация с id = {id} не найдена в базе данных.");
            return NotFound();
        }

        /// <summary>
        /// Метод, добавляющий консультацию пациента в БД.
        /// </summary>
        /// <param name="consultation">Данные новой консультации.</param>
        /// <returns>HTTP ответ, содержащий ответ сервиса.</returns>
        /// <response code="201">Возвращает ответ сервиса.</response>
        /// <response code="400">Возвращает ответ сервиса.</response>
        [HttpPost("consultations")]
        public ActionResult Add(Consultation consultation)
        {
            if (ModelState.IsValid)
            {
                patientContext.Consultations.Add(consultation);
                patientContext.SaveChanges();

                logger.LogInformation($"Добавлена новая консультация с id = {consultation.ConsultationId}" +
                    $" пациенту с id = {consultation.PatientId}.");
                return StatusCode(201, new { isSuccess = true, ErrorMessage = "", StatusCode = 201, Result = consultation.ConsultationId });
            }
            logger.LogError("При добавлении новой консультации данные не были получены от клиента, или они не прошли валидацию.");
            return BadRequest(new { isSucces = false, ErrorMessage = "Полученные данные не прошли валидацию.", StatusCode = 400, Result = "" });
        }

        /// <summary>
        /// Метод, изменяющий данные консультации пациента в БД.
        /// </summary>
        /// <param name="id">Уникальный id консультации.</param>
        /// <param name="consultation">Измененные данные консультации.</param>
        /// <returns>HTTP ответ, содержащий ответ сервиса.</returns>
        /// <response code="200">Возвращает ответ сервиса.</response>
        /// <response code="400">Возвращает ответ сервиса.</response>
        /// <response code="404">Возвращает ответ сервиса.</response>
        [HttpPut("consultations/{id}")]
        public ActionResult Edit(int id, Consultation consultation)
        {
            if (ModelState.IsValid)
            {
                var _consultation = patientContext.Consultations.Find(id);
                if (_consultation != null)
                {
                    consultation.ConsultationId = id;
                    consultation.PatientId = _consultation.PatientId;

                    patientContext.Entry(_consultation).CurrentValues.SetValues(consultation);
                    patientContext.SaveChanges();

                    logger.LogInformation($"Изменена консультация с id = {id} пациента с id = {consultation.PatientId}.");
                    return Ok(new { isSuccess = true, ErrorMessage = "", StatusCode = 201, Result = id });
                }
                logger.LogError($"Консультация с id = {id} не найдена в базе данных.");
                return NotFound(new { isSucces = false, ErrorMessage = $"Консультация с id = {id} не найдена в базе данных.", StatusCode = 404, Result = "" });
            }
            logger.LogError($"При изменении данных консультации с id = {id} данные не были получены от клиента, или они не прошли валидацию.");
            return BadRequest(new { isSucces = false, ErrorMessage = "Полученные данные не прошли валидацию.", StatusCode = 400, Result = "" });
        }

        /// <summary>
        /// Метод, удаляющий консультацию пациента из БД.
        /// </summary>
        /// <param name="id">Уникальный id консультации.</param>
        /// <returns>HTTP ответ, содержащий статус код.</returns>
        /// <response code="200">Возвращает ответ об успешном удалении.</response>
        /// <response code="400">Возвращает ошибку.</response>
        [HttpDelete("consultations/{id}")]
        public ActionResult Delete(int id)
        {
            var consultation = patientContext.Consultations.Find(id);
            if (consultation != null)
            {
                patientContext.Consultations.Remove(consultation);
                patientContext.SaveChanges();

                logger.LogInformation($"Консультация с id = {id} успешно удалена из базы данных.");
                return Ok();
            }
            logger.LogError($"Консультация с id = {id} была не найдена в базе данных.");
            return NotFound();
        }
    }
}