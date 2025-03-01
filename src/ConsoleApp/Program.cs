using System.Drawing;
using Waha;

class Program
{
    static async Task Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.Clear();

        Console.WriteLine("Initializing Waha API Client");
        IWahaApiClient wahaApiClient = new WahaApiClient(new HttpClient() { BaseAddress = new Uri("http://localhost:3000/") });

        var activeSessions = await wahaApiClient.GetSessionsAsync(false);
        Console.WriteLine($"Active sessions: {activeSessions.Count}");

        SessionShort activeSession = activeSessions.Count == 0
            ? await wahaApiClient.StartSessionAsync("default")
            : activeSessions.First();

        if (activeSession.Status == "SCAN_QR_CODE")
        {
            //var authQrCodeResponse = await wahaApiClient.GetAuthQrAsync(activeSession.Name);
            //TODO: Display QR code in console

            var authRequestCodeResponse = await wahaApiClient.RequestAuthCodeAsync(activeSession.Name, new AuthCodeRequest() { PhoneNumber = "17824095342", Method = "" });
            if (!authRequestCodeResponse.Success)
            {
                Console.WriteLine("Auth Code Error: " + authRequestCodeResponse.Message);
            }
        }

        var healthResponse = await wahaApiClient.CheckHealthAsync();
        Console.WriteLine("Server Status: " + healthResponse.Status);

        var session = await wahaApiClient.GetSessionAsync(activeSession.Name);
        var me = session.Me;
        Console.WriteLine($"Logged in as {me.PushName} ({me.Id})");

        await wahaApiClient.Profile
    }
}