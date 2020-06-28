using System;

namespace Diet.Api.Domain
{
    public class AccountSetting : Entity
    {
        public decimal TargetCalories { get; set; }

        public Guid AccountId { get; set; }
        public Account Account { get; set; }
    }
}