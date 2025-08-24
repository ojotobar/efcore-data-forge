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
- Supports both SQL and MongoDB
- Options to either use SQL, Mongo or both

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

---

## Usage

### Configurations: appsettings.json
```json
"EFCoreDataForge": {
    "MongoDb": {
      "ConnectionString": "mongodb://localhost:27017",
      "DatabaseName": "KwikNestaMongoStore"
    }
}
```

### 1. SQL: Entity Structure

#### Entity Base (Your database entities must inherit this class)
```csharp
public abstract class EntityBase
{
    public Guid Id { get; set; } Guid.NewGuid();
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

### 2. MongoDB: Entity Structure

#### Entity Base (Your database entities must inherit this class)
```csharp
public abstract class MongoEntityBase
{
}
```

#### User
```csharp
public class User : MongoEntityBase
{
    [BsonId]
    [BsonRepresentation(BsonType.String)] // or BsonType.Binary if you prefer
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
}
```

### Register the package in the Program.cs or Startup.cs
```csharp
// SQL
builder.Services.ConfigureEFCoreDataForge<TDbContext>();

// MongoDB
// There two overloads of this method
// The second overload have no need for adding configs in the appsettings.json
// This assumes to have the IMongoDatabase instance registered in the DI
builder.Services.ConfigureMongoEFCoreDataForge();

// SQL and MongoDB
// You only need to register this if you're going to use both SQL and MongoDB
// Specify the appsettings.json section name. Default is EFCoreDataForge
// You may use the overload if you already have an instance of IMongoDatabase registered in the DI Container
// In this case, you don't know need to have the above configurations in your appsettings.json
builder.Services.ConfigureEFCoreDataForgeManager<TContext>(builder.Configuration);
```

---

### Method Usage

#### 1. SQL
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

#### 2. MongoDB
```csharp
public class TestService
{
    private readonly IEFCoreMongoCrudKit _mongoCrudKit;

    public TestService(IEFCoreMongoCrudKit mongoCrudKit)
    {
        _mongoCrudKit = mongoCrudKit;
    }

    public async Task AddOneUser(MongoDataForgeTestUser user)
    {
        await _mongoCrudKit.InsertAsync(user);
    }

    public async Task<MongoDataForgeTestUser?> GetSingleUser(Guid id)
    {
        return await _mongoCrudKit.FindOneAsync<MongoDataForgeTestUser>(u => u.Id.Equals(id));
    }
}
```

#### SQL and MongoDB
```csharp
public class TestService
{
    private readonly IEFCoreDataForgeManager _dataForgeManager;

    public TestService(IEFCoreDataForgeManager mongoCrudKit)
    {
        _dataForgeManager = mongoCrudKit;
    }

    public async Task AddOneMongoUser(MongoDataForgeTestUser user)
    {
        await _dataForgeManager.Mongo.InsertAsync(user);
    }

    public async Task<MongoDataForgeTestUser?> GetSingleMongoUser(Guid id)
    {
        return await _dataForgeManager.Mongo.FindOneAsync<MongoDataForgeTestUser>(u => u.Id.Equals(id));
    }

    public async Task AddOneSQLUser(DataForgeTestUser user)
    {
        await _dataForgeManager.SQL.InsertAsync(user);
    }

    public async Task<DataForgeTestUser?> GetSingleSQLUser(Guid id)
    {
        return await _dataForgeManager.SQL.FindByIdAsync<DataForgeTestUser>(id, false);
    }
}
```

#### There are methods for delete, replace, update, count, and bulk insert and deletion, 

---

## License
This project is licensed under the MIT License - see the LICENSE file for details.