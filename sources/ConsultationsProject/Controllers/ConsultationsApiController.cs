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
        private readonly PatientsContext patientContext;

        public ConsultationsApiController(PatientsContext patientContext)
        {
            this.patientContext = patientContext;
        }

        [HttpGet("patient/consultations/{patient-id}")]
        public ActionResult<IEnumerable<Consultation>> GetAll([FromRoute(Name = "patient-id")]int patientId)
        {
            var consultations = patientContext.Consultations
                .Where(x => x.PatientId == patientId).ToList();
            return Ok(consultations);
        }

        [HttpGet("consultations/{id}")]
        public ActionResult<Patient> Get(int id)
        {
            var consultation = patientContext.Consultations.Find(id);
            if (consultation != null)
                return Ok(consultation);
            return NotFound();
        }

        [HttpPost("/consultations")]
        public ActionResult Add(Consultation consultation)
        {
            if (ModelState.IsValid)
            {
                patientContext.Consultations.Add(consultation);
                patientContext.SaveChanges();

                var id = patientContext.Consultations.Find(consultation).ConsultationId;
                return StatusCode(201, new { isSuccess = true, ErrorMessage = "", StatusCode = 201, Result = id });
            }
            return BadRequest(new { isSucces = false, ErrorMessage = "Полученные данные не прошли валидацию.", StatusCode = 400, Result = "" });
        }

        [HttpPut("consultations/{id}")]
        public ActionResult Edit(int id, Patient consultation)
        {
            if (ModelState.IsValid)
            {
                var _consultation = patientContext.Consultations.Find(id);
                if (_consultation != null)
                {
                    patientContext.Entry(_consultation).CurrentValues.SetValues(consultation);
                    patientContext.SaveChanges();

                    return Ok(new { isSuccess = true, ErrorMessage = "", StatusCode = 201, Result = id });
                }
                return NotFound(new { isSucces = false, ErrorMessage = $"Консультация с id = {id} не найдена в базе данных.", StatusCode = 404, Result = "" });
            }
            return BadRequest(new { isSucces = false, ErrorMessage = "Полученные данные не прошли валидацию.", StatusCode = 400, Result = "" });
        }

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