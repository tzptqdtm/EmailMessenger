using System.Text.Json;
using System.Text.Json.Serialization;
using EmailMessenger.Core.Services.Email;
using EmailMessenger.Core.Services.Job;
using EmailMessenger.Data.DataServices;
using EmailMessenger.Models.Data;
using EmailMessenger.Models.Web;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmailMessenger.Web.Controllers;
[Route("api/[controller]")]
[ApiController]
public class MailsController : ControllerBase
{
    private readonly ILogger<MailsController> _logger;
    private readonly IDataService _dataService;
    private readonly IBackgroundJobClient _backgroundJobClient;
    public MailsController(ILogger<MailsController> logger, IDataService dataService, IBackgroundJobClient backgroundJobClient)
    {
        _logger = logger;
        _dataService = dataService;
        _backgroundJobClient = backgroundJobClient;
    }

    /// <summary>
    /// Эндпоинт для отправки писем
    /// </summary>
    /// <param name="request"></param>
    /// <returns>messageId</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Post([FromBody] SendingRequest? request)
    {
        if (request == null)
        {
            return BadRequest("Request cannot be null");
        }

        if (request.Recipients == null || request.Recipients.Length == 0)
        {
            return BadRequest("Recipients cannot be null or empty");
        }

        if (string.IsNullOrWhiteSpace(request.Subject))
        {
            return BadRequest("Subject cannot be null or whitespace");
        }

        if (string.IsNullOrWhiteSpace(request.Body))
        {
            return BadRequest("Body cannot be null or whitespace");
        }

        try
        {
            var guid = await _dataService.CreateMessageAsync(request);

            _backgroundJobClient.Enqueue<IJobScheduler>(scheduler => scheduler.ScheduleNewJob(guid));
            
            return Ok(guid.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email");

            return InternalError();
        }

    }

    /// <summary>
    /// Эндпоинт для проверки статуса отправленных писем
    /// </summary>
    /// <param name="messageId"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get(string messageId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(messageId))
            {
                return BadRequest("Не указан идентификатор сообщения");
            }

            if (!Guid.TryParse(messageId, out var id))
            {
                return BadRequest("Неверный формат идентификатора сообщения");
            }

            var message = await _dataService.GetResult(id);

            if (message == null)
            {
                return NotFound();
            }
            
            return Ok(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting emails");

            return InternalError();
        }

    }

    private ObjectResult InternalError()
    {
        return StatusCode(500, "Something went wrong. Please contact the Administrator");
    }
}
