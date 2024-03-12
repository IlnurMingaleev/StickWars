# Unity GoogleSheets Database
Инструмент для редактора Unity, призванный помочь в дизайне и импорте базы данных игры (настроек персонажей, оружия, предметов и т. п.) из таблиц Google.

# Руководство
## Установка
### с помощью Package Manager
Чтобы установить данный ассет через Unity Package Manager используйте адрес:
```
https://github.com/NorskaGames/UnityGoogleSheetsDatabase.git
```
![package-manager-setup](https://drive.google.com/uc?id=16GE1j46xtedQu88d6cfM2G4itvqxpU2f)

## Настройка таблицы

Создайте таблицу и убедитесь, что она доступна по ссылке:
![spreadsheet-design](https://drive.google.com/uc?id=12Zo-_fQFYK8n9ljWMkfWtwbYhUUCP7ks)

_**Подсказка:** Проектируйте базу данных как любую другую БД (придерживайтесь хотя бы 1-ой нормальной формы: избегайте вложенных списков)._
![db-design-practices](https://drive.google.com/uc?id=1cGzRClYvEsvtzYkAlZp_nDVymvRPsjS1)

## Настройка контейнера

Создайте класс DataContainer и произвольный набор Data-классов как в примере ниже:
```
using NorskaLib.GoogleSheetsDatabase;

[CreateAssetMenu(fileName = "DataContainer", menuName = "Custom/DataContainer")]
public class DataContainer : DataContainerBase
{
    [PageName("SomeSpreadsheetPage")]
    public List<ExampleData> ExampleData;
}

[System.Serializable]
public class ExampleData
{
    public string Id;

    public float SomeFloat;
    public int SomeInt;
    public string SomeString;
}
```
_**Важно!** Убедитесь, что имена переменных совпадают с именами столбцов в таблице._

Теперь вы можете создать ассет-контейнер и импортировать в него таблицу как показано на примере ниже:
![container-inspector](https://drive.google.com/uc?id=16Rg4NIyj5c8-Qjq5phW0konDMRMKNN21)
