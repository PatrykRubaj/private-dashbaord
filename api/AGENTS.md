# Dashboard API description
This is an API project for my personal life dashboard. This dashboard should allow to track some habits formation e.g. weighlifting on specifix days. This dashboard should also expose useful endpoint for my iPhone Shortcuts application, e.g. an endpoint to log my sleep. This way I can aggregate data and do useful analysis of my sleep vs e.g. my weight.
This project should use SOLID principles when possible. It should be a boring and a simple monolythic API, but organized cleanly for future development.

# Potential features
- Air Quality monitoring and logging based on Air Gradient sensors:
  - it needs to store IPs of the sensors in database somewhere
  - store configuration for the refresh time of a sensor (database or maybe just appsettings.json)
  - fetch data from sensor API evey N minutes
  - save data in a table
  - provide endpoint to fetch and log data on call
  - provide endpoint with the most recent data
  - provide endpoint for graph drawing (or maybe just return drawn image with plotted data?)


# Tech Stack
- ASP.NET Core and .NET 8
- PostgreSQL 17 for data storage
- podman for containers
- Trying to limit myself only to builin functionality of the ASP.NET Core
- REST API
- EF Core 8 for data access and database migrations
- xUnit.net for unit tests

# API Structure
- `Core` project contains the controllers and main API application and configuration
  + `Core/Controllers` contains ASP.NET Core controllers / API endpoints
- `Core.DataAcess` contains EF Core migrations and DataContext setup
- `Core.Model` contains the model
  + `Core.Model/DTO` directory contains DTOs
- `Core.Tests` contains unit tests

## Commands
- Build: `dotnet_build` tool call
- Test: `dotnet_test` tool call

## Workflow Rules
- ALWAYS work on the current branch, unless current branch is main, master or develop. Ask the user if he really wants to continue working on such branch or suggest creatintg a new feature branch
- Run `dotnet_build` after compleating implementation
- Keep commits atomic - one logical change per commit
- Don't use comments, code should be readable on its own
- Don't use XML docummentation comments
- For development use user "admin@admin.com" with password "Admin123$"

### Patterns I DON'T Use (Never Suggest)
- AutoMapper (write explicit mappings)
- Exceptions for business logic errors
- Stored procedures