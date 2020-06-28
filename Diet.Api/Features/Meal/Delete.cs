using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Diet.Api.Data;
using Diet.Api.Domain;
using Diet.Api.Infrastructure;
using Diet.Api.Infrastructure.ExceptionHandling;
using Diet.Api.Infrastructure.Providers;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Diet.Api.Features.Meal
{
    public class Delete
    {
        public class Request : IRequest
        {
            public Guid Id { get; set; }
        }

        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.Id).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Request>
        {
            private readonly DietContext _context;
            private readonly ICurrentAccountProvider _currentAccount;

            public Handler(DietContext context, ICurrentAccountProvider currentAccount)
            {
                _context = context;
                _currentAccount = currentAccount;
            }

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                var meal = await _context.Meals.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (_currentAccount.Role == Role.RegularUser && meal.AccountId != _currentAccount.Id)
                {
                    throw new RestException(HttpStatusCode.Forbidden, ResourceConstant.Forbidden);
                }

                if (meal == null)
                {
                    throw new RestException(HttpStatusCode.NotFound, ResourceConstant.NotFound);
                }

                _context.Meals.Remove(meal);

                await _context.SaveChangesAsync(cancellationToken);

                return default;
            }
        }
    }
}