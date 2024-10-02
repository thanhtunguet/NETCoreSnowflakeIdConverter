# Project Overview: Handling ID Fields and Converting Long Values to Strings

This project is an ASP.NET Core Web API using Entity Framework Core with custom handling for fields that represent IDs. It includes special JSON serialization and deserialization to avoid JavaScript's limitation with 64-bit integers when interacting with client-side JavaScript applications.

## What is an ID Field?

In this project, an **ID field** refers to any field in the data model whose name ends with "id" or "Id" (regardless of whether it uses PascalCase or camelCase). These fields represent either **primary keys** or **foreign keys** in the database, which are typically of type `long` (64-bit integers) or `long?` (nullable 64-bit integers). For example:
- `ProductId` (Primary Key)
- `CategoryId` (Foreign Key)

These fields uniquely identify entities in the database and are used to establish relationships between different tables.

## The JavaScript Number Limitation

JavaScript's `Number` type uses 64-bit floating point numbers internally (IEEE 754), which can only **safely represent integers up to 53 bits**. This means that numbers larger than `2^53` (approximately `9,007,199,254,740,992`) lose precision when represented in JavaScript. Since C# `long` values are 64-bit integers, this limitation can cause issues when handling IDs in JavaScript applications, especially when sending data to/from the client.

For example, if a `long` value in C# is larger than what JavaScript can safely handle (such as a primary key with value `9876543210123456789`), the number will be inaccurately represented when passed to JavaScript, potentially causing errors or inconsistencies.

### The Solution: Converting `long` to Strings

To avoid this issue, this project converts all `long` and `long?` (nullable long) fields into **strings** when serializing objects to JSON. This ensures that the full 64-bit precision is maintained when sending data to the client, and that JavaScript applications can safely handle these values as strings.

Conversely, when receiving data from the client, the project converts these string values back to `long` or `long?` as needed during deserialization.

## How the Project Converts `long` Fields to Strings

We use **Newtonsoft.Json** (`Json.NET`) to handle the custom serialization and deserialization for all ID fields. Specifically, two custom `JsonConverter` classes are implemented:
- `LongToStringConverterForId`: Handles non-nullable `long` fields.
- `NullableLongToStringConverterForId`: Handles nullable `long?` fields.

### Custom `JsonConverter` Logic

1. **Serialization**: Converts all `long` and `long?` ID fields to strings when sending data to the client. This avoids any precision loss in JavaScript.
2. **Deserialization**: Converts string representations of `long` and `long?` values back to their respective numeric types when receiving data from the client.

### What Qualifies as an ID Field?
Any field whose name ends with `"id"` or `"Id"` (case-insensitive) is treated as an ID field and converted accordingly. This includes fields like:
- `ProductId`
- `categoryId`
- `OrderItemId`
- `UserId`

### Example Model

```csharp
public class Product
{
    public long ProductId { get; set; }          // Non-nullable ID
    public string Name { get; set; }
    public long? CategoryId { get; set; }        // Nullable foreign key ID
    public Category Category { get; set; }
}

public class Category
{
    public long CategoryId { get; set; }         // Non-nullable ID
    public string CategoryName { get; set; }
}
```

### Example JSON Response

When you request product data from the API, the `long` and `long?` ID fields will be serialized as strings:

```json
{
  "ProductId": "1234567890123456789",
  "Name": "Sample Product",
  "CategoryId": "9876543210123456789",
  "Category": {
    "CategoryId": "9876543210123456789",
    "CategoryName": "Sample Category"
  }
}
```

### Example JSON Request

When you send data to the API, you can provide the IDs as strings, and the API will automatically convert them back to `long` or `long?` as needed:

```json
{
  "ProductId": "1234567890123456789",
  "Name": "New Product",
  "CategoryId": "9876543210123456789"
}
```

### How to Add Custom Converters in ASP.NET Core

The converters are registered globally in the `Program.cs` file (for ASP.NET Core 6.0 and later) or in `Startup.cs` (for earlier versions).

In **Program.cs** (for ASP.NET Core 6.0+):

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.Converters.Add(new LongToStringConverterForId());
        options.SerializerSettings.Converters.Add(new NullableLongToStringConverterForId());
    });

var app = builder.Build();

app.MapControllers();
app.Run();
```

### Entity Framework Core Integration

This project uses **Entity Framework Core** to manage the database schema and perform CRUD operations. The `DbContext` is registered and configured in `Program.cs` to use SQL Server:

```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

The models are defined with `long` and `long?` for ID fields, and these fields are converted to/from strings during JSON serialization and deserialization.

### How to Run Migrations

1. **Add Initial Migration**:
   ```bash
   dotnet ef migrations add InitialCreate
   ```

2. **Apply the Migration to the Database**:
   ```bash
   dotnet ef database update
   ```

### Conclusion

By converting all `long` and `long?` ID fields to strings when serializing to JSON, this project avoids JavaScript's limitation with large integers and ensures safe transmission of ID values between the server and client. The conversion is automatic for all fields that match the ID naming convention, and other fields are handled normally without any special treatment.
