using System;
using System.Threading;
using System.Threading.Tasks;
using Diet.Api.Data;
using Diet.Api.Infrastructure.Providers;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Diet.Api.Features.Account
{
    /// <summary>
    /// !!! This is an example of how to handle one-to-one relationships in a single feature !!!
    /// Account and AccountSetting have One-To-One relationship. So, they are handled in the same feature
    /// AccountSetting should be moved to its own "Feature-Folder" if it becomes another feature
    /// Actually, This decision should be based on "Business Decisions"
    /// </summary>
    public class UpdateSetting
    {
        public class Request : IRequest
        {
            public decimal TargetCalories { get; set; }
        }

        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                // The maximum value of decimal(6,2) is 9999.99
                RuleFor(x => x.TargetCalories).NotEmpty().ScalePrecision(2, 6);
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

            public async Task Handle(Request request, CancellationToken cancellationToken)
            {
                var setting = await _context.AccountSettings.SingleOrDefaultAsync(x => x.AccountId == _currentAccount.Id,
                    cancellationToken);

                if (setting == null)
                {
                    setting = new Domain.AccountSetting
                    {
                        Id = Guid.NewGuid(),
                        AccountId = _currentAccount.Id
                    };
                    _context.AccountSettings.Add(setting);
                }

                setting.TargetCalories = request.TargetCalories;

                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}