![Language](https://badgen.net/badge/Language/C%23/purple)
![.NET](https://badgen.net/badge/.NET/.9)
![OS](https://badgen.net/badge/OS/linux%2C%20windows%2C%20macOS)
![License](https://badgen.net/github/license/waha-net/waha-net)
![Release](https://badgen.net/github/release/waha-net/waha-net)

![BuildStatus](https://github.com/Waha-net/waha-net/actions/workflows/dotnet.yml/badge.svg?branch=main)

⭐ Star us on GitHub — it motivates us a lot!
![License](https://badgen.net/github/stars/waha-net/waha-net)

# Waha Net
Waha Net is a .NET C# client library for interacting with [`WAHA (WhatsApp HTTP API)`](https://github.com/devlikeapro/waha). It simplifies the integration of WAHA services into your .NET applications.

## Installation
To install the `Waha` library, run the following command in your .NET project:

> dotnet add package Waha

Or, use the NuGet Package Manager in Visual Studio.

## Usage
This section demonstrates how to integrate Waha Net into your .NET projects to retrieve WhatsApp chats. This assumes that a WAHA session has been started and the QR code has been scanned and validated.

### Setting up WAHA Docker Container
First, ensure you have WAHA (WhatsApp HTTP API) running. Follow these steps to set it up as a Docker container:

**Prerequisite**: Ensure [Docker](https://docs.docker.com/get-docker/) is installed on your system.

```bash
docker pull devlikeapro/waha
```

Run the container:

```bash
docker run -it --rm -p 3000:3000/tcp --name waha devlikeapro/waha

# It prints logs and the last line must be
# WhatsApp HTTP API is running on: http://[::1]:3000
```

Open the link in your browser [http://localhost:3000/](http://localhost:3000/) and you'll see API documentation
(Swagger).

### ASP .NET

Here's a sample code snippet for an ASP.NET Core application that lists WhatsApp chats from the logged-in user:

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
