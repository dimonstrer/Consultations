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
using Microsoft.Extensions.Configuration;
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
        /// Объект маппера.
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// Количество пациентов на страницу.
        /// </summary>
        private readonly int PatientsPageSize;

        /// <summary>
        /// Количество консультаций на страницу у пациента.
        /// </summary>
        private readonly int ConsultationsPageSize;

        /// <summary>
        /// Конструктор контроллера.
        /// </summary>
        /// <param name="patientService">Сервис, ответственный за бизнес логику в работе с пациентами и консультациями.</param>
        /// <param name="logger">Логгер.</param>
        /// <param name="mapper">Объект маппера.</param>
        /// <param name="config">Конфигурация.</param>
        public PatientsApiController(IPatientService patientService, ILogger<PatientsApiController> logger, IMapper mapper, IConfiguration config)
        {
            this.patientService = patientService;
            this.logger = logger;
            this.mapper = mapper;
            PatientsPageSize = config.GetValue<int>("PaginationSettings:PatientsPageSize");
            ConsultationsPageSize = config.GetValue<int>("PaginationSettings:ConsultationsPageSize");
        }

        /// <summary>
        /// Метод, возвращающий всех пациентов из БД.
        /// </summary>
        /// <returns>HTTP ответ, содержащий статус код и пациентов.</returns>
        /// <response code="200">Возвращает всех пациентов.</response>
        /*[HttpGet]
        public ActionResult<IEnumerable<PatientDTO>> GetAll()
        {
            var patients = patientService.GetPatients().ToList();
            var patientsDTO = mapper.Map<List<PatientDTO>>(patients);
            logger.LogInformation($"Запрос вернул {patients.Count()} пациентов.");
            return Ok(patientsDTO);
        }*/

        /// <summary>
        /// Метод, возвращающий всех пациентов из БД постранично.
        /// </summary>
        /// <returns>HTTP ответ, содержащий статус код и пациентов.</returns>
        /// <response code="200">Возвращает всех пациентов.</response>
        [HttpGet]
        public ActionResult<IEnumerable<PatientDTO>> GetAllPaged(int page = 1)
        {
            var count = patientService.GetPatients().Count();
            PageViewModel pageViewModel = new PageViewModel(count, page, PatientsPageSize);
            if (page <= 0 || page > pageViewModel.TotalPages)
                page = 1;

            var patients = patientService.GetPatients()
                .Skip((page - 1) * PatientsPageSize)
                .Take(PatientsPageSize)
                .ToList();
            var patientsDTO = mapper.Map<List<PatientDTO>>(patients);

            IndexViewModel result = new IndexViewModel { PageViewModel = pageViewModel, Patients = patientsDTO };

            logger.LogInformation($"Запрос вернул {patients.Count()} пациентов на странице = {page}.");
            return Ok(result);
        }

        /// <summary>
        /// Метод, возвращающий пациента из БД.
        /// </summary>
        /// <param name="id">Уникальный id пациента.</param>
        /// <returns>HTTP ответ, содержащий статус код и пациента, или только статус код.</returns>
        /// <response code="200">Возвращает пациента.</response>
        /// <response code="404">Возвращает ошибку.</response>
        /// <param name="page">Текущая страница списка консультаций.</param>
        [HttpGet("{id}")]
        public ActionResult<PatientDTO> Get(int id, int page = 1)
        {
            var patient = patientService.GetPatient(id);
            if (patient != null)
            {
                var count = patientService.GetConsultations(id).Count();
                var pageViewModel = new PageViewModel(count, page, ConsultationsPageSize);

                if (page <= 0 || page > pageViewModel.TotalPages)
                    page = 1;

                patient.Consultations = patientService.GetConsultations(id)
                    .Where(x => x.PatientId == id)
                    .Skip((page - 1) * ConsultationsPageSize)
                    .Take(ConsultationsPageSize)
                    .ToList();

                var patientDTO = mapper.Map<PatientDTO>(patient);

                var result = new PatientViewModel { PageViewModel = pageViewModel, Patient = patientDTO };
                logger.LogInformation($"Запрос вернул пациента с id = {id}.");
                return Ok(result);
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
        public ActionResult Add(PatientDTO patient)
        {
            if (ModelState.IsValid)
            {
                var patientDB = mapper.Map<Patient>(patient);

                if (patientService.AddPatient(patientDB))
                {
                    logger.LogInformation($"Добавлен новый пациент с id = {patientDB.PatientId}.");
                    return StatusCode(201, new { isSuccess = true, ErrorMessage = "", StatusCode = 201, Result = patientDB.PatientId });
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
        public ActionResult Edit(int id, PatientDTO patient)
        {
            if (ModelState.IsValid)
            {
                var _patient = patientService.GetPatient(id);
                if (_patient != null)
                {
                    patient.PatientId = id;

                    var patientDB = mapper.Map<Patient>(patient);
                    if (patientService.UpdatePatient(patientDB))
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