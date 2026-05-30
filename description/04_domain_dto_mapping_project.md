# Domain Model и DTO Mapping

> В проекте предметная область описывает компьютеры организации, установленное программное обеспечение и лицензии.

## 1. Domain Models

### 1.1 Computer

```csharp
class Computer
{
    int Id;
    string Owner;
    string MacAddress;
    DateTime LastCheckDate;
    List<InstalledSoftware> Installations;
}
```

### 1.2 Software

```csharp
class Software
{
    int Id;
    string Name;
    LicenseType License;
    string SoftwareType;
}
```

### 1.3 InstalledSoftware

```csharp
class InstalledSoftware
{
    int Id;
    int ComputerId;
    int? SoftwareId;
    string? CustomSoftwareName;
    DateTime InstallDate;
}
```

### 1.4 LicenseType

```csharp
enum LicenseType
{
    Licensed,
    Free,
    Shareware,
    Trial
}
```

## 2. DTO

### ComputerDto

```json
{
  "id": 1,
  "owner": "Иванов И.И.",
  "macAddress": "00-11-22-33-44-55",
  "lastCheckDate": "2026-05-01"
}
```

### SoftwareDto

```json
{
  "id": 5,
  "name": "Microsoft Office",
  "license": "Licensed",
  "softwareType": "Office"
}
```

### InstalledSoftwareDto

```json
{
  "computerId": 1,
  "softwareId": 5,
  "installDate": "2026-05-01"
}
```

# Инварианты и ограничения данных

| Понятие | Ограничение |
|----------|------------|
| MAC-адрес | Должен быть уникальным для компьютера |
| Компьютер | Обязательно имеет владельца |
| ПО | Должно иметь название |
| Лицензия | Только значения LicenseType |
| Установка ПО | Привязана к конкретному компьютеру |

# Таблица маппинга

| Источник данных | Domain Model | DTO | Примечание |
|-----------------|-------------|-----|------------|
| Форма создания ПК | Computer | ComputerDto | Данные пользователя |
| Справочник ПО | Software | SoftwareDto | Выбор программы |
| Форма установки ПО | InstalledSoftware | InstalledSoftwareDto | Связь ПК и ПО |
| SQLite | Все сущности | DTO | Передача данных в представление |
