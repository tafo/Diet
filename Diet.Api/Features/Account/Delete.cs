using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Diet.Api.Data;
using Diet.Api.Infrastructure;
using Diet.Api.Infrastructure.ExceptionHandling;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Diet.Api.Features.Account
{
    /// <summary>
    /// We could use Id property in Request model
    /// Additionally, We could add a Status field to check deleted users. Check Status = 0(Active), Status = 1(Deleted), ...
    /// Because, it is not a good practice to delete Account(User) records. But, I need to check the decisions of product team. 
    /// </summary>
    public class Delete
    {
        public class Request : IRequest<Response>
        {
            public string Email { get; set; }
        }

        public class Response
        {
            
        }

        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(1024);
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
                var account = await _context.Accounts.SingleOrDefaultAsync(x => x.Email == request.Email, cancellationToken);

                if (account == null)
                {
                    throw new RestException(HttpStatusCode.NotFound, ResourceConstant.NotFound);
                }

                _context.Accounts.Remove(account);
                
                await _context.SaveChangesAsync(cancellationToken);
                
                return new Response();
            }
        }
    }
}