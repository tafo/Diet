using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Diet.Api.Data;
using Diet.Api.Infrastructure;
using Diet.Api.Infrastructure.ExceptionHandling;
using Diet.Api.Infrastructure.Security;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Diet.Api.Features.Account
{
    public class Token
    {
        public class Response
        {
            public string Token { get; set; }
        }

        public class Request : IRequest<Response>
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.Email).NotEmpty().EmailAddress();
                RuleFor(x => x.Password).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly DietContext _context;
            private readonly IPasswordHasher _passwordHasher;
            private readonly ITokenProvider _tokenProvider;

            public Handler(DietContext context, IPasswordHasher passwordHasher, ITokenProvider tokenProvider)
            {
                _context = context;
                _passwordHasher = passwordHasher;
                _tokenProvider = tokenProvider;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                var account = await _context.Accounts.Where(x => x.Email == request.Email).SingleOrDefaultAsync(cancellationToken);

                if (account == null)
                {
                    throw new RestException(HttpStatusCode.Unauthorized, ResourceConstant.Unauthorized);
                }

                if (!_passwordHasher.VerifyHashedPassword(request.Password, account.PasswordHash))
                {
                    throw new RestException(HttpStatusCode.Unauthorized, ResourceConstant.Unauthorized);
                }

                return new Response
                {
                    Token = _tokenProvider.Create(account)
                };
            }
        }
    }
}