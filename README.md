# Implementing and Securing an API with ASP.NET Core
Started the tutorial on 2017-09-16.

The MyCodeCamp.Data project found in GitHub is using .NET Core 1.0. I updated everything to use the new [.NET Core 2.0](https://blogs.msdn.microsoft.com/dotnet/2017/08/14/announcing-net-core-2-0/).
The Claims and Roles are not found after migrating the solution to .NET Core 2.0. I will work on that later in the course.

## Getting Started
See the [MyCodeCamp](MyCodeCamp) project.

## Reading Data

### First Controller
Avoid launching the browser

![Avoid launching the browser](Images/201709160916-Avoid_Launching_The_Browser.png)

### The Data Model

The MyCodeCamp.Data project is found here: https://github.com/shawnwildermuth/WebAPICoreResources. (In the future that project should be a library. There's a comment on the Program.cs file).

The tutorial showed a CMD `dotnet ef database update` to create the database. I actually did it in the Package Manager Console `Update-Database`

### Getting Collections

Copy the connection string from the MyCodeCamp.Data project to MyCodeCamp

![Copy the connection string](Images/201709161023-Copy_The_Connection_String.png)

### Getting Items

Add `services.AddMvc().AddJsonOptions` to avoid circular serialization in EF Entities.

