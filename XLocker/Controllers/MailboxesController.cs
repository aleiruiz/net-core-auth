using XLocker.DTOs.Mailbox;
using XLocker.Response.Common;
using XLocker.Response.Mailbox;
using XLocker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace XLocker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailboxesController : ControllerBase
    {
        private readonly ILogger<MailboxesController> _logger;
        private readonly IMailboxService _mailboxService;

        public MailboxesController(ILogger<MailboxesController> logger, IMailboxService mailboxService)
        {
            _logger = logger;
            _mailboxService = mailboxService;
        }

        [HttpGet]
        [Authorize(Policy = "GetMailbox")]
        public async Task<ActionResult<ResponseList<MailboxResponse>>> Get([FromQuery] GetMailboxDTO request)
        {
            if (request.IsPaginated())
            {
                return Ok(await _mailboxService.Get(request));
            }
            return Ok(await _mailboxService.GetAll());
        }

        [HttpPost]
        [Authorize(Policy = "CreateMailbox")]
        public async Task<ActionResult<ABSMailbox>> Create(CreateMailboxDTO request)
        {
            var mailbox = await _mailboxService.Create(request);
            return Ok(mailbox);
        }

        [HttpPut("{mailboxId}")]
        [Authorize(Policy = "UpdateMailbox")]
        public async Task<ActionResult<bool>> Update([FromRoute] string mailboxId, [FromBody] UpdateMailboxDTO request)
        {
            return Ok(await _mailboxService.Update(request, mailboxId));
        }

        [HttpPut("{mailboxId}/Status")]
        [Authorize(Policy = "UpdateMailboxStatus")]
        public async Task<ActionResult<bool>> UpdateStatus([FromRoute] string mailboxId, [FromBody] UpdateMailboxStatusDTO request)
        {
            return Ok(await _mailboxService.UpdateStatus(request, mailboxId));
        }

        [HttpDelete("{mailboxId}")]
        [Authorize(Policy = "DeleteMailbox")]
        public async Task<ActionResult<bool>> Delete([FromRoute] string mailboxId)
        {
            return Ok(await _mailboxService.Delete(mailboxId));
        }
    }

}
