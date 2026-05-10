# GradeBook API

This is a small ASP.NET Core Web API project for managing grades.

## Implemented Requirements

- Reviewed the initial codebase and documented SOLID/design issues in `SOLID_REVIEW.md`
- Renamed generic classes to match the GradeBook domain:
  - `Item` -> `Grade`
  - `IItemReader` -> `IGradeRepository`
  - `ItemRepository` -> `GradeRepository`
  - `ItemController` -> `GradeController`
- Added a service layer:
  - `IGradeService`
  - `GradeService`
- Added filtering logic for the first `N` passing and active grades
- Refactored the repository to fetch data from an external endpoint
- Upgraded the project from .NET 8 to .NET 10

## Technologies

- ASP.NET Core Web API
- .NET 10
- C#
- HttpClient
- Dependency Injection

## Main Endpoints

Get all grades:

```http
GET /api/grade
````

Get a grade by id:

```http
GET /api/grade/{id}
```

Get the first N passing and active grades:

```http
GET /api/grade/passing?count=3
```

A passing grade is a grade with value greater than or equal to `5`, and the grade must also be active.

## Configuration

The external data source is configured in `appsettings.json`:

```json
"GradeSource": {
  "Url": "https://gist.githubusercontent.com/ArdeleanTudor/8ea407832cd9794960e0e6bbd1319f6e/raw/"
}
```

## Run the project

```bash
dotnet restore
dotnet build
dotnet run
```

The project requires a .NET SDK version that supports .NET 10.


