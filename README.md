# Rent Gubakha Bot

Telegram-бот для выбора и аренды домов в Губахе. Проект работает на .NET 10,
получает обновления через long polling и при необходимости пишет события в
Google Sheets.

## Важное действие после утечки

В истории репозитория находились рабочий Telegram-токен, Google OAuth client
secret и OAuth refresh token внутри ZIP-архива. Удаление файлов в новом коммите
не делает старые значения безопасными.

1. Отзовите старый Telegram-токен через `@BotFather` и выпустите новый.
2. Удалите скомпрометированный OAuth client и его токены в Google Cloud Console.
3. Для Google Sheets создайте отдельный service account с минимальными правами и
   дайте его адресу доступ только к нужной таблице.
4. После ротации перепишите Git-историю с помощью `git filter-repo` или BFG и
   выполните согласованный force-push. Все клоны после этого нужно пересоздать.

## Конфигурация

Скопируйте `.env.example` в локальный файл `.env` только как памятку. Приложение
читает переменные непосредственно из окружения и не загружает `.env` само.

| Переменная | Обязательна | Назначение |
| --- | --- | --- |
| `TELEGRAM_BOT_TOKEN` | да | Новый токен Telegram-бота |
| `GOOGLE_SHEETS_SPREADSHEET_ID` | нет | Включает запись статистики в указанную таблицу |
| `GOOGLE_SHEETS_WORKSHEET` | нет | Лист таблицы, по умолчанию `Лист1` |
| `GOOGLE_CREDENTIALS_JSON` | нет | JSON service account, переданный через secret store |
| `GOOGLE_APPLICATION_CREDENTIALS` | нет | Путь к JSON service account / ADC; предпочтителен локально |

Если `GOOGLE_SHEETS_SPREADSHEET_ID` не задан, бот запускается без аналитики.
Если идентификатор задан, а Google credentials недоступны, приложение завершится
с ошибкой конфигурации вместо запуска в частично рабочем состоянии.

Пример запуска из PowerShell:

```powershell
$env:TELEGRAM_BOT_TOKEN = '<new-token>'
$env:GOOGLE_SHEETS_SPREADSHEET_ID = '<spreadsheet-id>'
$env:GOOGLE_APPLICATION_CREDENTIALS = 'C:\secrets\rent-bot-service-account.json'
dotnet run --configuration Release
```

## Сборка и контейнер

```powershell
dotnet restore
dotnet build --configuration Release --no-restore
dotnet run --project BotGubakha.Tests --configuration Release --no-build --no-restore
docker build -t rent-gubakha-bot .
```

Секреты передаются контейнеру только во время запуска. Не используйте `ARG`,
`ENV` в Dockerfile или `COPY` для реальных ключей.

```powershell
docker run --rm `
  -e TELEGRAM_BOT_TOKEN='<new-token>' `
  -e GOOGLE_SHEETS_SPREADSHEET_ID='<spreadsheet-id>' `
  -e GOOGLE_APPLICATION_CREDENTIALS='/run/secrets/google.json' `
  -v 'C:\secrets\rent-bot-service-account.json:/run/secrets/google.json:ro' `
  rent-gubakha-bot
```

Приложение корректно обрабатывает `Ctrl+C`, подтверждает callback-запросы,
продолжает polling после сетевых ошибок и не останавливает основной сценарий при
единичной ошибке записи статистики.
