using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Diet.Api.Data;
using Diet.Api.Domain;
using Diet.Api.Infrastructure;
using Diet.Api.Infrastructure.ExceptionHandling;
using Diet.Api.Infrastructure.Providers;
using Diet.Api.Services;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Diet.Api.Features.Meal
{
    public class Update
    {
        public class Request : IRequest
        {
            public Guid Id { get; set; }
            [DataType(DataType.Date)]
            public DateTime Date { get; set; }
            public string Time { get; set; }
            public string Text { get; set; }
            public decimal? Calories { get; set; }
        }

        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.Id).NotEmpty();
                RuleFor(x => x.Time).Must(BeValidTimeFormat).WithMessage(ResourceConstant.InvalidTime);
                RuleFor(x => x.Text).NotEmpty().MaximumLength(4000);
                RuleFor(x => x.Calories).GreaterThanOrEqualTo(0);
            }

            private bool BeValidTimeFormat(string timeValue)
            {
                return DateTime.TryParseExact(timeValue, ResourceConstant.TimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
            }
        }

        public class Handler : IRequestHandler<Request>
        {
            private readonly DietContext _context;
            private readonly ICurrentAccountProvider _currentAccount;
            private readonly ICaloriesService _caloriesService;

            public Handler(DietContext context, ICurrentAccountProvider currentAccount, ICaloriesService caloriesService)
            {
                _context = context;
                _currentAccount = currentAccount;
                _caloriesService = caloriesService;
            }

            public async Task Handle(Request request, CancellationToken cancellationToken)
            {
                var query = _context.Meals.Where(x => x.Id == request.Id);

                // User can only update her own meal
                // Admin can update all meals
                if (_currentAccount.Role != Role.Admin)
                {
                    query = query.Where(x => x.Account.Id == _currentAccount.Id);
                }

                var meal = await query.SingleOrDefaultAsync(cancellationToken);

                if (meal == null)
                {
                    throw new RestException(HttpStatusCode.NotFound, ResourceConstant.NotFound);
                }

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

                meal.Date = request.Date;
                meal.Time = DateTime.ParseExact(request.Time, ResourceConstant.TimeFormat, CultureInfo.InvariantCulture,
                    DateTimeStyles.None);
                meal.Text = request.Text;
                meal.Calories = request.Calories;
                // CalorieStatus calculation should be optimized
                meal.CalorieStatus = account?.CurrentCalories.GetValueOrDefault() + request.Calories - meal.Calories < account?.TargetCalories;

                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}