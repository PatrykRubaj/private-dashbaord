# Dashboard

Personal life dashboard API. A monolithic ASP.NET Core REST service for aggregating data from everyday sources and exposing useful endpoints (e.g. for iPhone Shortcuts).

## Database Migrations
### To add a new migration
`dotnet ef migrations add MigrationName --project Core.DataAccess --startup-project Core`
### To update database
`dotnet ef database update --project Core.DataAccess --startup-project Core`

## Features

### Sleep logging (iOS Shortcuts / AutoSleep)
Endpoints under `/SleepEntry` for recording nightly sleep data. Designed to be called from an iOS Shortcut that reads the AutoSleep export, so each night's metrics (hours, REM/deep, BPM, recharge, etc.) are stored and queryable by date range.

### Air quality monitoring (AirGradient sensors)
Periodically polls AirGradient indoor air quality sensors on the local network and stores the readings for analysis and graphing.
- Background service polls every active sensor at a configurable interval (`AirGradient:PollingIntervalSeconds` in `appsettings.json`).
- Endpoints under `/AirQuality`: latest reading, history (with optional `from`/`until` range), and on-demand fetch+log per sensor.

