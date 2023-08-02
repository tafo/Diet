    using System.Threading;
using System.Threading.Tasks;
using Diet.Api.Data;
using Diet.Api.Infrastructure.Providers;
using Diet.Api.Infrastructure.Security;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Diet.Api.Features.Account
{
    /// <summary>
    /// There is only Password field currently to be updated
    /// </summary>
    public class Update
    {
        public class Request : IRequest
        {
            public string Password { get; set; }
        }

        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.Password).NotEmpty().MinimumLength(3);
            }
        }

        public class Handler : IRequestHandler<Request>
        {
            private readonly DietContext _context;
            private readonly ICurrentAccountProvider _currentAccount;
            private readonly IPasswordHasher _passwordHasher;

            public Handler(DietContext context, ICurrentAccountProvider currentAccount, IPasswordHasher passwordHasher)
            {
                _context = context;
                _currentAccount = currentAccount;
                _passwordHasher = passwordHasher;
            }
            
            public async Task Handle(Request request, CancellationToken cancellationToken)
            {
                var account = await _context.Accounts.SingleOrDefaultAsync(x => x.Id == _currentAccount.Id, cancellationToken);

                account.PasswordHash = _passwordHasher.HashPassword(request.Password);

                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}