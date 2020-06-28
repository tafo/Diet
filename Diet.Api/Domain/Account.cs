using System.Collections.Generic;

namespace Diet.Api.Domain
{
    public class Account : Entity
    {
        public string Email { get; set; }   
        public string PasswordHash { get; set; }
        public string Role { get; set; }

        public AccountSetting Setting { get; set; }

        public ICollection<Meal> Meals { get; set; }
    }

    /// <summary>
    /// Implementing Role with a new Entity is not necessary
    /// If product team requests it then it should be implemented in its own context
    /// </summary>
    public static class Role
    {
        public const string Admin = nameof(Admin);
        public const string UserManager = nameof(UserManager);
        public const string RegularUser = nameof(RegularUser);
    }
}