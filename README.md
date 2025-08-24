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
- Supports **SQL, MongoDB, and Dapper**
- Options to either use SQL, Mongo, Dapper, or a combination

---

## 📦 Installation
Install from [NuGet.org](https://www.nuget.org/packages/EFCore.DataForge):

```bash
dotnet add package EFCore.DataForge
```

Or add it to your `.csproj` file:
```bash
<PackageReference Include="EFCore.DataForge" Version="x.y.z" />
```

---

## ⚙️ Usage
### Configurations: appsettings.json

```json
"EFCoreDataForge": {
    "MongoDb": {
      "ConnectionString": "mongodb://localhost:27017",
      "DatabaseName": "MongoDatabaseName"
    }
},
"ConnectionString": {
    "DefaultConnection": "Server=localhost;Database=DatabaseName;User Id=sa;Password=yourpassword;"
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

#### User
```csharp
public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.String)] // or BsonType.Binary if you prefer
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
}
```

### Register the package in the Program.cs or Startup.cs
```csharp
// SQL (EFCore)
builder.Services.ConfigureEFCoreDataForge<TDbContext>();

// MongoDB
builder.Services.ConfigureMongoEFCoreDataForge();

// SQL + Mongo (both)
builder.Services.ConfigureEFCoreDataForgeManager<TDbContext>(builder.Configuration);

// Dapper
builder.Services.AddSingleton<ISqlConnectionFactory>(sp =>
    new SqlConnectionFactory(builder.Configuration.GetConnectionString("SqlDb")!));
builder.Services.AddScoped<IDapperForge, DapperForge>();
```

---

### Method Usage

#### 1. SQL
```csharp
public class TestService 
{
    private readonly IEFCoreCrudKit _crudKit;
    public TestService(IEFCoreCrudKit crudKit) => _crudKit = crudKit;

    public Task AddUser(User user) => _crudKit.InsertAsync(user);

    public Task<User?> GetUser(Guid id) =>
        _crudKit.FindByIdAsync<User>(id, trackChanges: true);
}
```

#### 2. MongoDB
```csharp
public class TestService
{
    private readonly IEFCoreMongoCrudKit _mongoCrudKit;
    public TestService(IEFCoreMongoCrudKit mongoCrudKit) => _mongoCrudKit = mongoCrudKit;

    public Task AddOneUser(User user) => _mongoCrudKit.InsertAsync(user);

    public Task<User?> GetSingleUser(Guid id) =>
        _mongoCrudKit.FindOneAsync<User>(u => u.Id == id);
}
```

#### SQL and MongoDB
```csharp
public class TestService
{
    private readonly IEFCoreDataForgeManager _dataForgeManager;
    public TestService(IEFCoreDataForgeManager dataForgeManager) => _dataForgeManager = dataForgeManager;

    public Task AddSqlUser(User user) => _dataForgeManager.SQL.InsertAsync(user);

    public Task<User?> GetSqlUser(Guid id) =>
        _dataForgeManager.SQL.FindByIdAsync<User>(id, false);

    public Task AddMongoUser(User user) => _dataForgeManager.Mongo.InsertAsync(user);

    public Task<User?> GetMongoUser(Guid id) =>
        _dataForgeManager.Mongo.FindOneAsync<User>(u => u.Id == id);
}
```

## 🔌 Dapper Integration

### DataForge also provides lightweight Dapper helpers for raw SQL performance scenarios.
#### Register
```csharp
builder.ServicesConfigureDataForgeRawCrudKit(builder.Configuration, "DefaultConnection");
```
#### Example Usage
```csharp
public class ReportService
{
    private readonly IDataForgeRawCrudKit _dapper;
    public ReportService(IDataForgeRawCrudKit dapper) => _dapper = dapper;

    public Task<IEnumerable<User>> GetActiveUsers()
        => _dapper.QueryAsync<User>("SELECT * FROM Users WHERE IsDeprecated = 0");

    public Task<int> AddUser(User user)
        => _dapper.ExecuteAsync("INSERT INTO Users (Id, Name) VALUES (@Id, @Name)", user);
}
```
---

# FluentQuery

`FluentQuery` is a lightweight, chainable SQL query builder for .NET.  
It allows you to construct SQL statements using a fluent API without manually concatenating strings.  

---

## Features

- Build `SELECT`, `INSERT`, and `JOIN` queries fluently.  
- Supports `WHERE`, `AND`, `OR`, `IN`, `NOT IN`.  
- Easy handling of collections (`Guid` or `object`) for `IN` clauses.  
- Order and group results (`ORDER BY`, `GROUP BY`).  
- Pagination with `LIMIT`.  
- Supports column/value insertion (`COLUMNS`, `VALUES`).  

---

## Usage

### 1. Basic SELECT

```csharp
var query = new FluentQuery()
    .Select("Id, Name")
    .From("Users")
    .ToQuery();

// Output:
// SELECT Id, Name FROM Users
```

### 2. SELECT with WHERE and AND

```csharp
var query = new FluentQuery("*")
    .From("Orders")
    .Where("Status = 'Active'")
    .And("Amount > 100")
    .ToQuery();

// Output:
// SELECT * FROM Orders WHERE Status = 'Active' AND Amount > 100
```

### 3. SELECT with IN clause

```csharp
var userIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

var query = new FluentQuery()
    .Select("Id", "Name")
    .From("Users")
    .WhereIn("UserId", userIds)
    .ToQuery();

// Output:
// SELECT Id, Name FROM Users WHERE UserId IN ('guid1','guid2')
```

### 4. JOIN Example

```csharp
var query = new FluentQuery()
    .Select("u.Id, u.Name, o.Total")
    .From("Users u")
    .Join("Orders o")
    .On("u.Id = o.UserId")
    .OrderBy("o.Total", ascending: false)
    .ToQuery();

// Output:
// SELECT u.Id, u.Name, o.Total FROM Users u JOIN Orders o ON u.Id = o.UserId ORDER BY o.Total DESC
```

### 5. INSERT Example

```csharp
var query = new FluentQuery("INSERT INTO Users")
    .Columns("Id", "Name", "Email")
    .Values("1", "'John'", "'john@example.com'")
    .ToQuery();
```

---

## Notes

- This is a string builder utility. It does **not** protect against SQL injection.  
- Use with **trusted input only**, or parameterize queries when executing against a database.  
- Ideal for **internal query generation**, quick prototypes, or admin tools.  

---

## License
This project is licensed under the MIT License - see the LICENSE file for details.