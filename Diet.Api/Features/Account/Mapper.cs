using System;
using Diet.Api.Domain;
using Diet.Api.Infrastructure.Security;

namespace Diet.Api.Features.Account
{
    public static class Mapper
    {
        public static Domain.Account ToAccount(this Create.Request request, IPasswordHasher passwordHasher)
        {
            return new Domain.Account
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                PasswordHash = passwordHasher.HashPassword(request.Password),
                Role = Role.RegularUser
            };
        }

        public static Index.Model ToModel(this Domain.Account account)
        {
            return new Index.Model
            {
                Id = account.Id,
                Email = account.Email,
                Role = account.Role
            };
        }
    }
}