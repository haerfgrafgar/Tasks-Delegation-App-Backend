using CordApp.Dtos.Task;
using CordApp.Dtos.Work;
using CordApp.Interface;
using CordApp.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CordApp.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/work")]
    [ApiController]
    public class WorkController : ControllerBase
    {
        private readonly IWorkRepository _workRepo;
        

        public WorkController(IWorkRepository workRepo)
        {
            _workRepo = workRepo;
        }

        //--------------------------------------------------- GET ---------------------------------------------------//

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var works = await _workRepo.GetAllAsync();

            return Ok(works);
        }

        [HttpGet("{workId:int}")]
        [Authorize]
        public async Task<IActionResult> GetByWorkIdAsync([FromRoute] int workId)
        {
            var work = await _workRepo.GetByIdAsync(workId);

            if (work == null)
                return NotFound("WorkId não encontrado.");

            return Ok(work);
        }

        [HttpGet("executando")]
        [Authorize]
        public async Task<IActionResult> GetUnfinishedStartedByExecId()
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized("No userId assigned to this token.");

            var works = await _workRepo.GetAllNotFinishedStartedByExecIdAsync(userId);

            return Ok(works);
        }

        [HttpGet("calendar/executando")]
        [Authorize]
        public async Task<IActionResult> GetUnfinishedByExecId()
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized("No userId assigned to this token.");

            var works = await _workRepo.GetAllNotFinishedStartedAndFutureByExecIdAsync(userId);

            List<CalendarWorkDto> response = new List<CalendarWorkDto>();

            foreach (var work in works)
            {
                response.Add(work.ToCalendarWorkDtoFromWork());
            }

            return Ok(response);
        }

        [HttpGet("coordenando")]
        [Authorize]
        public async Task<IActionResult> GetUnfinishedStartedByCordId()
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized("No userId assigned to this token.");

            var works = await _workRepo.GetAllNotFinishedStartedByCordIdAsync(userId);

            return Ok(works);
        }

        [HttpGet("calendar/coordenando")]
        [Authorize]
        public async Task<IActionResult> GetUnfinishedByCordId()
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized("No userId assigned to this token.");

            var works = await _workRepo.GetAllNotFinishedStartedAndFutureByCordIdAsync(userId);

            List<CalendarWorkDto> response = new List<CalendarWorkDto>();

            foreach(var work in works)
            {
                response.Add(work.ToCalendarWorkDtoFromWork());
            }

            return Ok(response);
        }

        [HttpGet("executando/{username}")]
        [Authorize]
        public async Task<IActionResult> GetUserUnfinishedStartedByExecId([FromRoute] string username)
        {
            var works = await _workRepo.GetAllNotFinishedStartedByUsernameAsync(username);

            if (works == null)
                return NotFound("Username not found.");

            return Ok(works);
        }

        [HttpGet("calendar/executando/{username}")]
        [Authorize]
        public async Task<IActionResult> GetUserUnfinishedByExecId([FromRoute] string username, [FromQuery] TimeRangeDto timeRange)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var works = await _workRepo.GetAllNotFinishedStartedAndFutureByUsernameAsync(username, timeRange);

            if (works == null)
                return NotFound("Username not found.");

            List<CalendarWorkDto> response = new List<CalendarWorkDto>();

            foreach (var work in works)
            {
                response.Add(work.ToCalendarWorkDtoFromWork());
            }

            return Ok(response);
        }

        [HttpGet("finalizado")]
        [Authorize]
        public async Task<IActionResult> GetFinishedByCordId()
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized("No userId assigned to this token.");

            var works = await _workRepo.GetAllFinishedByCordIdAsync(userId);

            return Ok(works);
        }

        [HttpGet("atual")]
        [Authorize]
        public async Task<IActionResult> GetSelfInProgressWorkByUserId()
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized("No userId assigned to this token.");

            var work = await _workRepo.GetSelfInProgressWorkAsync(userId);
            if (work == null)
                return NotFound("GetInProgressWorkByUserId(), work não encontrado ou userId não encontrado.");

            return Ok(work);
        }

        [HttpGet("atual/{username}")]
        [Authorize]
        public async Task<IActionResult> GetInProgressWorkByUsername([FromRoute] string username)
        {
            var work = await _workRepo.GetInprogressWorkByUsername(username);

            if (work == null)
                return NotFound("Work or username not found.");

            return Ok(work);
        }

        //--------------------------------------------------- POST ---------------------------------------------------//

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateWorkRequestDto workDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized("No userId assigned to this token.");

            var workModel = workDto.ToWorkFromCreateDto();
            workModel.CordId = userId;

            var work = await _workRepo.CreateAsync(workModel, workDto.ExecUsername);
            if (work == null)
                return NotFound("Username does not exist");

            return Ok(workModel);
        }

        //--------------------------------------------------- PUT ---------------------------------------------------//

        [HttpPut("finalizar/{id:int}")]
        [Authorize]
        public async Task<IActionResult> Finalizar([FromRoute] int id)
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized("No userId assigned to this token.");

            var work = await _workRepo.FinishWork(id, userId);
            if (work == null)
                return NotFound("Work ID não encontrado ou usuário não tem permissão.");


            return Ok(work);
        }

        [HttpPut("corrigir/{workId:int}")]
        [Authorize]
        public async Task<IActionResult> Corrigir([FromBody] CorrigirWorkDto corrigirWorkDto, [FromRoute] int workId)
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized("No userId assigned to this token.");

            var work = await _workRepo.CorrigirWork(workId, userId, corrigirWorkDto);
            if (work == null)
                return NotFound("Work ID não encontrado ou usuário não tem permissão ou tarefa não finalizada.");


            return Ok(work);
        }

        //--------------------------------------------------- DELETE --------------------------------------------------//






    }
}
