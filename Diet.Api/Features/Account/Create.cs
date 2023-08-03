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
    public class Create
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
                RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(1024).WithMessage("Invalid Email");
                RuleFor(x => x.Password).NotEmpty().MinimumLength(3);
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
                if(await _context.Accounts.AnyAsync(x => x.Email == request.Email, cancellationToken))
                {
                    throw new RestException(HttpStatusCode.Conflict, ResourceConstant.AlreadyUsedEmail);
                }

                var account = request.ToAccount(_passwordHasher);
                _context.Accounts.Add(account);
                
                await _context.SaveChangesAsync(cancellationToken);
                
                return new Response
                {
                    Token = _tokenProvider.Create(account)
                };
            }
        }
    }
}