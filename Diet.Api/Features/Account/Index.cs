using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Diet.Api.Data;
using Diet.Api.Features.Filter;
using Diet.Api.Helper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Diet.Api.Features.Account
{
    public class Index
    {
        public class Model
        {
            public Guid Id { get; set; }
            public string Email { get; set; }
            public string Role { get; set; }
        }

        public class Response : FilteredResponse<Model> { }

        public class Request : FilteredRequest, IRequest<Response> { }

        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.PageIndex).GreaterThanOrEqualTo(0);
                RuleFor(x => x.PageSize).LessThanOrEqualTo(100);
            }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly DietContext _context;

            public Handler(DietContext context)
            {
                _context = context;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                var accounts = _context.Accounts.Filter(request.Filter).AsNoTracking();
                var itemCount = await accounts.CountAsync(cancellationToken);
                var items = await accounts.Paginate(request.PageIndex, request.PageSize).Select(x => x.ToModel())
                    .ToListAsync(cancellationToken);
                
                return new Response
                {
                    Items = items,
                    PageIndex = request.PageIndex,
                    PageSize = request.PageSize,
                    PageCount = Extensions.GetPageCount(itemCount, request.PageSize)
                };
            }
        }
    }
}