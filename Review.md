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

builder.Services.AddScoped<IItemReader, ItemRepository>();


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
