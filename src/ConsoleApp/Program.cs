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
        Console.WriteLine($"Active session: {activeSession.Name} ({activeSession.Status})");


        if (activeSession.Status == "SCAN_QR_CODE")
        {
            string authOptions = "QR_CODE";

            switch (authOptions)
            {
                case "QRCODE":
                    var authQrCodeResponse = await wahaApiClient.GetAuthQrAsync(activeSession.Name);
                    //TODO: Learn how to display QR code in Console Application
                    break;
                case "SMS":
                    var authSmsRequestCodeResponse = await wahaApiClient.RequestAuthCodeAsync(activeSession.Name, new AuthCodeRequest() { PhoneNumber = "17824095342", Method = "SMS" });
                    if (!authSmsRequestCodeResponse.Success)
                    {
                        Console.WriteLine("Auth Code Error: " + authSmsRequestCodeResponse.Message);
                    }
                    break;
                case "VOICE":
                    var authVoiceRequestCodeResponse = await wahaApiClient.RequestAuthCodeAsync(activeSession.Name, new AuthCodeRequest() { PhoneNumber = "17824095342", Method = "VOICE" });
                    if (!authVoiceRequestCodeResponse.Success)
                    {
                        Console.WriteLine("Auth Code Error: " + authVoiceRequestCodeResponse.Message);
                    }
                    break;
                case "":
                    var authRequestCodeResponse = await wahaApiClient.RequestAuthCodeAsync(activeSession.Name, new AuthCodeRequest() { PhoneNumber = "17824095342", Method = "" });
                    if (!authRequestCodeResponse.Success)
                    {
                        Console.WriteLine("Auth Code Error: " + authRequestCodeResponse.Message);
                    }
                    break;
            }
        }

        var session = await wahaApiClient.GetSessionAsync(activeSession.Name);
        var me = session.Me;
        Console.WriteLine($"Logged in as {me.PushName} ({me.Id})");

        var profile = await wahaApiClient.GetProfileAsync(session.Name);
        Console.WriteLine($"Profile: {profile.Name} ({profile.Id})");

        var chats = await wahaApiClient.GetChatsAsync(session.Name);

        Console.ReadLine();
    }
}