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
                case "QR_CODE":
                    var authQrCodeResponse = await wahaApiClient.GetAuthQrAsync(activeSession.Name);
                    //TODO: Learn how to display QR code in Console Application
                    break;
                case "SMS":
                    var authSmsRequestCodeResponse = await wahaApiClient.RequestAuthCodeAsync(activeSession.Name, new AuthCodeRequest() { PhoneNumber = "17824095342", Method = "SMS" });
                    if (!string.IsNullOrWhiteSpace(authSmsRequestCodeResponse.Code))
                    {
                        Console.WriteLine("Auth Code Error: " + authSmsRequestCodeResponse.Code);
                    }
                    break;
                case "VOICE":
                    var authVoiceRequestCodeResponse = await wahaApiClient.RequestAuthCodeAsync(activeSession.Name, new AuthCodeRequest() { PhoneNumber = "17824095342", Method = "VOICE" });
                    if (!string.IsNullOrWhiteSpace(authVoiceRequestCodeResponse.Code))
                    {
                        Console.WriteLine("Auth Code Error: " + authVoiceRequestCodeResponse.Code);
                    }
                    break;
                case "":
                    var authRequestCodeResponse = await wahaApiClient.RequestAuthCodeAsync(activeSession.Name, new AuthCodeRequest() { PhoneNumber = "17824095342", Method = "" });
                    if (!string.IsNullOrWhiteSpace(authRequestCodeResponse.Code))
                    {
                        Console.WriteLine("Auth Code Error: " + authRequestCodeResponse.Code);
                    }
                    break;
            }
        }

        var session = await wahaApiClient.GetSessionAsync(activeSession.Name);
        Console.WriteLine($"Logged in as {session.User.PushName} ({session.User.Id})");

        var profile = await wahaApiClient.GetProfileAsync(session.Name);
        Console.WriteLine($"Profile: {profile.Name} ({profile.Id})");

        var chats = await wahaApiClient.GetChatsAsync(session.Name);
        foreach (var chat in chats)
        {
            Console.WriteLine($"Chat {chat.Name} with id '{chat.Id.Id}' @ {chat.Timestamp}");
        }

        Console.WriteLine("Select chat id");
        var chatId = Console.ReadLine();

        if(!string.IsNullOrWhiteSpace(chatId))
        {
            var messages = await wahaApiClient.GetChatMessagesAsync(session.Name, chatId);
            foreach (var message in messages)
            {
                Console.WriteLine($"Message @ {message.Timestamp} from {message.From} to {message.To}: {message.Body}");
            }
        }

        Console.ReadLine();
    }
}