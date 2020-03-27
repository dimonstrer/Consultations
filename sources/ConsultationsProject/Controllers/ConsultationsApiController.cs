using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ConsultationsProject.Models;
using ConsultationsProject.Models.DTO;
using ConsultationsProject.Models.Interfaces;
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
        /// Сервис пациентов и консультаций.
        /// </summary>
        private readonly IPatientService patientService;

        /// <summary>
        /// Логгер
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Объект маппера.
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// Конструктор контроллера.
        /// </summary>
        /// <param name="patientService">Сервис, ответственный за бизнес логику в работе с пациентами и консультациями.</param>
        /// <param name="logger">Логгер.</param>
        /// <param name="mapper">Объект маппера.</param>
        public ConsultationsApiController(IPatientService patientService, ILogger<ConsultationsApiController> logger, IMapper mapper)
        {
            this.patientService = patientService;
            this.logger = logger;
            this.mapper = mapper;
        }

        /// <summary>
        /// Метод, возвращающий все консультации пациента из БД.
        /// </summary>
        /// <param name="patientId">Уникальный id пациента.</param>
        /// <returns>HTTP ответ, содержащий статус код и консультации.</returns>
        /// <response code="200">Возвращае все консультации.</response>
        [HttpGet("patient/consultations/{patient-id}")]
        public ActionResult<IEnumerable<ConsultationDTO>> GetAll([FromRoute(Name = "patient-id")]int patientId)
        {
            var consultations = patientService.GetConsultations(patientId).ToList();
            var consultationsDTO = mapper.Map<List<ConsultationDTO>>(consultations);
            logger.LogInformation($"Запрос вернул {consultations.Count} консультаций пациента с id = {patientId}.");
            return Ok(consultationsDTO);
        }

        /// <summary>
        /// Метод, возвращающий консультацию пациента из БД.
        /// </summary>
        /// <param name="id">Уникальный id консультации.</param>
        /// <returns>HTTP ответ, содержащий статус код и консультацию, или только статус код.</returns>
        /// <response code="200">Возвращает консультацию.</response>
        /// <response code="404">Возвращает ошибку.</response>
        [HttpGet("consultations/{id}")]
        public ActionResult<ConsultationDTO> Get(int id)
        {
            var consultation = patientService.GetConsultation(id);
            if (consultation != null)
            {
                var consultationDTO = mapper.Map<ConsultationDTO>(consultation);
                logger.LogInformation($"Запрос вернул консультацию с id = {id}.");
                return Ok(consultationDTO);
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
        public ActionResult Add(ConsultationDTO consultation)
        {
            if (ModelState.IsValid)
            {
                var consultationDB = mapper.Map<Consultation>(consultation);
                patientService.AddConsultation(consultationDB);

                logger.LogInformation($"Добавлена новая консультация с id = {consultationDB.ConsultationId}" +
                    $" пациенту с id = {consultationDB.PatientId}.");
                return StatusCode(201, new { isSuccess = true, ErrorMessage = "", StatusCode = 201, Result = consultationDB.ConsultationId });
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
        public ActionResult Edit(int id, ConsultationDTO consultation)
        {
            if (ModelState.IsValid)
            {
                var _consultation = patientService.GetConsultation(id);
                if (_consultation != null)
                {
                    consultation.ConsultationId = id;
                    consultation.PatientId = _consultation.PatientId;

                    var consultationDB = mapper.Map<Consultation>(consultation);

                    patientService.UpdateConsultation(consultationDB);

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
            var consultation = patientService.GetConsultation(id);
            if (consultation != null)
            {
                patientService.DeleteConsultation(id);

                logger.LogInformation($"Консультация с id = {id} успешно удалена из базы данных.");
                return Ok();
            }
            logger.LogError($"Консультация с id = {id} была не найдена в базе данных.");
            return NotFound();
        }
    }
}