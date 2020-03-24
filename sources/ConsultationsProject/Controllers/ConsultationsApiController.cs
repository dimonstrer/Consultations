using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConsultationsProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        /// Конструктор контроллера.
        /// </summary>
        /// <param name="patientContext">Контекст БД с пациентами и консультациями.</param>
        public ConsultationsApiController(PatientsContext patientContext)
        {
            this.patientContext = patientContext;
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
        public ActionResult<Patient> Get(int id)
        {
            var consultation = patientContext.Consultations.Find(id);
            if (consultation != null)
                return Ok(consultation);
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

                return StatusCode(201, new { isSuccess = true, ErrorMessage = "", StatusCode = 201, Result = consultation.ConsultationId });
            }
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

                    return Ok(new { isSuccess = true, ErrorMessage = "", StatusCode = 201, Result = id });
                }
                return NotFound(new { isSucces = false, ErrorMessage = $"Консультация с id = {id} не найдена в базе данных.", StatusCode = 404, Result = "" });
            }
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
                return Ok();
            }
            return NotFound();
        }
    }
}