using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XLocker.DTOs.Diagnostic;
using XLocker.Response.Common;
using XLocker.Response.Maintance;
using XLocker.Services;

namespace XLocker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiagnosticController : ControllerBase
    {
        private readonly ILogger<DiagnosticController> _logger;
        private readonly IDiagnosticService _diagnosticService;

        public DiagnosticController(ILogger<DiagnosticController> logger, IDiagnosticService diagnosticService)
        {
            _logger = logger;
            _diagnosticService = diagnosticService;
        }

        [HttpGet]
        [Authorize(Policy = "GetDiagnostic")]
        public async Task<ActionResult<ResponseList<DiagnosticResponse>>> Get([FromQuery] GetDiagnosticDTO request)
        {
            if (request.IsPaginated())
            {
                return Ok(await _diagnosticService.Get(request));
            }
            return Ok(await _diagnosticService.GetAll());
        }

        [HttpPost]
        public async Task<ActionResult<ABSDiagnostic>> Create(CreateDiagnosticDTO request)
        {
            var maintanceOrder = await _diagnosticService.Create(request);
            return Ok(maintanceOrder);
        }

        [HttpDelete("{diagnosticId}")]
        [Authorize(Policy = "DeleteDiagnostic")]
        public async Task<ActionResult<bool>> Delete([FromRoute] string diagnosticId)
        {
            return Ok(await _diagnosticService.Delete(diagnosticId));
        }
    }

}
