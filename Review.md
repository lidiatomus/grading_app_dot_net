# SOLID Review

## 1 - Missing Dependency Injection registration

### Principle affected
Dependency Inversion Principle

### Location
Program.cs and ItemController.cs

### Problem
ItemController depends on the IItemReader abstraction, but no implementation was registered in the dependency injection container. Because of this, ASP.NET Core cannot create ItemController at runtime.

### Fix
Registered ItemRepository as the implementation for IItemReader in Program.cs:

```csharp
builder.Services.AddScoped<IItemReader, ItemRepository>();
```

## 2 - Generic Naming

### Principle affected
Single Responsibility Principle and clean code readability.

### Location
`Item.cs`, `IItemReader.cs`, `ItemRepository.cs`, `ItemController.cs`.

### Problem
The original class and interface names were too generic for a GradeBook application.

The project manages grades, but the domain model was named `Item`. This made the code harder to understand because `Item` does not clearly describe what the object represents.

Original names:

```csharp
public class Item
```

```csharp
public interface IItemReader
```

```csharp
public class ItemRepository
```

```csharp
public class ItemController
```

The name `IItemReader` was also not very consistent with the repository layer. Since the class was responsible for retrieving grade data, it was clearer to describe it as a repository abstraction.

### Fix

The names were changed to better match the domain of the application and the repository layer.

Renamed:

* `Item` to `Grade`
* `IItemReader` to `IGradeRepository`
* `ItemRepository` to `GradeRepository`
* `ItemController` to `GradeController`

Final names:

```csharp
public class Grade
```

```csharp
public interface IGradeRepository
```

```csharp
public class GradeRepository : IGradeRepository
```

```csharp
public class GradeController : ControllerBase
```

This makes the code easier to understand because the names now describe the actual responsibility of each class.

## 3 - Business Logic Inside Controller

### Principle affected
Single Responsibility Principle.

### Location
`GradeController.cs`, method `GetAll`.

### Problem
The controller was responsible for handling HTTP requests, retrieving data, calculating statistics, and applying business logic.

A controller should mainly handle the request and response flow. Business logic should not be placed directly in the controller because this makes the controller harder to maintain and test.

Original example:

```csharp
var totalCount = gradeList.Count;
var averageValue = gradeList.Any() ? gradeList.Average(g => g.Value) : 0;
```

The new requirement also needed filtering grades by business rules:

```csharp
g.Value >= 5 && g.IsActive
```

This kind of logic belongs in a service layer, not directly in the controller.

### Fix
A service layer was introduced.

Created:

- `IGradeService`
- `GradeService`

The controller now depends on `IGradeService`, while `GradeService` depends on `IGradeRepository`.

Final structure:

```text
GradeController -> IGradeService -> GradeService -> IGradeRepository -> GradeRepository
```

The filtering logic was moved to `GradeService`:

```csharp
public async Task<IEnumerable<Grade>> GetFirstPassingActiveGradesAsync(int count)
{
    if (count <= 0)
    {
        return Enumerable.Empty<Grade>();
    }

    var grades = await _gradeRepository.GetAllAsync();

    return grades
        .Where(g => g.Value >= 5 && g.IsActive)
        .Take(count);
}
```

This keeps the controller focused on HTTP concerns and the service focused on business logic.

## 4 - In-Memory Repository Data Source

### Principle affected
Single Responsibility Principle and Dependency Inversion Principle.

### Location
`GradeRepository.cs`, methods `GetAllAsync` and `GetByIdAsync`.

### Problem
The original repository used an in-memory list as its data source.

Original code:

```csharp
protected readonly List<Grade> _grades = new();
```

This was not aligned with the requirement, because the repository had to retrieve grade data from an external endpoint.

The repository also kept internal state that was not needed for the final solution.

### Fix
The in-memory list was removed.

The repository was refactored to use `HttpClient` and fetch the grade data from the external endpoint configured in `appsettings.json`.

A response model was added because the external JSON contains the grades inside an `items` property:

```csharp
public class GradeResponse
{
    public List<Grade> Items { get; set; } = new();
}
```

The repository now deserializes the external response and returns the grades:

```csharp
var gradeResponse = JsonSerializer.Deserialize<GradeResponse>(
    json,
    new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    });

return gradeResponse?.Items ?? Enumerable.Empty<Grade>();
```

The dependency injection registration was also changed from:

```csharp
builder.Services.AddScoped<IGradeRepository, GradeRepository>();
```

to:

```csharp
builder.Services.AddHttpClient<IGradeRepository, GradeRepository>();
```

This makes the repository responsible for data access and removes the hardcoded in-memory data source.

## 5 - Framework Upgrade

### Requirement
Upgrade the project from .NET 8 to .NET 10.

### Location
`Siemens.Internship2026.GradeBook.csproj`.

### Problem
The project was originally targeting .NET 8.

Original configuration:

```xml
<TargetFramework>net8.0</TargetFramework>
```

### Fix
The target framework was updated to .NET 10.

Final configuration:

```xml
<TargetFramework>net10.0</TargetFramework>
```

The project was then rebuilt using a .NET SDK version that supports .NET 10.