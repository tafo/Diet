# Diet
This was a demo application for a job interview. The main task was to implement a filter feature. 

I added new features or extended existing ones. Please check Lexer, Parser, ExpressionProvider implementations. 

## Diet.Init

Diet is a **RESTful** API to store calorie data of meals in order to check a diet plan. Currently, it supports only Account and Meal management. 

## Diet.CodeBase

The code base is implemented to be extensible and maintainable. It runs on .NET Core. It was architected to be a *Feature Oriented* monolithic application. Main implementation principle was [YAGNI](https://en.wikipedia.org/wiki/You_aren%27t_gonna_need_it). 

It is using AspNetCore 3.1 with following technologies and methodologies

* CQRS
  * [MediatR](https://github.com/jbogard/MediatR) as a mediator implementation
  * [Command Query Responsibility Segregation](https://martinfowler.com/bliki/CQRS.html)
  * Controller(Request) and Feature(RequestHandler) communicate through the mediator. 
* [JSON Web Token](https://jwt.io/introduction/) authentication with claims
* Api Documentation and Swagger Api Explorer with [Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)
* [Entity Framework Core](https://github.com/dotnet/efcore) as an Object Relational Mapper
  *  [Code First Approach](https://docs.microsoft.com/en-us/ef/ef6/modeling/code-first/migrations/)
* [FluentValidation](https://github.com/JeremySkinner/FluentValidation)
* Features and Infrastructure are tested with sociable testing methodology

## Diet.Requirements

**Main Requirement**

The API should provide filter capabilities for all endpoints that return a list of elements. The API filtering should allow using parenthesis for defining operations precedence and use any combination of the available fields. Check following example.

>(date eq '2016-05-01') AND ((number_of_calories gt 20) OR (number_of_calories lt 10)).

**Other Requirements**

* The API must be able to return data in the JSON format.
* API Users must be able to create an account and log in.
* All API calls must be authenticated.
  * JSON Web Token Authentication based on https://tools.ietf.org/html/rfc7519.
* Write unit tests.
* Each entry has a date, time, text, and number of calories.
  * If the number of calories is not provided, the API should connect to a Calories API provider (https://www.nutritionix.com) and try to get the number of calories for the entered meal.
* The API should provide pagination for all endpoints that return a list of elements.
* Implement at least three roles with different permission levels: 
  * a regular user would only be able to CRUD on their owned records
  * a user manager would be able to CRUD only users
  * An admin would be able to CRUD all records and users.
* User setting – Expected number of calories per day.
  * Each entry should have an extra boolean field set to true if the total for that day is less than expected number of calories per day, otherwise should be false.

## Diet.Current
* Supported Binary Operators
  * Or,
  * And,
  * Equal as eq,
  * NotEqual as ne,
  * GreaterThan as gt,
  * GreaterThanOrEqual as ge,
  * LessThan as lt,
  * LessThanOrEqual as le
* Supported Unary Operators
  * Minus as "-"
  * Not as "not"
* DateTime, Bollean, and Decimal values must be enclosed with quotes

## Diet.Remarks
(!Complete) >> To be continued...
