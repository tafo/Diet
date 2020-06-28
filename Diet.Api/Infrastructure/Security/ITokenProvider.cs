using Diet.Api.Domain;

namespace Diet.Api.Infrastructure.Security
{
    public interface ITokenProvider
    {
        string Create(Account account);
    }
}