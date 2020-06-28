using System;
using System.Globalization;
using Diet.Api.Infrastructure;

namespace Diet.Api.Features.Meal
{
    public static class Mapper
    {
        public static Domain.Meal ToMeal(this Create.Request request, Guid accountId)
        {
            return new Domain.Meal
            {
                Id = Guid.NewGuid(),
                Date = request.Date,
                Time = DateTime.ParseExact(request.Time, ResourceConstant.TimeFormat, CultureInfo.InvariantCulture,
                    DateTimeStyles.None),
                Text = request.Text,
                Calories = request.Calories,
                AccountId = accountId,
            };
        }

        public static Index.Model ToModel(this Domain.Meal meal)
        {
            return new Index.Model
            {
                Id = meal.Id,
                Date = meal.Date.ToShortDateString(),
                Time = meal.Time.ToString(ResourceConstant.TimeFormat, CultureInfo.InvariantCulture),
                Text = meal.Text,
                Calories = meal.Calories,
                AccountEmail = meal.Account?.Email,
                CalorieStatus = meal.CalorieStatus
            };
        }
    }
}