using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Diet.Api.Data;
using Diet.Api.Infrastructure;
using Diet.Api.Infrastructure.Providers;
using Diet.Api.Services;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Diet.Api.Features.Meal
{
    public class Create
    {
        public class Response
        {
            public Guid Id { get; set; }
        }

        public class Request : IRequest<Response>
        {
            [DataType(DataType.Date)]
            public DateTime Date { get; set; }
            // ToDo : Display time format in swagger
            public string Time { get; set; }
            /// <summary>
            /// Food name
            /// </summary>
            public string Text { get; set; }
            public decimal? Calories { get; set; }
        }

        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.Time).Must(BeValidTimeFormat).WithMessage(ResourceConstant.InvalidTime);
                RuleFor(x => x.Text).NotEmpty().MaximumLength(4000);
                RuleFor(x => x.Calories).GreaterThanOrEqualTo(0);
            }

            private bool BeValidTimeFormat(string timeValue)
            {
                return DateTime.TryParseExact(timeValue, ResourceConstant.TimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
            }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly DietContext _context;
            private readonly ICurrentAccountProvider _currentAccount;
            private readonly ICaloriesService _caloriesService;

            public Handler(
                DietContext context, 
                ICurrentAccountProvider currentAccount, ICaloriesService caloriesService)
            {
                _context = context;
                _currentAccount = currentAccount;
                _caloriesService = caloriesService;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                var account =
                    await
                    (
                        from setting in _context.AccountSettings
                        where setting.AccountId == _currentAccount.Id
                        select new
                        {
                            setting.TargetCalories,
                            CurrentCalories = setting.Account.Meals.Where(x => x.Date == request.Date).Sum(x => x.Calories)
                        }
                    ).SingleOrDefaultAsync(cancellationToken);
                    
                if (!request.Calories.HasValue)
                {
                    request.Calories = await _caloriesService.GetCaloriesAsync(request.Text);
                }

                var meal = request.ToMeal(_currentAccount.Id);

                // This is a business operation. So, it should not be in the related mapper extension
                meal.CalorieStatus = account?.CurrentCalories.GetValueOrDefault() + request.Calories < account?.TargetCalories;

                _context.Meals.Add(meal);
                
                await _context.SaveChangesAsync(cancellationToken);
                
                return new Response
                {
                    Id = meal.Id
                };
            }
        }
    }
}