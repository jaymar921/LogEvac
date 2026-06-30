# LogEvac [![NuGet Version](https://img.shields.io/nuget/v/LogEvac.svg?style=flat)](https://www.nuget.org/packages/LogEvac/)&nbsp;[![NuGet Downloads](https://img.shields.io/nuget/dt/logevac.svg)](https://www.nuget.org/packages/LogEvac/)

> Automated log cleanup library for .NET applications using Serilog MSSQL sinks
>
> Created by [JayMar921](https://jayharronabejar.vercel.app)

**LogEvac** is a lightweight .NET library that automatically removes old log records from SQL Server databases configured with Serilog MSSQL sinks. Perfect for keeping your logging database lean and performant without manual intervention.

## 📦 Quick Install

Choose your preferred installation method:

| Method | Command | .NET Version |
|--------|---------|---------|
| **.NET CLI** | `dotnet add package LogEvac --version 1.1.0` | 10 |
| **NuGet Package Manager Console** | `Install-Package LogEvac -Version 1.1.0` | 10 |
| **.csproj PackageReference** | `<PackageReference Include="LogEvac" Version="1.1.0" />` | 10 |
| **.NET CLI** | `dotnet add package LogEvac --version 1.0.4` | 8 |
| **NuGet Package Manager Console** | `Install-Package LogEvac -Version 1.0.4` | 8 |
| **.csproj PackageReference** | `<PackageReference Include="LogEvac" Version="1.0.4" />` | 8 |

## 🔗 Example Project

**[Project-Serilog](https://github.com/jaymar921/Project-SerilogDI)** - Complete working reference implementation

This repository demonstrates:
- ✅ Full LogEvac integration setup
- ✅ Hangfire configuration with InMemoryStorage
- ✅ Serilog MSSQL sink configuration
- ✅ Dependency injection patterns

**Perfect for:** Getting started quickly or understanding integration patterns


## 📋 Table of Contents

- [Features](#features)
- [Prerequisites](#prerequisites)
- [Installation & Dependencies](#installation--dependencies)
- [Configuration](#configuration)
- [Getting Started](#getting-started)
- [Configuration Options](#configuration-options)
- [Example Project](#example-project)
- [Troubleshooting](#troubleshooting)
- [License](#license)

## ✨ Features

- **🤖 Automated Cleanup**: Schedule automatic log deletion based on age criteria
- **⚙️ Highly Configurable**: Fine-tune cleanup frequency and retention periods (minutes, hours, days, weeks, or months)
- **🔧 Easy Integration**: Simple extension methods for dependency injection
- **📊 Application-Specific**: Target specific applications and environments
- **🔄 Hangfire Powered**: Leverages Hangfire for reliable, persistent job scheduling
- **📈 Performance**: Efficiently remove large batches of old logs without impacting database performance

## 📋 Prerequisites

- **.NET 8.0** or later
- **SQL Server** (LocalDB, Express, Standard, or Enterprise)
- **Serilog** configured with **MSSQL sink** and **ApplicationName** column defined
- **Hangfire** already configured in your ASP.NET Core application
- **Entity Framework Core** 8.0 or later

## 📦 Installation & Dependencies

### 1. Add to Your Project

This library is designed to be integrated directly into your project. Add the source files to your ASP.NET Core application.

### 2. Required NuGet Dependencies

Ensure your project has these dependencies installed:

```xml
<!-- In your .csproj or install via NuGet Package Manager -->
<ItemGroup>
  <PackageReference Include="Hangfire.AspNetCore" Version="1.8.0" />
  <PackageReference Include="Hangfire.SqlServer" Version="1.8.0" />
  <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
  <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
  <PackageReference Include="Serilog" Version="3.0.0" />
  <PackageReference Include="Serilog.Sinks.MSSqlServer" Version="6.5.0" />
</ItemGroup>
```

**Quick Install via Package Manager Console:**

```powershell
Install-Package Hangfire.AspNetCore
Install-Package Hangfire.SqlServer
Install-Package Microsoft.EntityFrameworkCore
Install-Package Microsoft.EntityFrameworkCore.SqlServer
Install-Package Serilog
Install-Package Serilog.Sinks.MSSqlServer
```

## ⚙️ Configuration

### Step 1: Update `appsettings.json`

Add the logging connection string and LogEvac settings:

```json
{
  "ConnectionStrings": {
    "LoggingDb": "Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=true;Database=Logs"
  },
  "LogEvacSettings": {
    "ApplicationName": "MyApplicationName",
    "Environment": "Production",
    "LoggingTable": "LogEvents",
    "CheckingFrequencyInMinutes": 0,
    "CheckingFrequencyInHours": 2,
    "CleanUpDataOlderThanMinutesValue": 0,
    "CleanUpDataOlderThanDaysValue": 0,
    "CleanUpDataOlderThanWeeksValue": 0,
    "CleanUpDataOlderThanMonthsValue": 1
  }
}
```

### Step 2: Reference `appsettings.example.json`

Use the included `appsettings.example.json` as a template for your environment-specific configurations.

## 🚀 Getting Started

### Step 1: Configure Services in `Program.cs`

In your ASP.NET Core startup file, register LogEvac after Hangfire and Serilog:

```csharp
// Setup Hangfire
builder.Services.AddHangfire(config => config.UseSqlServerStorage(hangfireConnection));
builder.Services.AddHangfireServer();

// Setup Serilog (with MSSQL sink)
builder.Host.UseSerilog((context, loggerConfig) =>
{
    loggerConfig
        .WriteTo.MSSqlServer(
            connectionString: serilogConnection,
            sinkOptions: new MSSqlServerSinkOptions { TableName = "Logs" }
        );
});

// Initialize LogEvac
builder.InitializeLogEvac(); // Uses "LoggingDb" connection string by default
// OR use a custom connection string name:
// builder.InitializeLogEvac("LoggingDb-Local");
```

### Step 2: Schedule the Cleanup Job

Before calling `app.Run()`, schedule the background job:

```csharp
var app = builder.Build();

// ... other middleware configurations ...

app.ScheduleLogEvacJob(); // Starts the recurring cleanup job
app.Run();
```

## 🔧 Configuration Options

The `LogEvacSettings` section controls LogEvac behavior:

| Setting | Type | Default | Description |
|---------|------|---------|-------------|
| `ApplicationName` | string | - | **Required.** Application name to match in Serilog logs |
| `Environment` | string | - | Environment name (e.g., "Production", "Staging") |
| `LoggingTable` | string | "LogEvents" | Name of the logging table in the database |
| `CheckingFrequencyInMinutes` | int | 0 | Cleanup frequency in minutes (overrides hours if > 0) |
| `CheckingFrequencyInHours` | int | 2 | Cleanup frequency in hours (used if minutes is 0) |
| `CleanUpDataOlderThanMinutesValue` | int | 0 | Delete logs older than X minutes (overrides all if > 0) |
| `CleanUpDataOlderThanDaysValue` | int | 0 | Delete logs older than X days (overrides weeks/months if > 0) |
| `CleanUpDataOlderThanWeeksValue` | int | 0 | Delete logs older than X weeks (overrides months if > 0) |
| `CleanUpDataOlderThanMonthsValue` | int | 1 | Delete logs older than X months (default) |

**Precedence Order** (highest to lowest):
1. `CleanUpDataOlderThanMinutesValue`
2. `CleanUpDataOlderThanDaysValue`
3. `CleanUpDataOlderThanWeeksValue`
4. `CleanUpDataOlderThanMonthsValue`

### Example: Run Cleanup Every 30 Minutes, Delete Logs Older Than 7 Days

```json
{
  "LogEvacSettings": {
    "ApplicationName": "MyApp",
    "Environment": "Production",
    "CheckingFrequencyInMinutes": 30,
    "CleanUpDataOlderThanDaysValue": 7
  }
}
```

## 🆘 Troubleshooting

### ❌ Scheduler Not Initialized

**Problem:** LogEvac services are not being initialized.

**Solutions:**
- Ensure `ConnectionStrings:LoggingDb` is configured correctly in `appsettings.json`
- Verify `LogEvacSettings:AppName` is not empty
- Confirm Hangfire is configured before calling `InitializeLogEvac()`

### ❌ No Logs Being Deleted

**Problem:** Cleanup job runs but doesn't delete logs.

**Solutions:**
- Check that the `ApplicationName` in settings matches the Serilog `ApplicationName` column values
- Verify the cleanup age criteria (minutes, days, weeks, or months) is set correctly
- Ensure the Serilog MSSQL sink has the `ApplicationName` column in the logs table
- Check Hangfire Dashboard to see job execution history and errors

### ❌ "Entity Framework" or "DbContext" Errors

**Problem:** Missing or misconfigured database context.

**Solutions:**
- Ensure `Microsoft.EntityFrameworkCore.SqlServer` is installed
- Verify connection string is valid and database exists
- Run `dotnet ef database update` if using migrations

### ❌ Connection String Not Found

**Problem:** Error referencing connection string key.

**Solutions:**
- Verify the connection string key exists in `appsettings.json`
- If using a custom key, ensure you pass it correctly: `builder.InitializeLogEvac("CustomKeyName")`
- Check for typos in connection string names

## 📝 License

This project was created by [JayMar921](https://jayharronabejar.vercel.app).

---

### Quick Reference

| Task | Command/Code |
|------|--------------|
| Install Dependencies | `Install-Package Hangfire.AspNetCore Hangfire.SqlServer Microsoft.EntityFrameworkCore Microsoft.EntityFrameworkCore.SqlServer Serilog Serilog.Sinks.MSSqlServer` |
| Register Services | `builder.InitializeLogEvac();` |
| Schedule Job | `app.ScheduleLogEvacJob();` |
| Use Custom DB | `builder.InitializeLogEvac("MyCustomDbKey");` |