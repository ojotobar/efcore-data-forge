# EFCore.DataForge

[![NuGet](https://img.shields.io/nuget/v/EFCore.DataForge.svg)](https://www.nuget.org/packages/EFCore.DataForge)
[![NuGet Downloads](https://img.shields.io/nuget/dt/EFCore.DataForge.svg)](https://www.nuget.org/packages/EFCore.DataForge)

**EFCore.DataForge** is a lightweight utility library for **Entity Framework Core** that provides ready-to-use helpers for common database operations — including CRUD, bulk operations, and soft deletes — so you can write less boilerplate code and focus on your business logic.
It demonstrates a common pattern for entity modeling with metadata such as creation date, update date, and deprecation status.

---

## 🚀 Features
- **Create, Read, Update, Delete** helpers for EF Core entities
- **Bulk operations** for insert, update, and delete
- **Soft delete** support with query filters
- Clean, reusable API for DbContext operations
- Works with EF Core 8 and later
- Easy to extend
- **EntityBase**: Abstract base class with common properties for all entities.
- Automatic `Guid` ID generation.
- Timestamps for creation and last update.
- Deprecation flag to mark entities as obsolete.

---

## 📦 Installation
Install from [NuGet.org](https://www.nuget.org/packages/EFCore.DataForge):

```bash
dotnet add package EFCore.DataForge
```

Or add it to your `.csproj` file:
```xml
<PackageReference Include="EFCore.DataForge" Version="x.y.z" />
```

## Usage

### Entity Structure

#### Entity Base (Your database entities must inherit this class)
```csharp
public abstract class EntityBase
{
    public Guid Id => Guid.NewGuid();
    public bool IsDeprecated { get; set; }
    public DateTime CreatedOn => DateTime.UtcNow;
    public DateTime LastUpdatedOn { get; set; } = DateTime.UtcNow;
}
```

#### User
```csharp
public class User : EntityBase
{
    public string Name { get; set; } = string.Empty;
}
```

### Register the package in the Program.cs or Startup.cs
```csharp
builder.Services.ConfigureEFCoreDataForge<TDbContext>();
```

### Inject the IEFCoreCrudKit interface into the constructor and use the methods
```csharp
public class TestService 
{
    private readonly IEFCoreCrudKit _crudKit;
    public TestService(IEFCoreCrudKit crudKit)
    {
        _crudKit = crudKit;
    }

    public async Task AddUser(User user)
    {
        await _crudKit.InsertAsync(user);
    }

    public async Task<User?> GetUser(Guid id)
    {
        var user = await _repository
            .FindByIdAsync<User>(id, trackChanges: true);

        return user;
    }
}
```

## License
This project is licensed under the MIT License - see the LICENSE file for details.