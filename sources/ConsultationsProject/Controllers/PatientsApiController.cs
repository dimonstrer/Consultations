using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConsultationsProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ConsultationsProject.Controllers
{
    [Route("api/patient-management/patients")]
    [ApiController]
    public class PatientsApiController : ControllerBase
    {
        private readonly PatientsContext patientContext;

        public PatientsApiController(PatientsContext patientContext)
        {
            this.patientContext = patientContext;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Patient>> GetAll()
        {
            var patients = patientContext.Patients.ToList();
            return Ok(patients);
        }

        [HttpGet("{id}")]
        public ActionResult<Patient> Get(int id)
        {
            var patient = patientContext.Patients.Find(id);
            if (patient != null)
                return Ok(patient);
            return NotFound();
        }

        [HttpPost]
        public ActionResult Add(Patient patient)
        {
            if (ModelState.IsValid)
            {
                patientContext.Add(patient);
                patientContext.SaveChanges();

                var id = patientContext.Patients.Find(patient).PatientId;
                return StatusCode(201, new { isSuccess = true, ErrorMessage = "", StatusCode = 201, Result = id });
            }
            return BadRequest(new { isSucces = false, ErrorMessage = "Полученные данные не прошли валидацию.", StatusCode = 400, Result = "" });
        }

        [HttpPut("{id}")]
        public ActionResult Edit(int id, Patient patient)
        {
            if (ModelState.IsValid)
            {
                var _patient = patientContext.Patients.Find(id);
                if (_patient != null)
                {
                    patientContext.Entry(_patient).CurrentValues.SetValues(patient);
                    patientContext.SaveChanges();

                    return Ok(new { isSuccess = true, ErrorMessage = "", StatusCode = 201, Result = id });
                }
                return NotFound(new { isSucces = false, ErrorMessage = $"Пациент с id = {id} не найден в базе данных", StatusCode = 404, Result = "" });
            }
            return BadRequest(new { isSucces = false, ErrorMessage = "Полученные данные не прошли валидацию.", StatusCode = 400, Result = "" });
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var patient = patientContext.Patients.Find(id);
            if (patient != null)
            {
                patientContext.Patients.Remove(patient);
                patientContext.SaveChanges();
                return Ok();
            }
            return NotFound();
        }
    }
}