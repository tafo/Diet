using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Diet.Api.Features.Meal
{
    [Route("[controller]")]
    [Authorize(Roles = "Admin, RegularUser")]
    public class MealController
    {
        private readonly IMediator _mediator;

        public MealController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<Index.Response> Index([FromQuery] Index.Request request)
        {
            return await _mediator.Send(request);
        }

        [HttpPost]
        public async Task<Create.Response> Create([FromBody] Create.Request request)
        {
            return await _mediator.Send(request);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Update.Request request)
        {
            await _mediator.Send(request);
            return new NoContentResult();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] Delete.Request request)
        {
            await _mediator.Send(request);
            return new NoContentResult();
        }
    }
}