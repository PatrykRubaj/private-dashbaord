# Dashboard
## Database Migrations
### To add a new migration
`dotnet ef migrations add MigrationName --project Core.DataAccess --startup-project Core`
### To update database
`dotnet ef database update --project Core.DataAccess --startup-project Core`