using System;

namespace Diet.Api.Domain
{
    public class Meal : Entity
    {
        public DateTime Date { get; set; }
        public DateTime Time { get; set; }
        public string Text { get; set; }
        public decimal? Calories { get; set; }
        public bool CalorieStatus { get; set; }

        public Guid AccountId { get; set; }
        public Account Account { get; set; }
    }
}