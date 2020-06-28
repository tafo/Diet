using System;
using System.Threading;
using System.Threading.Tasks;
using Diet.Api.Data;
using MediatR;

namespace Diet.Api.Infrastructure
{
    public class TransactionPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly DietContext _dietContext;

        public TransactionPipelineBehavior(DietContext dietContext)
        {
            _dietContext = dietContext;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            try
            {
                await _dietContext.BeginTransactionAsync();
                var response = await next();
                await _dietContext.CommitTransactionAsync();
                return response;
            }
            catch (Exception)
            {
                _dietContext.RollbackTransaction();
                throw;
            }
        }
    }
}