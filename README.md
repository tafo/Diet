# Diet
A demo application for a job interview. 
The main task was to implement a filter feature. Please check Lexer, Parser, ExpressionProvider implementations. 

# Diet.Init

Diet is a **RESTful** API to store calorie data of meals in order to check a diet plan. Currently, it supports only Account and Meal management. 

# Diet.CodeBase

The code base is implemented to be extensible and maintainable. It runs on .NET Core. It was architected to be a *Feature Oriented* monolithic application. Main implementation principle was [YAGNI](https://en.wikipedia.org/wiki/You_aren%27t_gonna_need_it). 

It is using AspNetCore 3.1 with following technologies and methodologies

 - CQRS
	 - [MediatR](https://github.com/jbogard/MediatR) as a mediator implementation
	 - [Command Query Responsibility Segregation](https://martinfowler.com/bliki/CQRS.html)
	 - >Controller(Request) and Feature(RequestHandler) communicate through the mediator. 
 - [JSON Web Token](https://jwt.io/introduction/) authentication with claims
 - Api Documentation and Swagger Api Explorer with [Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)
 - [Entity Framework Core](https://github.com/dotnet/efcore) as an Object Relational Mapper
	 - [Code First Approach](https://docs.microsoft.com/en-us/ef/ef6/modeling/code-first/migrations/)
 - [FluentValidation](https://github.com/JeremySkinner/FluentValidation)

To be continued...
