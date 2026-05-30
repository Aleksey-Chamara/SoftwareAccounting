# Контракты взаимодействия и интерфейсы системы

> Взаимодействие компонентов строится на основе MVC-архитектуры ASP.NET Core. Контроллеры выступают посредниками между пользовательским интерфейсом, бизнес-логикой и базой данных.

## 1. Backend Interfaces (C#)

Интерфейсы определяют границы между слоями приложения и позволяют тестировать бизнес-логику независимо от базы данных и пользовательского интерфейса.

### 1.1 Репозиторий компьютеров (Repository Pattern)

```csharp
public interface IComputerRepository
{
    Task<IEnumerable<Computer>> GetAllAsync();
    Task<Computer?> GetByIdAsync(int id);
    Task AddAsync(Computer computer);
    Task UpdateAsync(Computer computer);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
```

### 1.2 Репозиторий программного обеспечения

```csharp
public interface ISoftwareRepository
{
    Task<IEnumerable<Software>> GetAllAsync();
    Task<Software?> GetByIdAsync(int id);
    Task AddAsync(Software software);
    Task UpdateAsync(Software software);
    Task DeleteAsync(int id);
}
```

### 1.3 Управление установленным ПО

```csharp
public interface IInstallationService
{
    Task AddSoftwareAsync(int computerId, int softwareId, DateTime installDate);
    Task AddCustomSoftwareAsync(int computerId, string customName, DateTime installDate);
    Task RemoveSoftwareAsync(int installationId);
    Task<IEnumerable<InstalledSoftware>> GetInstalledSoftwareAsync(int computerId);
}
```

## 2. Типы ошибок

```csharp
public enum ApplicationError
{
    ComputerNotFound,
    SoftwareNotFound,
    InstallationNotFound,
    InvalidMacAddress,
    DuplicateComputer,
    UnauthorizedAccess,
    ValidationFailed,
    DatabaseError,
    UnknownError
}
```

## 3. Frontend Contracts (MVC Views)

### Computer View Model

```csharp
public class ComputerViewModel
{
    public int Id { get; set; }
    public string Owner { get; set; }
    public string MacAddress { get; set; }
    public DateTime LastCheckDate { get; set; }
    public int InstalledSoftwareCount { get; set; }
}
```

## 4. Правила взаимодействия

1. Пользователь работает через MVC Views.
2. Контроллеры вызывают сервисы.
3. Сервисы работают с репозиториями.
4. Репозитории используют Entity Framework Core.
5. Все данные хранятся в SQLite.
6. Только Admin может изменять данные.
