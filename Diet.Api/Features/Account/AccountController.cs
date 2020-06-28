using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Diet.Api.Features.Account
{
    [Route("[controller]")]
    public class AccountController
    {
        private readonly IMediator _mediator;
        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<Create.Response> Create([FromBody] Create.Request request)
        {
            return await _mediator.Send(request);
        }

        [HttpGet("token")]
        public async Task<Token.Response> Token([FromQuery] Token.Request request)
        {
            return await _mediator.Send(request);
        }

        [HttpGet]
        [Authorize(Roles = "Admin, UserManager")]
        public async Task<Index.Response> Index([FromQuery]Index.Request request)
        {
            return await _mediator.Send(request);
        }

        [HttpPut("setting")]
        [Authorize]
        public async Task<IActionResult> UpdateSetting([FromBody] UpdateSetting.Request request)
        {
            await _mediator.Send(request);
            return new NoContentResult();
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] Update.Request request)
        {
            await _mediator.Send(request);
            return new NoContentResult();
        }

        [HttpDelete]
        [Authorize(Roles = "Admin, UserManager")]
        public async Task<IActionResult> Delete([FromBody] Delete.Request request)
        {
            await _mediator.Send(request);
            return new NoContentResult();
        }
    }
}