using linen_management_system.DTOModels;
using linen_management_system.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LinenManagement.Controllers
{
    [ApiController]
    [Route("api/cartlogs")]
    [Authorize]
    public class CartLogsController : ControllerBase
    {
        private readonly ICartLogService _service;
        private readonly ILogger<CartLogsController> _logger;

        public CartLogsController(ICartLogService service, ILogger<CartLogsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("{cartLogId}")]
        public async Task<IActionResult> GetCartLog(int cartLogId)
        {
            try
            {
                var data = await _service.GetCartLogByIdAsync(cartLogId);
                return Ok(new { success = true, cartLog = data });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCartLogs([FromQuery] string cartType, [FromQuery] int? locationId, [FromQuery] int? employeeId)
        {
            var data = await _service.GetCartLogsAsync(cartType, locationId, employeeId);
            return Ok(new { success = true, cartLogs = data });
        }

        [HttpPost("upsert")]
        public async Task<IActionResult> UpsertCartLog([FromBody] CartLogDto request)
        {
            try
            {
                var employeeId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
                var data = await _service.UpsertCartLogAsync(request, employeeId);
                return Ok(new { success = true, cartLogs = data });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { success = false, message = "Unauthorized" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{cartLogId}")]
        public async Task<IActionResult> DeleteCartLog(int cartLogId)
        {
            try
            {
                var employeeId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
                await _service.DeleteCartLogAsync(cartLogId, employeeId);
                return Ok(new { success = true, message = "Deleted" });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { success = false, message = "Unauthorized" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
        }
    }
}