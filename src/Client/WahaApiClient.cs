using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace Waha
{
    /// <summary>
    /// Provides an implementation of the <see cref="IWahaApiClient"/> interface for interacting with the Waha API.
    /// This client facilitates communication with various endpoints of the Waha API, allowing operations such as
    /// session management, message sending, contact handling, group management, and more.
    /// 
    /// The client uses an <see cref="HttpClient"/> to send HTTP requests and handle responses, ensuring proper
    /// serialization and deserialization of request and response payloads. Each method corresponds directly to
    /// an HTTP endpoint in the Waha API, making it easy to perform actions programmatically.
    /// 
    /// Example usage:
    /// <code>
    /// var apiClient = new WahaApiClient(new HttpClient { BaseAddress = new Uri("https://localhost:3000") });
    /// var sessions = await apiClient.GetSessionsAsync();
    /// </code>
    /// </summary>
    public class WahaApiClient : IWahaApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<WahaApiClient> _logger;

        public WahaApiClient(HttpClient httpClient, ILogger<WahaApiClient>? logger = null)
        {
            _httpClient = httpClient;
            _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<WahaApiClient>.Instance;

            _logger.LogDebug("WahaApiClient initialized with base address: {BaseAddress}", httpClient.BaseAddress);
        }

        #region [ SESSIONS ]

        public async Task<IReadOnlyList<SessionShort>> GetSessionsAsync(bool all, CancellationToken cancellationToken)
        {
            string url = $"/api/sessions";
            url = QueryHelpers.AddQueryString(url, new Dictionary<string, string>
            {
                ["all"] = all.ToString()
            });
            HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<List<SessionShort>>(cancellationToken: cancellationToken);
            return result ?? new List<SessionShort>();
        }

        public async Task<Session> CreateSessionAsync(SessionCreateRequest createRequest, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/api/sessions", createRequest, cancellationToken);
            response.EnsureSuccessStatusCode();

            var session = await response.Content.ReadFromJsonAsync<Session>(cancellationToken: cancellationToken);
            return session ?? throw new InvalidOperationException("Session creation returned null");
        }

        public async Task<Session> GetSessionAsync(string sessionName, CancellationToken cancellationToken)
        {
            string url = $"/api/sessions/{sessionName}";
            HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var session = await response.Content.ReadFromJsonAsync<Session>(cancellationToken: cancellationToken);
            return session ?? throw new InvalidOperationException($"Session '{sessionName}' not found or returned null");
        }

        public async Task<Session> UpdateSessionAsync(string sessionName, SessionUpdateRequest updateRequest, CancellationToken cancellationToken)
        {
            string url = $"/api/sessions/{sessionName}";
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync(url, updateRequest, cancellationToken);
            response.EnsureSuccessStatusCode();

            var session = await response.Content.ReadFromJsonAsync<Session>(cancellationToken: cancellationToken);
            return session ?? throw new InvalidOperationException($"Session '{sessionName}' update returned null");
        }

        public async Task DeleteSessionAsync(string sessionName, CancellationToken cancellationToken)
        {
            string url = $"/api/sessions/{sessionName}";
            HttpResponseMessage response = await _httpClient.DeleteAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task<SessionUser> GetMySessionUserAsync(string sessionName, CancellationToken cancellationToken)
        {
            string url = $"/api/sessions/{sessionName}/me";
            HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var me = await response.Content.ReadFromJsonAsync<SessionUser>(cancellationToken: cancellationToken);
            return me ?? throw new InvalidOperationException($"Fetching 'me' from session '{sessionName}' returned null");
        }

        public async Task<SessionShort> StartSessionAsync(string sessionName, CancellationToken cancellationToken)
        {
            string url = $"/api/sessions/{sessionName}/start";
            HttpResponseMessage response = await _httpClient.PostAsync(url, null, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<SessionShort>(cancellationToken: cancellationToken);
            return result ?? throw new InvalidOperationException($"Start session '{sessionName}' returned null");
        }

        public async Task<SessionShort> StopSessionAsync(string sessionName, CancellationToken cancellationToken)
        {
            string url = $"/api/sessions/{sessionName}/stop";
            HttpResponseMessage response = await _httpClient.PostAsync(url, null, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<SessionShort>(cancellationToken: cancellationToken);
            return result ?? throw new InvalidOperationException($"Stop session '{sessionName}' returned null");
        }

        public async Task<SessionShort> LogoutSessionAsync(string sessionName, CancellationToken cancellationToken)
        {
            string url = $"/api/sessions/{sessionName}/logout";
            HttpResponseMessage response = await _httpClient.PostAsync(url, null, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<SessionShort>(cancellationToken: cancellationToken);
            return result ?? throw new InvalidOperationException($"Logout session '{sessionName}' returned null");
        }

        public async Task<SessionShort> RestartSessionAsync(string sessionName, CancellationToken cancellationToken)
        {
            string url = $"/api/sessions/{sessionName}/restart";
            HttpResponseMessage response = await _httpClient.PostAsync(url, null, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<SessionShort>(cancellationToken: cancellationToken);
            return result ?? throw new InvalidOperationException($"Restart session '{sessionName}' returned null");
        }

        #endregion

        #region [ AUTH ]

        public async Task<AuthQrResponse> GetAuthQrAsync(string sessionName, string format, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting auth via QR code for {SessionName} session", sessionName);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var url = $"/api/{sessionName}/auth/qr";
                url = QueryHelpers.AddQueryString(url, new Dictionary<string, string>
                {
                    ["format"] = format
                });
                _logger.LogDebug("Sending GET request to {Url}", url);

                var response = await _httpClient.GetAsync(url, cancellationToken);
                _logger.LogDebug("Received response with status code {StatusCode} from GET {Url}", response.StatusCode, url);

                response.EnsureSuccessStatusCode();

                AuthQrResponse result = new AuthQrResponse() 
                { 
                    QrCodeImageStream = await response.Content.ReadAsStreamAsync(cancellationToken) 
                };

                _logger.LogInformation("Got QRCode auth image for {SessionName} session in {ElapsedMs}ms with lenght of {QrCodeLenght}", sessionName, stopwatch.ElapsedMilliseconds, result.QrCodeImageStream.Length);

                return result ?? throw new InvalidOperationException("Unable to deserialize QR response.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start auth via QR code for {SessionName} after {ElapsedMs}ms", sessionName, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        public async Task<AuthRequestCodeResponse> RequestAuthCodeAsync(string sessionName, AuthCodeRequest request, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsJsonAsync($"/api/{sessionName}/auth/request-code", request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var requestCodeResponse = await response.Content.ReadFromJsonAsync<AuthRequestCodeResponse>(cancellationToken: cancellationToken);
            return requestCodeResponse ?? throw new InvalidOperationException("Unable to deserialize request code response.");
        }

        #endregion

        #region [ PROFILE ]

        public async Task<Profile> GetProfileAsync(string sessionName, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting profile for {SessionName} session", sessionName);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var url = $"/api/{sessionName}/profile";
                _logger.LogDebug("Sending GET request to {Url}", url);

                var response = await _httpClient.GetAsync(url, cancellationToken);
                _logger.LogDebug("Received response with status code {StatusCode} from GET {Url}", response.StatusCode, url);

                response.EnsureSuccessStatusCode();

                var profile = await response.Content.ReadFromJsonAsync<Profile>(cancellationToken: cancellationToken);
                _logger.LogInformation("Got profile for {SessionName} session in {ElapsedMs}ms", sessionName, stopwatch.ElapsedMilliseconds);

                return profile ?? throw new InvalidOperationException("Unable to deserialize profile response.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get profile for {SessionName} after {ElapsedMs}ms", sessionName, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        public async Task<bool> UpdateProfileNameAsync(string sessionName, string name, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating profile name for {SessionName} session to '{Name}'", sessionName, name);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var url = $"/api/{sessionName}/profile/name";
                _logger.LogDebug("Sending PUT request to {Url}", url);

                var content = new { name };
                var response = await _httpClient.PutAsJsonAsync(url, content, cancellationToken);
                _logger.LogDebug("Received response with status code {StatusCode} from PUT {Url}", response.StatusCode, url);

                response.EnsureSuccessStatusCode();

                _logger.LogInformation("Updated profile name for {SessionName} session in {ElapsedMs}ms", sessionName, stopwatch.ElapsedMilliseconds);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update profile name for {SessionName} after {ElapsedMs}ms", sessionName, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        public async Task<bool> UpdateProfileAboutAsync(string sessionName, string about, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating profile about for {SessionName} session", sessionName);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var url = $"/api/{sessionName}/profile/about";
                _logger.LogDebug("Sending PUT request to {Url}", url);

                var content = new { about };
                var response = await _httpClient.PutAsJsonAsync(url, content, cancellationToken);
                _logger.LogDebug("Received response with status code {StatusCode} from PUT {Url}", response.StatusCode, url);

                response.EnsureSuccessStatusCode();

                _logger.LogInformation("Updated profile about for {SessionName} session in {ElapsedMs}ms", sessionName, stopwatch.ElapsedMilliseconds);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update profile about for {SessionName} after {ElapsedMs}ms", sessionName, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        public async Task<bool> UpdateProfilePictureAsync(string sessionName, UpdateProfilePictureRequest updateProfilePictureRequest, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating profile picture for {SessionName} session", sessionName);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var url = $"/api/{sessionName}/profile/picture";
                _logger.LogDebug("Sending PUT request to {Url}", url);

                var response = await _httpClient.PutAsJsonAsync(url, updateProfilePictureRequest, cancellationToken);
                _logger.LogDebug("Received response with status code {StatusCode} from PUT {Url}", response.StatusCode, url);

                response.EnsureSuccessStatusCode();

                _logger.LogInformation("Updated profile picture for {SessionName} session in {ElapsedMs}ms", sessionName, stopwatch.ElapsedMilliseconds);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update profile picture for {SessionName} after {ElapsedMs}ms", sessionName, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        #endregion

        #region [ SCREENSHOT ]

        public async Task<byte[]> GetScreenshotAsync(string session, CancellationToken cancellationToken)
        {
            var url = $"/api/screenshot?session={session}";
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync(cancellationToken);
        }

        #endregion

        #region [ CHATTING ]

        public async Task<Message> SendTextAsync(SendTextRequest request, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/sendText", request, cancellationToken);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<Message>(cancellationToken: cancellationToken))
                   ?? throw new InvalidOperationException("SendText returned null");
        }

        public async Task<Message> SendImageAsync(SendImageRequest request, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/sendImage", request, cancellationToken);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<Message>(cancellationToken: cancellationToken))
                   ?? throw new InvalidOperationException("SendImage returned null");
        }

        public async Task<Message> SendFileAsync(SendFileRequest request, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/sendFile", request, cancellationToken);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<Message>(cancellationToken: cancellationToken))
                   ?? throw new InvalidOperationException("SendFile returned null");
        }

        public async Task<Message> SendVoiceAsync(SendVoiceRequest request, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/sendVoice", request, cancellationToken);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<Message>(cancellationToken: cancellationToken))
                   ?? throw new InvalidOperationException("SendVoice returned null");
        }

        public async Task<Message> SendVideoAsync(SendVideoRequest request, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/sendVideo", request, cancellationToken);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<Message>(cancellationToken: cancellationToken))
                   ?? throw new InvalidOperationException("SendVideo returned null");
        }

        public async Task<Message> SendButtonsAsync(SendButtonsRequest request, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/sendButtons", request, cancellationToken);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<Message>(cancellationToken: cancellationToken))
                   ?? throw new InvalidOperationException("SendButtons returned null");
        }

        public async Task<Message> ForwardMessageAsync(ForwardMessageRequest request, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/forwardMessage", request, cancellationToken);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<Message>(cancellationToken: cancellationToken))
                   ?? throw new InvalidOperationException("ForwardMessage returned null");
        }

        public async Task SendSeenAsync(SendSeenRequest request, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/sendSeen", request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task StartTypingAsync(ChatRequest request, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/startTyping", request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task StopTypingAsync(ChatRequest request, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/stopTyping", request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task<Message> SetReactionAsync(ReactionRequest request, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PutAsJsonAsync("/api/reaction", request, cancellationToken);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<Message>(cancellationToken: cancellationToken))
                   ?? throw new InvalidOperationException("SetReaction returned null");
        }

        public async Task SetStarAsync(StarRequest request, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PutAsJsonAsync("/api/star", request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task<Message> SendPollAsync(SendPollRequest request, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/sendPoll", request, cancellationToken);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<Message>(cancellationToken: cancellationToken))
                   ?? throw new InvalidOperationException("SendPoll returned null");
        }

        public async Task<Message> SendLocationAsync(SendLocationRequest request, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/sendLocation", request, cancellationToken);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<Message>(cancellationToken: cancellationToken))
                   ?? throw new InvalidOperationException("SendLocation returned null");
        }

        public async Task<Message> SendLinkPreviewAsync(SendLinkPreviewRequest request, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/sendLinkPreview", request, cancellationToken);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<Message>(cancellationToken: cancellationToken))
                   ?? throw new InvalidOperationException("SendLinkPreview returned null");
        }

        public async Task<Message> SendContactVcardAsync(SendContactVcardRequest request, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/sendContactVcard", request, cancellationToken);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<Message>(cancellationToken: cancellationToken))
                   ?? throw new InvalidOperationException("SendContactVcard returned null");
        }

        #endregion

        #region [ CHANNELS ]

        public async Task<IReadOnlyList<Channel>> SearchChannelsByViewAsync(string session, ChannelSearchByViewRequest request, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/channels/search/by-view";
            var response = await _httpClient.PostAsJsonAsync(url, request, cancellationToken);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<List<Channel>>(cancellationToken: cancellationToken))
                   ?? new List<Channel>();
        }

        public async Task<IReadOnlyList<Channel>> SearchChannelsByTextAsync(string session, ChannelSearchByTextRequest request, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/channels/search/by-text";
            var response = await _httpClient.PostAsJsonAsync(url, request, cancellationToken);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<List<Channel>>(cancellationToken: cancellationToken))
                   ?? new List<Channel>();
        }

        public async Task<IReadOnlyList<object>> GetChannelSearchViewsAsync(string session, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/channels/search/views";
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<List<object>>(cancellationToken: cancellationToken))
                   ?? new List<object>();
        }

        public async Task<IReadOnlyList<object>> GetChannelSearchCountriesAsync(string session, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/channels/search/countries";
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<List<object>>(cancellationToken: cancellationToken))
                   ?? new List<object>();
        }

        public async Task<IReadOnlyList<object>> GetChannelSearchCategoriesAsync(string session, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/channels/search/categories";
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<List<object>>(cancellationToken: cancellationToken))
                   ?? new List<object>();
        }

        public async Task<IReadOnlyList<ChannelMessage>> PreviewChannelMessagesAsync(string session, string id, bool downloadMedia, int limit, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/channels/{id}/messages/preview?downloadMedia={downloadMedia}&limit={limit}";
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<List<ChannelMessage>>(cancellationToken: cancellationToken))
                   ?? new List<ChannelMessage>();
        }

        public async Task<IReadOnlyList<Channel>> GetChannelsAsync(string session, string? role, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/channels";
            if (!string.IsNullOrEmpty(role)) url += $"?role={role}";

            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<List<Channel>>(cancellationToken: cancellationToken))
                   ?? new List<Channel>();
        }

        public async Task<Channel> CreateChannelAsync(string session, CreateChannelRequest createChannelRequest, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/channels";
            var response = await _httpClient.PostAsJsonAsync(url, createChannelRequest, cancellationToken);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<Channel>(cancellationToken: cancellationToken))
                   ?? throw new InvalidOperationException("CreateChannel returned null");
        }

        public async Task DeleteChannelAsync(string session, string channelId, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/channels/{channelId}";
            var response = await _httpClient.DeleteAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task<Channel> GetChannelAsync(string session, string channelIdOrInvite, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/channels/{channelIdOrInvite}";
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<Channel>(cancellationToken: cancellationToken))
                   ?? throw new InvalidOperationException("GetChannel returned null");
        }

        public async Task FollowChannelAsync(string session, string channelId, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/channels/{channelId}/follow";
            var response = await _httpClient.PostAsync(url, null, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task UnfollowChannelAsync(string session, string channelId, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/channels/{channelId}/unfollow";
            var response = await _httpClient.PostAsync(url, null, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task MuteChannelAsync(string session, string channelId, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/channels/{channelId}/mute";
            var response = await _httpClient.PostAsync(url, null, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task UnmuteChannelAsync(string session, string channelId, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/channels/{channelId}/unmute";
            var response = await _httpClient.PostAsync(url, null, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        #endregion

        #region [ STATUS ]

        public async Task SendTextStatusAsync(string session, TextStatusRequest request, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/status/text";
            var response = await _httpClient.PostAsJsonAsync(url, request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task SendImageStatusAsync(string session, ImageStatusRequest request, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/status/image";
            var response = await _httpClient.PostAsJsonAsync(url, request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task SendVoiceStatusAsync(string session, VoiceStatusRequest request, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/status/voice";
            var response = await _httpClient.PostAsJsonAsync(url, request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task SendVideoStatusAsync(string session, VideoStatusRequest request, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/status/video";
            var response = await _httpClient.PostAsJsonAsync(url, request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteStatusAsync(string session, DeleteStatusRequest request, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/status/delete";
            var response = await _httpClient.PostAsJsonAsync(url, request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        #endregion

        #region [ CHATS ]

        public async Task<IReadOnlyList<Chat>> GetChatsAsync(string session, int limit, int offset, string sortBy, string sortOrder, CancellationToken cancellationToken)
        {
            var queryStringParameters = new Dictionary<string, string>
            {
                ["limit"] = limit.ToString(),
                ["offset"] = offset.ToString()
            };
            if(!string.IsNullOrWhiteSpace(sortBy))
                queryStringParameters.Add("sortBy", sortBy);
            if (!string.IsNullOrWhiteSpace(sortOrder))
                queryStringParameters.Add("sortOrder", sortOrder);

            var url = QueryHelpers.AddQueryString($"/api/{session}/chats", queryStringParameters);

            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var chats = await response.Content.ReadFromJsonAsync<List<Chat>>(cancellationToken: cancellationToken);
            return chats ?? new List<Chat>();
        }

        public async Task<IReadOnlyList<ChatOverview>> GetChatsOverviewAsync(string session, int limit, int offset, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/chats/overview";
            url = QueryHelpers.AddQueryString(url, new Dictionary<string, string>
            {
                ["limit"] = limit.ToString(),
                ["offset"] = offset.ToString()
            });
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var overview = await response.Content.ReadFromJsonAsync<List<ChatOverview>>(cancellationToken: cancellationToken);
            return overview ?? new List<ChatOverview>();
        }

        public async Task DeleteChatAsync(string session, string chatId, CancellationToken cancellationToken)
        {
            var response = await _httpClient.DeleteAsync($"/api/{session}/chats/{chatId}", cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task<ChatPicture> GetChatPictureAsync(string session, string chatId, bool refresh, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/chats/{chatId}/picture";
            url = QueryHelpers.AddQueryString(url, new Dictionary<string, string>
            {
                ["refresh"] = refresh.ToString()
            });
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var picture = await response.Content.ReadFromJsonAsync<ChatPicture>(cancellationToken: cancellationToken);
            return picture ?? throw new InvalidOperationException("Unable to deserialize chat picture.");
        }

        public async Task<IReadOnlyList<ChatMessage>> GetChatMessagesAsync(string session, string chatId, int limit, int offset, string filterTimestampLte, string filterTimestampGte, bool? filterOnlyMyMessages, bool downloadMedia, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/chats/{chatId}/messages";
            url = QueryHelpers.AddQueryString(url, new Dictionary<string, string>
            {
                ["limit"] = limit.ToString(),
                ["offset"] = offset.ToString(),
                ["downloadMedia"] = downloadMedia.ToString().ToLower()
            });
            if(filterOnlyMyMessages.HasValue)
                url = QueryHelpers.AddQueryString(url, "filter.fromMe", filterOnlyMyMessages.Value.ToString().ToLower());
            if (!string.IsNullOrWhiteSpace(filterTimestampLte))
                url = QueryHelpers.AddQueryString(url, "filter.timestamp.lte", filterTimestampLte);
            if (!string.IsNullOrWhiteSpace(filterTimestampGte))
                url = QueryHelpers.AddQueryString(url, "filter.timestamp.gte", filterTimestampGte);

            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var messages = await response.Content.ReadFromJsonAsync<List<ChatMessage>>(cancellationToken: cancellationToken);
            return messages ?? new List<ChatMessage>();
        }

        public async Task<ChatMessage> SendChatMessageAsync(string session, string chatId, CreateChatMessageRequest request, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsJsonAsync($"/api/{session}/chats/{chatId}/messages", request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var message = await response.Content.ReadFromJsonAsync<ChatMessage>(cancellationToken: cancellationToken);
            return message ?? throw new InvalidOperationException("Unable to deserialize created message.");
        }

        public async Task ClearChatMessagesAsync(string session, string chatId, CancellationToken cancellationToken)
        {
            var response = await _httpClient.DeleteAsync($"/api/{session}/chats/{chatId}/messages", cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task<ChatMessage> GetChatMessageAsync(string session, string chatId, string messageId, CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync($"/api/{session}/chats/{chatId}/messages/{messageId}", cancellationToken);
            response.EnsureSuccessStatusCode();

            var message = await response.Content.ReadFromJsonAsync<ChatMessage>(cancellationToken: cancellationToken);
            return message ?? throw new InvalidOperationException("Unable to deserialize chat message.");
        }

        public async Task DeleteChatMessageAsync(string session, string chatId, string messageId, CancellationToken cancellationToken)
        {
            var response = await _httpClient.DeleteAsync($"/api/{session}/chats/{chatId}/messages/{messageId}", cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task<ChatMessage> EditChatMessageAsync(string session, string chatId, string messageId, EditChatMessageRequest request, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/{session}/chats/{chatId}/messages/{messageId}", request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var edited = await response.Content.ReadFromJsonAsync<ChatMessage>(cancellationToken: cancellationToken);
            return edited ?? throw new InvalidOperationException("Unable to deserialize edited message.");
        }

        public async Task PinChatMessageAsync(string session, string chatId, string messageId, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsync($"/api/{session}/chats/{chatId}/messages/{messageId}/pin", content: null, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task UnpinChatMessageAsync(string session, string chatId, string messageId, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsync($"/api/{session}/chats/{chatId}/messages/{messageId}/unpin", content: null, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task ArchiveChatAsync(string session, string chatId, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsync($"/api/{session}/chats/{chatId}/archive", content: null, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task UnarchiveChatAsync(string session, string chatId, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsync($"/api/{session}/chats/{chatId}/unarchive", content: null, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task UnreadChatAsync(string session, string chatId, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsync($"/api/{session}/chats/{chatId}/unread", content: null, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        #endregion

        #region [ CONTACTS ]

        public async Task<IReadOnlyList<Contact>> GetAllContactsAsync(string session, int limit, int offset, string sortAsc, string sortOrder, CancellationToken cancellationToken)
        {
            var url = $"/api/contacts/all";
            url = QueryHelpers.AddQueryString(url, new Dictionary<string, string>
            {
                ["session"] = session,
                ["limit"] = limit.ToString(),
                ["offset"] = offset.ToString(),
            });
            if (!string.IsNullOrWhiteSpace(sortAsc))
                url = QueryHelpers.AddQueryString(url, "sortAsc", sortAsc);
            if (!string.IsNullOrWhiteSpace(sortOrder))
                url = QueryHelpers.AddQueryString(url, "sortOrder", sortOrder);

            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            return (await response.Content.ReadFromJsonAsync<List<Contact>>(cancellationToken: cancellationToken))
                   ?? new List<Contact>();
        }

        public async Task<object?> GetContactAsync(string session, string contactId, CancellationToken cancellationToken)
        {
            var url = $"/api/contacts?contactId={contactId}&session={session}";
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<object>(cancellationToken: cancellationToken);
        }

        public async Task<NumberExistResult> CheckContactExistsAsync(string session, string phone, CancellationToken cancellationToken)
        {
            var url = $"/api/contacts/check-exists?phone={phone}&session={session}";
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<NumberExistResult>(cancellationToken: cancellationToken))
                   ?? throw new InvalidOperationException("CheckContactExists returned null");
        }

        public async Task<string?> GetContactAboutAsync(string session, string contactId, CancellationToken cancellationToken)
        {
            var url = $"/api/contacts/about?contactId={contactId}&session={session}";
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync(cancellationToken);
        }

        public async Task<string?> GetContactProfilePictureAsync(string session, string contactId, bool refresh, CancellationToken cancellationToken)
        {
            var url = $"/api/contacts/profile-picture?contactId={contactId}&refresh={refresh}&session={session}";
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync(cancellationToken);
        }

        public async Task BlockContactAsync(string session, string contactId, CancellationToken cancellationToken)
        {
            var body = new { contactId, session };
            var response = await _httpClient.PostAsJsonAsync("/api/contacts/block", body, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task UnblockContactAsync(string session, string contactId, CancellationToken cancellationToken)
        {
            var body = new { contactId, session };
            var response = await _httpClient.PostAsJsonAsync("/api/contacts/unblock", body, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        #endregion

        #region [ GROUPS ]

        public async Task<Group> CreateGroupAsync(string session, CreateGroupRequest request, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/groups";
            var response = await _httpClient.PostAsJsonAsync(url, request, cancellationToken);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<Group>(cancellationToken: cancellationToken))
                   ?? throw new InvalidOperationException("CreateGroup returned null");
        }

        public async Task<IReadOnlyList<Group>> GetGroupsAsync(string session, bool? sortAsc, string? sortBy, int? limit, int? offset, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/groups";
            if (sortBy != null) url += $"?sortBy={sortBy}";
            if (sortAsc.HasValue) url += (sortBy == null ? "?" : "&") + $"sortOrder={(sortAsc.Value ? "asc" : "desc")}";
            if (limit.HasValue) url += (sortBy == null && !sortAsc.HasValue ? "?" : "&") + $"limit={limit.Value}";
            if (offset.HasValue) url += "&offset=" + offset.Value;

            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<Dictionary<string, Group>>(cancellationToken: cancellationToken))?.Values.ToList()
                   ?? new List<Group>();
        }

        public async Task<object?> GetGroupJoinInfoAsync(string session, string code, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/groups/join-info?code={code}";
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<object>(cancellationToken: cancellationToken);
        }

        public async Task<JoinGroupResponse> JoinGroupAsync(string session, JoinGroupRequest request, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/groups/join";
            var response = await _httpClient.PostAsJsonAsync(url, request, cancellationToken);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<JoinGroupResponse>(cancellationToken: cancellationToken))
                   ?? throw new InvalidOperationException("JoinGroup returned null");
        }

        public async Task RefreshGroupsAsync(string session, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/groups/refresh";
            var response = await _httpClient.PostAsync(url, null, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task<Group> GetGroupAsync(string session, string groupId, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/groups/{groupId}";
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<Group>(cancellationToken: cancellationToken))
                   ?? throw new InvalidOperationException("GetGroup returned null");
        }

        public async Task DeleteGroupAsync(string session, string groupId, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/groups/{groupId}";
            var response = await _httpClient.DeleteAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task<SettingsSecurityChangeRequest> SetGroupInfoAdminOnlyAsync(string session, string groupId, SettingsSecurityChangeRequest request, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/groups/{groupId}/settings/security/info-admin-only";
            var response = await _httpClient.PutAsJsonAsync(url, request, cancellationToken);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<SettingsSecurityChangeRequest>(cancellationToken: cancellationToken))
                   ?? throw new InvalidOperationException("SetGroupInfoAdminOnly returned null");
        }

        public async Task<SettingsSecurityChangeRequest> GetGroupInfoAdminOnlyAsync(string session, string groupId, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/groups/{groupId}/settings/security/info-admin-only";
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<SettingsSecurityChangeRequest>(cancellationToken: cancellationToken))
                   ?? new SettingsSecurityChangeRequest();
        }

        public async Task<SettingsSecurityChangeRequest> SetGroupMessagesAdminOnlyAsync(string session, string groupId, SettingsSecurityChangeRequest request, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/groups/{groupId}/settings/security/messages-admin-only";
            var response = await _httpClient.PutAsJsonAsync(url, request, cancellationToken);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<SettingsSecurityChangeRequest>(cancellationToken: cancellationToken))
                   ?? throw new InvalidOperationException("SetGroupMessagesAdminOnly returned null");
        }

        public async Task<SettingsSecurityChangeRequest> GetGroupMessagesAdminOnlyAsync(string session, string groupId, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/groups/{groupId}/settings/security/messages-admin-only";
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<SettingsSecurityChangeRequest>(cancellationToken: cancellationToken))
                   ?? new SettingsSecurityChangeRequest();
        }

        public async Task LeaveGroupAsync(string session, string groupId, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/groups/{groupId}/leave";
            var response = await _httpClient.PostAsync(url, null, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task SetGroupDescriptionAsync(string session, string groupId, DescriptionRequest request, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/groups/{groupId}/description";
            var response = await _httpClient.PutAsJsonAsync(url, request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task SetGroupSubjectAsync(string session, string groupId, SubjectRequest request, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/groups/{groupId}/subject";
            var response = await _httpClient.PutAsJsonAsync(url, request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task<string> GetGroupInviteCodeAsync(string session, string groupId, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/groups/{groupId}/invite-code";
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync(cancellationToken);
        }

        public async Task<string> RevokeGroupInviteCodeAsync(string session, string groupId, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/groups/{groupId}/invite-code/revoke";
            var response = await _httpClient.PostAsync(url, null, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync(cancellationToken);
        }

        public async Task<object?> GetGroupParticipantsAsync(string session, string groupId, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/groups/{groupId}/participants";
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<object>(cancellationToken: cancellationToken);
        }

        public async Task AddGroupParticipantsAsync(string session, string groupId, ParticipantsRequest request, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/groups/{groupId}/participants/add";
            var response = await _httpClient.PostAsJsonAsync(url, request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task RemoveGroupParticipantsAsync(string session, string groupId, ParticipantsRequest request, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/groups/{groupId}/participants/remove";
            var response = await _httpClient.PostAsJsonAsync(url, request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task PromoteGroupParticipantsAsync(string session, string groupId, ParticipantsRequest request, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/groups/{groupId}/admin/promote";
            var response = await _httpClient.PostAsJsonAsync(url, request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task DemoteGroupParticipantsAsync(string session, string groupId, ParticipantsRequest request, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/groups/{groupId}/admin/demote";
            var response = await _httpClient.PostAsJsonAsync(url, request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        #endregion

        #region [ PRESENCES ]

        public async Task SetSessionPresenceAsync(string session, SessionPresenceRequest request, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/presence";
            var response = await _httpClient.PostAsJsonAsync(url, request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task<IReadOnlyList<ChatPresences>> GetAllPresencesAsync(string session, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/presence";
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<List<ChatPresences>>(cancellationToken: cancellationToken))
                   ?? new List<ChatPresences>();
        }

        public async Task<ChatPresences> GetPresenceAsync(string session, string chatId, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/presence/{chatId}";
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<ChatPresences>(cancellationToken: cancellationToken))
                   ?? new ChatPresences { ChatId = chatId };
        }

        public async Task SubscribePresenceAsync(string session, string chatId, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/presence/{chatId}/subscribe";
            var response = await _httpClient.PostAsync(url, null, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        #endregion

        #region [ LABELS ]

        public async Task<IReadOnlyList<Label>> GetAllLabelsAsync(string session, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/labels";
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<List<Label>>(cancellationToken: cancellationToken))
                   ?? new List<Label>();
        }

        public async Task<Label> CreateLabelAsync(string session, LabelRequest body, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/labels";
            var response = await _httpClient.PostAsJsonAsync(url, body, cancellationToken);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<Label>(cancellationToken: cancellationToken))
                   ?? throw new InvalidOperationException("CreateLabel returned null");
        }

        public async Task<Label> UpdateLabelAsync(string session, string labelId, LabelRequest body, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/labels/{labelId}";
            var response = await _httpClient.PutAsJsonAsync(url, body, cancellationToken);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<Label>(cancellationToken: cancellationToken))
                   ?? throw new InvalidOperationException("UpdateLabel returned null");
        }

        public async Task DeleteLabelAsync(string session, string labelId, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/labels/{labelId}";
            var response = await _httpClient.DeleteAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task<IReadOnlyList<Label>> GetLabelsForChatAsync(string session, string chatId, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/labels/chats/{chatId}";
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<List<Label>>(cancellationToken: cancellationToken))
                   ?? new List<Label>();
        }

        public async Task PutLabelsForChatAsync(string session, string chatId, SetLabelsRequest request, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/labels/chats/{chatId}";
            var response = await _httpClient.PutAsJsonAsync(url, request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task<object?> GetChatsByLabelAsync(string session, string labelId, CancellationToken cancellationToken)
        {
            var url = $"/api/{session}/labels/{labelId}/chats";
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<object>(cancellationToken: cancellationToken);
        }

        #endregion

        #region [ OBSERVABILITY ]

        public async Task<PingResponse> PingAsync(CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync("/ping", cancellationToken);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<PingResponse>(cancellationToken: cancellationToken))
                   ?? throw new InvalidOperationException("Ping returned null");
        }

        public async Task<HealthResponse> CheckHealthAsync(CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync("/health", cancellationToken);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<HealthResponse>(cancellationToken: cancellationToken))
                   ?? throw new InvalidOperationException("CheckHealth returned null");
        }

        public async Task<Environment> GetServerVersionAsync(CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync("/api/server/version", cancellationToken);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<Environment>(cancellationToken: cancellationToken))
                   ?? throw new InvalidOperationException("GetServerVersion returned null");
        }

        public async Task<object?> GetServerEnvironmentAsync(bool all, CancellationToken cancellationToken)
        {
            var url = $"/api/server/environment?all={all}";
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<object>(cancellationToken: cancellationToken);
        }

        public async Task<ServerStatusResponse> GetServerStatusAsync(CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync("/api/server/status", cancellationToken);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<ServerStatusResponse>(cancellationToken: cancellationToken))
                   ?? throw new InvalidOperationException("GetServerStatus returned null");
        }

        public async Task<StopResponse> StopServerAsync(StopRequest request, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/server/stop", request, cancellationToken);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<StopResponse>(cancellationToken: cancellationToken))
                   ?? throw new InvalidOperationException("StopServer returned null");
        }

        public async Task<byte[]> GetHeapSnapshotAsync(CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync("/api/server/debug/heapsnapshot", cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync(cancellationToken);
        }

        #endregion
    }
}