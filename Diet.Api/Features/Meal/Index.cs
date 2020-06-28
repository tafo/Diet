using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Diet.Api.Data;
using Diet.Api.Domain;
using Diet.Api.Features.Filter;
using Diet.Api.Helper;
using Diet.Api.Infrastructure.Providers;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Diet.Api.Features.Meal
{
    public class Index
    {
        public class Model
        {
            public Guid Id { get; set; }
            public string Date { get; set; }
            public string Time { get; set; }
            public string Text { get; set; }
            public decimal? Calories { get; set; }
            public string AccountEmail { get; set; }
            public bool CalorieStatus { get; set; }
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
            private readonly ICurrentAccountProvider _currentAccount;

            public Handler(DietContext context, ICurrentAccountProvider currentAccount)
            {
                _context = context;
                _currentAccount = currentAccount;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                var meals = _context.Meals.Filter(request.Filter).AsNoTracking();

                meals = _currentAccount.Role == Role.Admin
                    ? meals.Include(x => x.Account)
                    : meals.Where(x => x.AccountId == _currentAccount.Id);

                var itemCount = await meals.CountAsync(cancellationToken);
                var items = await meals.Paginate(request.PageIndex, request.PageSize).Select(x => x.ToModel())
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