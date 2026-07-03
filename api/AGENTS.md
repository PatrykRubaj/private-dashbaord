# Dashboard API description
This is an API project for my personal life dashboard. This dashboard should allow to track some habits formation e.g. weighlifting on specifix days. This dashboard should also expose useful endpoint for my iPhone Shortcuts application, e.g. an endpoint to log my sleep. This way I can aggregate data and do useful analysis of my sleep vs e.g. my weight.
This project should use SOLID principles when possible. It should be a boring and a simple monolythic API, but organized cleanly for future development.

# Potential features
## Air Quality monitoring and logging based on Air Gradient sensors:
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

# Potential features
## Air Quality monitoring and logging based on Air Gradient sensors:
- it needs to store IPs of the sensors in database somewhere
- store configuration for the refresh time of a sensor (database or maybe just appsettings.json)
- fetch data from sensor API evey N minutes
- save data in a table
- provide endpoint to fetch and log data on call
- provide endpoint with the most recent data
- provide endpoint for graph drawing (or maybe just return drawn image with plotted data?)
- avoid using hangfire for periodic sensor calls if possible and clean, bespoke implementation is possible

Each AirGradient sensor has an GET API `/measures/current`. It returns following response

```json
{
  "wifi": -46,
  "serialno": "ecda3b1eaaaf",
  "rco2": 447,
  "pm01": 3,
  "pm02": 7,
  "pm10": 8,
  "pm003Count": 442,
  "atmp": 25.87,
  "atmpCompensated": 24.47,
  "rhum": 43,
  "rhumCompensated": 49,
  "tvocIndex": 100,
  "tvocRaw": 33051,
  "noxIndex": 1,
  "noxRaw": 16307,
  "boot": 6,
  "bootCount": 6,
  "ledMode": "pm",
  "firmware": "3.1.3",
  "model": "I-9PSL",
  "monitorDisplayCompensatedValues": true
}
```

Here's a table which explains the values

| Properties                        | Type    | Explanation                                                                            |
|-----------------------------------|---------|----------------------------------------------------------------------------------------|
| `serialno`                        | String  | Serial Number of the monitor                                                           |
| `wifi`                            | Number  | WiFi signal strength                                                                   |
| `pm01`                            | Number  | PM1.0 in ug/m3 (atmospheric environment)                                               |
| `pm02`                            | Number  | PM2.5 in ug/m3 (atmospheric environment)                                               |
| `pm10`                            | Number  | PM10 in ug/m3 (atmospheric environment)                                                |
| `pm02Compensated`                 | Number  | PM2.5 in ug/m3 with correction applied (from fw version 3.1.4 onwards)                 |
| `pm01Standard`                    | Number  | PM1.0 in ug/m3 (standard particle)                                                     |
| `pm02Standard`                    | Number  | PM2.5 in ug/m3 (standard particle)                                                     |
| `pm10Standard`                    | Number  | PM10 in ug/m3 (standard particle)                                                      |
| `rco2`                            | Number  | CO2 in ppm                                                                             |
| `pm003Count`                      | Number  | Particle count 0.3um per dL                                                            |
| `pm005Count`                      | Number  | Particle count 0.5um per dL                                                            |
| `pm01Count`                       | Number  | Particle count 1.0um per dL                                                            |
| `pm02Count`                       | Number  | Particle count 2.5um per dL                                                            |
| `pm50Count`                       | Number  | Particle count 5.0um per dL (only for indoor monitor)                                  |
| `pm10Count`                       | Number  | Particle count 10um per dL (only for indoor monitor)                                   |
| `atmp`                            | Number  | Temperature in Degrees Celsius                                                         |
| `atmpCompensated`                 | Number  | Temperature in Degrees Celsius with correction applied                                 |
| `rhum`                            | Number  | Relative Humidity                                                                      |
| `rhumCompensated`                 | Number  | Relative Humidity with correction applied                                              |
| `tvocIndex`                       | Number  | Senisiron VOC Index                                                                    |
| `tvocRaw`                         | Number  | VOC raw value                                                                          |
| `noxIndex`                        | Number  | Senisirion NOx Index                                                                   |
| `noxRaw`                          | Number  | NOx raw value                                                                          |
| `boot`                            | Number  | Counts every measurement cycle. Low boot counts indicate restarts.                     |
| `bootCount`                       | Number  | Same as boot property. Required for Home Assistant compatability. (deprecated soon!)   |
| `ledMode`                         | String  | Current configuration of the LED mode                                                  |
| `firmware`                        | String  | Current firmware version                                                               |
| `model`                           | String  | Current model name                                                                     |

In my home these sensors have addresses:
- http://10.1.0.191 - bedroom
- http://10.1.0.117 - office

Each AirGradient sensor also has an GET API `/config`. It returns following response

```json 
{
  "country": "TH",
  "pmStandard": "ugm3",
  "ledBarMode": "pm",
  "abcDays": 7,
  "tvocLearningOffset": 12,
  "noxLearningOffset": 12,
  "mqttBrokerUrl": "",
  "temperatureUnit": "c",
  "configurationControl": "local",
  "postDataToAirGradient": true,
  "ledBarBrightness": 100,
  "displayBrightness": 100,
  "offlineMode": false,
  "model": "I-9PSL",
  "monitorDisplayCompensatedValues": true,
  "corrections": {
    "pm02": {
      "correctionAlgorithm": "epa_2021",
      "slr": {}
      }
    }
  }
}
```

Here's a table which explains the returned config values

| Properties                        | Description                                                      | Type    | Accepted Values                                                                                                                         | Example                                         |
|-----------------------------------|:-----------------------------------------------------------------|---------|-----------------------------------------------------------------------------------------------------------------------------------------|-------------------------------------------------|
| `country`                         | Country where the device is.                                     | String  | Country code as [ALPHA-2 notation](https://www.iban.com/country-codes)                                                                  | `{"country": "TH"}`                             |
| `model`                           | Hardware identifier (only GET).                                  | String  | I-9PSL-DE                                                                                                                               | `{"model": "I-9PSL-DE"}`                        |
| `pmStandard`                      | Particle matter standard used on the display.                    | String  | `ugm3`: ug/m3 <br> `us-aqi`: USAQI                                                                                                      | `{"pmStandard": "ugm3"}`                        |
| `ledBarMode`                      | Mode in which the led bar can be set.                            | String  | `co2`: LED bar displays CO2 <br>`pm`: LED bar displays PM <br>`off`: Turn off LED bar                                                   | `{"ledBarMode": "off"}`                         |
| `displayBrightness`               | Brightness of the Display.                                       | Number  | 0-100                                                                                                                                   | `{"displayBrightness": 50}`                     |
| `ledBarBrightness`                | Brightness of the LEDBar.                                        | Number  | 0-100                                                                                                                                   | `{"ledBarBrightness": 40}`                      |
| `abcDays`                         | Number of days for CO2 automatic baseline calibration.           | Number  | Maximum 200 days. Default 8 days.                                                                                                       | `{"abcDays": 8}`                                |
| `mqttBrokerUrl`                   | MQTT broker URL.                                                 | String  |                                                                                                                                         | `{"mqttBrokerUrl": "mqtt://192.168.0.18:1883"}` |
| `temperatureUnit`                 | Temperature unit shown on the display.                           | String  | `c` or `C`: Degree Celsius °C <br>`f` or `F`: Degree Fahrenheit °F                                                                      | `{"temperatureUnit": "c"}`                      |
| `configurationControl`            | The configuration source of the device.                          | String  | `both`: Accept local and cloud configuration <br>`local`: Accept only local configuration  <br>`cloud`: Accept only cloud configuration | `{"configurationControl": "both"}`              |
| `postDataToAirGradient`           | Send data to AirGradient cloud.                                  | Boolean | `true`: Enabled <br>`false`: Disabled                                                                                                   | `{"postDataToAirGradient": true}`               |
| `co2CalibrationRequested`         | Can be set to trigger a calibration.                             | Boolean | `true`: CO2 calibration (400ppm) will be triggered                                                                                      | `{"co2CalibrationRequested": true}`             |
| `ledBarTestRequested`             | Can be set to trigger a test.                                    | Boolean | `true` : LEDs will run test sequence                                                                                                    | `{"ledBarTestRequested": true}`                 |
| `noxLearningOffset`               | Set NOx learning gain offset.                                    | Number  | 0-720 (default 12)                                                                                                                      | `{"noxLearningOffset": 12}`                     |
| `tvocLearningOffset`              | Set VOC learning gain offset.                                    | Number  | 0-720 (default 12)                                                                                                                      | `{"tvocLearningOffset": 12}`                    |
| `offlineMode`                     | Set monitor to run without WiFi.                                 | Boolean |  `false`: Disabled (default) <br> `true`: Enabled                                                                                       | `{"offlineMode": true}`                         |
| `monitorDisplayCompensatedValues` | Set the display show the PM value with/without compensate  value (only on [3.1.9]()) | Boolean | `false`: Without compensate (default) <br> `true`: with compensate                                                                      | `{"monitorDisplayCompensatedValues": false }`   |
| `corrections`                     | Sets correction options to display and measurement values on local server response. (version >= [3.1.11]())    | Object |  _see corrections section_             | _see corrections section_                         |

Additionally AirGradient sensors have PUT API `/config`. It allows to modify settings for the sensors. So the properties from the table above can be set to different values.