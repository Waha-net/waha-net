<a name="top"></a>
![.NET](https://img.shields.io/badge/.NET-9.0-512BD4)
[![language](https://img.shields.io/badge/language-C%23-239120)](https://learn.microsoft.com/ru-ru/dotnet/csharp/tour-of-csharp/overview)
![OS](https://img.shields.io/badge/OS-linux%2C%20windows%2C%20macOS-0078D4)
[![GitHub release](https://img.shields.io/github/v/release/waha-net/waha-net)](#)
[![Build & Publish NuGet to GitHub Registry](https://github.com/Waha-net/waha-net/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/Waha-net/waha-net/actions/workflows/dotnet.yml)

⭐ Star us on GitHub — it motivates us a lot!

# Waha Net
A .NET C# Library for [`WAHA (WhatsApp HTTP API)`](https://github.com/devlikeapro/waha)

### Installation
To install the `Waha` library, run the following command in your .NET project:

> dotnet add package Waha

### Usage
Below is a short example showing how to integrate Waha into your .NET projects to retrieve `WhatsApp chats`, once the session is started and the QR has been validated.

#### ASP .NET
```csharp
using Waha;

var builder = WebApplication.CreateBuilder(args);

//This method will look for "Waha" settings configuration section or connectionstring in your appsettings.json
//It also will use Waha default endpoint value ("localhost:3000") if can´t find a valid configuration
builder.AddWahaApiClient("Waha");

var app = builder.Build();
app.MapDefaultEndpoints();

app.MapGet("/chats", async (
  IWahaApiClient wahaApiClient, CancellationToken cancellationToken,
  [FromHeader] int limit = 5, [FromHeader] int offset = 0, [FromHeader] string sortBy = "", [FromHeader] string sortOrder = "")
{
  var sessions = await wahaApiClient.GetSessionsAsync(true, cancellationToken);
  var session = sessions.FirstOrDefault();
  if (session == null)
  {
      return Results.Json(new { Message = "No active session found." }, statusCode: StatusCodes.Status412PreconditionFailed);
  }
  var chats = await wahaApiClient.GetChatsAsync(session.Name, limit, offset, sortBy, sortOrder, cancellationToken);
  return Results.Ok(chats);
}).WithName("GetChats");
```

#### OTHERS APPS
```csharp
using Waha;

var wahaApiClient = new WahaApiClient(new HttpClient() { BaseAddress = WahaSettings.Default.Endpoint });
var sessions = await wahaApiClient.GetSessionsAsync(true, cancellationToken);
var session = sessions.FirstOrDefault();
if (session != null)
{
    var chats = await wahaApiClient.GetChatsAsync(session.Name, 10, 0, "", "", null);
    // Process the chats as needed...
}
```

### Contributing
We welcome and appreciate contributions from the community. You can open a pull request or report issues through our [GitHub Issues](https://github.com/Waha-net/waha-net/issues/). Please review our contribution guidelines for details on coding standards and development practices.

### Feedback & Support
For any questions, issues, or ideas, feel free to reach out via:

* [GitHub Issues](https://github.com/Waha-net/waha-net/issues)
  
Your feedback helps us make `Waha` library even better!
