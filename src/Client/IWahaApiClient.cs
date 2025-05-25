namespace Waha
{
    /// <summary>
    /// Defines a client interface to access all Waha APIs as exposed by Waha Service (https://waha.devlike.pro/swagger/).
    /// Each method corresponds directly to an HTTP endpoint in the Waha API.
    /// Return types and parameters reference the strongly-typed entities from the Waha namespace.
    /// </summary>
    public interface IWahaApiClient
    {
        private const int DEFAULT_LIMIT = 10;
        private const int DEFAULT_OFFSET = 0;

        #region [ SESSIONS ]

        /// <summary>
        /// Corresponds to GET /api/sessions.
        /// Gets a IReadOnlyList of sessions (optionally including STOPPED ones if <paramref name="all"/> is true).
        /// </summary>
        /// <param name="all">If true, returns all sessions including STOPPED ones.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<IReadOnlyList<SessionShort>> GetSessionsAsync(bool all = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to POST /api/sessions.
        /// Creates a new session (and can auto-start it).
        /// </summary>
        /// <param name="createRequest">The session creation request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<Session> CreateSessionAsync(SessionCreateRequest createRequest, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to GET /api/sessions/{session}.
        /// Gets details for a single session by name.
        /// </summary>
        /// <param name="sessionName">The name of the session.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<Session> GetSessionAsync(string sessionName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to PUT /api/sessions/{session}.
        /// Updates a session.
        /// </summary>
        /// <param name="sessionName">The name of the session.</param>
        /// <param name="updateRequest">The session update request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<Session> UpdateSessionAsync(string sessionName, SessionUpdateRequest updateRequest, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to DELETE /api/sessions/{session}.
        /// Deletes a session, stopping and logging out if necessary.
        /// </summary>
        /// <param name="sessionName">The name of the session.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task DeleteSessionAsync(string sessionName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to GET /api/sessions/{session}/me.
        /// Retrieves account info for the authenticated user in the session.
        /// </summary>
        /// <param name="sessionName">The name of the session.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<SessionUser> GetMySessionUserAsync(string sessionName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to POST /api/sessions/{session}/start.
        /// Starts a session by name.
        /// </summary>
        /// <param name="sessionName">The name of the session.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<SessionShort> StartSessionAsync(string sessionName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to POST /api/sessions/{session}/stop.
        /// Stops a session by name.
        /// </summary>
        /// <param name="sessionName">The name of the session.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<SessionShort> StopSessionAsync(string sessionName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to POST /api/sessions/{session}/logout.
        /// Logs out a session by name.
        /// </summary>
        /// <param name="sessionName">The name of the session.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<SessionShort> LogoutSessionAsync(string sessionName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to POST /api/sessions/{session}/restart.
        /// Restarts a session by name.
        /// </summary>
        /// <param name="sessionName">The name of the session.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<SessionShort> RestartSessionAsync(string sessionName, CancellationToken cancellationToken = default);

        #endregion

        #region [ AUTH ]

        /// <summary>
        /// Corresponds to GET /api/{session}/auth/qr.
        /// Retrieves a QR code object for authentication.
        /// </summary>
        /// <param name="sessionName">Name or ID of the session.</param>
        /// <param name="format">QR code output format. Available values: image, raw</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<AuthQrResponse> GetAuthQrAsync(string sessionName, string format = "image", CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to POST /api/{session}/auth/request-code.
        /// Requests an authentication code for the specified session.
        /// </summary>
        /// <param name="sessionName">Name or ID of the session.</param>
        /// <param name="request">Details needed to request the code (e.g. phone number).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<AuthRequestCodeResponse> RequestAuthCodeAsync(string sessionName, AuthCodeRequest request, CancellationToken cancellationToken = default);

        #endregion

        #region [ PROFILE ]

        /// <summary>
        /// Corresponds to GET /api/{session}/profile.
        /// Get my profile.
        /// </summary>
        /// <param name="sessionName">The session name.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        Task<Profile> GetProfileAsync(string sessionName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to PUT /api/{session}/profile/name.
        /// </summary>
        /// <param name="sessionName">The session name.</param>
        /// <param name="name">New name to be applied</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        Task<bool> UpdateProfileNameAsync(string sessionName, string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to PUT /api/{session}/profile/about.
        /// </summary>
        /// <param name="sessionName">The session name.</param>
        /// <param name="about">The about section to be applied.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        Task<bool> UpdateProfileAboutAsync(string sessionName, string about, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to PUT /api/{session}/profile/picture.
        /// </summary>
        /// <param name="sessionName">The session name.</param>
        /// <param name="updateProfilePictureRequest">New profile picture request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        Task<bool> UpdateProfilePictureAsync(string sessionName, UpdateProfilePictureRequest updateProfilePictureRequest, CancellationToken cancellationToken = default);

        #endregion

        #region [ SCREENSHOT ]

        /// <summary>
        /// Corresponds to GET /api/screenshot?session=.
        /// Gets a screenshot of the running WAHA Web session.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<byte[]> GetScreenshotAsync(string session, CancellationToken cancellationToken = default);

        #endregion

        #region [ CHATTING ]

        /// <summary>
        /// Corresponds to POST /api/sendText.
        /// Sends a text message.
        /// </summary>
        /// <param name="request">The send text request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<Message> SendTextAsync(SendTextRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to POST /api/sendImage.
        /// Sends an image message.
        /// </summary>
        /// <param name="request">The send image request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<Message> SendImageAsync(SendImageRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to POST /api/sendFile.
        /// Sends a file message.
        /// </summary>
        /// <param name="request">The send file request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<Message> SendFileAsync(SendFileRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to POST /api/sendVoice.
        /// Sends a voice message.
        /// </summary>
        /// <param name="request">The send voice request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<Message> SendVoiceAsync(SendVoiceRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to POST /api/sendVideo.
        /// Sends a video message.
        /// </summary>
        /// <param name="request">The send video request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<Message> SendVideoAsync(SendVideoRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to POST /api/sendButtons.
        /// Sends a set of interactive buttons.
        /// </summary>
        /// <param name="request">The send buttons request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<Message> SendButtonsAsync(SendButtonsRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to POST /api/forwardMessage.
        /// Forwards an existing message to another chat.
        /// </summary>
        /// <param name="request">The forward message request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<Message> ForwardMessageAsync(ForwardMessageRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to POST /api/sendSeen.
        /// Marks a chat as seen.
        /// </summary>
        /// <param name="request">The send seen request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task SendSeenAsync(SendSeenRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to POST /api/startTyping.
        /// Starts 'typing...' presence in a chat.
        /// </summary>
        /// <param name="request">The chat request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task StartTypingAsync(ChatRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to POST /api/stopTyping.
        /// Stops 'typing...' presence in a chat.
        /// </summary>
        /// <param name="request">The chat request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task StopTypingAsync(ChatRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to PUT /api/reaction.
        /// Reacts to a message with an emoji.
        /// </summary>
        /// <param name="request">The reaction request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<Message> SetReactionAsync(ReactionRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to PUT /api/star.
        /// Stars or unstars a message.
        /// </summary>
        /// <param name="request">The star request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task SetStarAsync(StarRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to POST /api/sendPoll.
        /// Sends a poll to a chat.
        /// </summary>
        /// <param name="request">The send poll request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<Message> SendPollAsync(SendPollRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to POST /api/sendLocation.
        /// Sends a location message.
        /// </summary>
        /// <param name="request">The send location request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<Message> SendLocationAsync(SendLocationRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to POST /api/sendLinkPreview.
        /// Sends a link preview message.
        /// </summary>
        /// <param name="request">The send link preview request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<Message> SendLinkPreviewAsync(SendLinkPreviewRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to POST /api/sendContactVcard.
        /// Sends a contact vcard.
        /// </summary>
        /// <param name="request">The send contact vcard request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<Message> SendContactVcardAsync(SendContactVcardRequest request, CancellationToken cancellationToken = default);

        #endregion

        #region [ CHANNELS ]

        /// <summary>
        /// Corresponds to POST /api/{session}/channels/search/by-view.
        /// Searches for channels by "view".
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="request">The channel search request by view.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<IReadOnlyList<Channel>> SearchChannelsByViewAsync(string session, ChannelSearchByViewRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to POST /api/{session}/channels/search/by-text.
        /// Searches for channels by text keywords.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="request">The channel search request by text.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<IReadOnlyList<Channel>> SearchChannelsByTextAsync(string session, ChannelSearchByTextRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to GET /api/{session}/channels/search/views.
        /// Gets a IReadOnlyList of search views available for channel searching.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<IReadOnlyList<object>> GetChannelSearchViewsAsync(string session, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to GET /api/{session}/channels/search/countries.
        /// Gets a IReadOnlyList of countries for channel search.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<IReadOnlyList<object>> GetChannelSearchCountriesAsync(string session, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to GET /api/{session}/channels/search/categories.
        /// Gets a IReadOnlyList of categories for channel search.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<IReadOnlyList<object>> GetChannelSearchCategoriesAsync(string session, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to GET /api/{session}/channels/{id}/messages/preview.
        /// Previews channel messages by channel ID or invite code.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="id">Channel ID or invite code.</param>
        /// <param name="downloadMedia">If true, downloads media files.</param>
        /// <param name="limit">Max number of messages to retrieve.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<IReadOnlyList<ChannelMessage>> PreviewChannelMessagesAsync(string session, string id, bool downloadMedia = false, int limit = DEFAULT_LIMIT, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to GET /api/{session}/channels.
        /// Gets a IReadOnlyList of known channels (optionally filtered by role).
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="role">Optional filter by role.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<IReadOnlyList<Channel>> GetChannelsAsync(string session, string? role = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to POST /api/{session}/channels.
        /// Creates a new channel.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="createChannelRequest">The channel creation request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<Channel> CreateChannelAsync(string session, CreateChannelRequest createChannelRequest, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to DELETE /api/{session}/channels/{id}.
        /// Deletes a channel by ID.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="channelId">The channel ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task DeleteChannelAsync(string session, string channelId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to GET /api/{session}/channels/{id}.
        /// Gets a channel's info by ID or invite code.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="channelIdOrInvite">The channel ID or invite code.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<Channel> GetChannelAsync(string session, string channelIdOrInvite, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to POST /api/{session}/channels/{id}/follow.
        /// Follows (subscribes to) a channel.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="channelId">The channel ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task FollowChannelAsync(string session, string channelId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to POST /api/{session}/channels/{id}/unfollow.
        /// Unfollows (unsubscribes from) a channel.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="channelId">The channel ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task UnfollowChannelAsync(string session, string channelId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to POST /api/{session}/channels/{id}/mute.
        /// Mutes a channel.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="channelId">The channel ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task MuteChannelAsync(string session, string channelId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to POST /api/{session}/channels/{id}/unmute.
        /// Unmutes a channel.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="channelId">The channel ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task UnmuteChannelAsync(string session, string channelId, CancellationToken cancellationToken = default);

        #endregion

        #region [ STATUS ]

        /// <summary>
        /// Corresponds to POST /api/{session}/status/text.
        /// Sends a text status (story).
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="request">The text status request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task SendTextStatusAsync(string session, TextStatusRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to POST /api/{session}/status/image.
        /// Sends an image status (story).
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="request">The image status request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task SendImageStatusAsync(string session, ImageStatusRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to POST /api/{session}/status/voice.
        /// Sends a voice status (story).
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="request">The voice status request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task SendVoiceStatusAsync(string session, VoiceStatusRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to POST /api/{session}/status/video.
        /// Sends a video status (story).
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="request">The video status request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task SendVideoStatusAsync(string session, VideoStatusRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to POST /api/{session}/status/delete.
        /// Deletes a previously sent status (story).
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="request">The delete status request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task DeleteStatusAsync(string session, DeleteStatusRequest request, CancellationToken cancellationToken = default);

        #endregion

        #region [ CHATS ]

        /// <summary>
        /// Corresponds to GET /api/{session}/chats.
        /// Retrieves all chats in the specified session.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="limit">Max number of contacts to retrieve.</param>
        /// <param name="offset">Offset for pagination.</param>
        /// <param name="sortBy">Sort by field.</param><
        /// <param name="sortOrder">Sort order - desc (Z => A, New first) or asc (A => Z, Old first).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<IReadOnlyList<Chat>> GetChatsAsync(string session, int limit = DEFAULT_LIMIT, int offset = DEFAULT_OFFSET, string sortBy = "", string sortOrder = "", CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to GET /api/{session}/chats/overview.
        /// Retrieves an overview of chats for the specified session.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<IReadOnlyList<ChatOverview>> GetChatsOverviewAsync(string session, int limit = DEFAULT_LIMIT, int offset = DEFAULT_OFFSET, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to DELETE /api/{session}/chats/{chatId}.
        /// Deletes the specified chat.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="chatId">The chat ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task DeleteChatAsync(string session, string chatId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to GET /api/{session}/chats/{chatId}/picture.
        /// Retrieves the chat's picture data.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="chatId">The chat ID.</param>
        /// <param name="refresh">Refresh the picture from the server (24h cache by default). Do not refresh if not needed, you can get rate limit error</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<ChatPicture> GetChatPictureAsync(string session, string chatId, bool refresh = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to GET /api/{session}/chats/{chatId}/messages.
        /// Retrieves all messages in the specified chat.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="chatId">The chat ID.</param>
        /// <param name="limit">Max number of contacts to retrieve.</param>
        /// <param name="offset">Offset for pagination.</param>
        /// <param name="filterTimestampLte">Filter messages before this timestamp (inclusive).</param>
        /// <param name="filterTimestampGte">Filter messages after this timestamp (inclusive).</param>
        /// <param name="filterOnlyMyMessages">Null bring all messages. You can filter if you want to take only your messages (true) or others messages (false).</param>
        /// <param name="downloadMedia">Download media for messages.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<IReadOnlyList<ChatMessage>> GetChatMessagesAsync(string session, string chatId, int limit = DEFAULT_LIMIT, int offset = DEFAULT_OFFSET, string filterTimestampLte = "", string filterTimestampGte = "", bool? filterOnlyMyMessages = null, bool downloadMedia = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to POST /api/{session}/chats/{chatId}/messages.
        /// Sends (creates) a new message in the specified chat.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="chatId">The chat ID.</param>
        /// <param name="request">The request payload containing message data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<ChatMessage> SendChatMessageAsync(string session, string chatId, CreateChatMessageRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to DELETE /api/{session}/chats/{chatId}/messages.
        /// Clears all messages in the specified chat.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="chatId">The chat ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task ClearChatMessagesAsync(string session, string chatId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to GET /api/{session}/chats/{chatId}/messages/{messageId}.
        /// Retrieves a single message by ID within the specified chat.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="chatId">The chat ID.</param>
        /// <param name="messageId">The message ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<ChatMessage> GetChatMessageAsync(string session, string chatId, string messageId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to DELETE /api/{session}/chats/{chatId}/messages/{messageId}.
        /// Deletes a single message by ID within the specified chat.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="chatId">The chat ID.</param>
        /// <param name="messageId">The message ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task DeleteChatMessageAsync(string session, string chatId, string messageId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to PUT /api/{session}/chats/{chatId}/messages/{messageId}.
        /// Edits (updates) an existing message within the specified chat.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="chatId">The chat ID.</param>
        /// <param name="messageId">The message ID.</param>
        /// <param name="request">The request payload containing updated message data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<ChatMessage> EditChatMessageAsync(string session, string chatId, string messageId, EditChatMessageRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to POST /api/{session}/chats/{chatId}/messages/{messageId}/pin.
        /// Pins a specific message in the chat.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="chatId">The chat ID.</param>
        /// <param name="messageId">The message ID to pin.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task PinChatMessageAsync(string session, string chatId, string messageId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to POST /api/{session}/chats/{chatId}/messages/{messageId}/unpin.
        /// Unpins a specific message in the chat.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="chatId">The chat ID.</param>
        /// <param name="messageId">The message ID to unpin.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task UnpinChatMessageAsync(string session, string chatId, string messageId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to POST /api/{session}/chats/{chatId}/archive.
        /// Archives the specified chat.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="chatId">The chat ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task ArchiveChatAsync(string session, string chatId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to POST /api/{session}/chats/{chatId}/unarchive.
        /// Unarchives the specified chat.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="chatId">The chat ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task UnarchiveChatAsync(string session, string chatId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to POST /api/{session}/chats/{chatId}/unread.
        /// Marks the specified chat as unread.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="chatId">The chat ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task UnreadChatAsync(string session, string chatId, CancellationToken cancellationToken = default);

        #endregion

        #region [ CONTACTS ]

        /// <summary>
        /// Corresponds to GET /api/contacts/all.
        /// Retrieves all contacts from the session.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="sortAsc">Whether to sort ascending.</param>
        /// <param name="sortOrder">Sort order - desc (Z => A, New first) or asc (A => Z, Old first)</param>
        /// <param name="limit">Max number of contacts to retrieve.</param>
        /// <param name="offset">Offset for pagination.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<IReadOnlyList<Contact>> GetAllContactsAsync(string session, int limit = DEFAULT_LIMIT, int offset = DEFAULT_OFFSET, string sortAsc = "", string sortOrder = "", CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to GET /api/contacts?contactId=.
        /// Retrieves basic info about a single contact.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="contactId">The contact ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<object?> GetContactAsync(string session, string contactId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to GET /api/contacts/check-exists.
        /// Checks if a phone number is registered in WhatsApp (preferred method).
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="phone">The phone number.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<NumberExistResult> CheckContactExistsAsync(string session, string phone, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to GET /api/contacts/about.
        /// Gets the "about" info for a contact, if allowed.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="contactId">The contact ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<string?> GetContactAboutAsync(string session, string contactId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to GET /api/contacts/profile-picture.
        /// Gets the profile picture URL for a contact (may return null if privacy blocks).
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="contactId">The contact ID.</param>
        /// <param name="refresh">If true, forces a refresh of the picture.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<string?> GetContactProfilePictureAsync(string session, string contactId, bool refresh = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to POST /api/contacts/block.
        /// Blocks a contact.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="contactId">The contact ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task BlockContactAsync(string session, string contactId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to POST /api/contacts/unblock.
        /// Unblocks a contact.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="contactId">The contact ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task UnblockContactAsync(string session, string contactId, CancellationToken cancellationToken = default);

        #endregion

        #region [ GROUPS ]

        /// <summary>
        /// Corresponds to POST /api/{session}/groups.
        /// Creates a new group with participants.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="request">The group creation request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<Group> CreateGroupAsync(string session, CreateGroupRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to GET /api/{session}/groups.
        /// Retrieves all groups.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="sortAsc">Whether to sort ascending.</param>
        /// <param name="sortBy">The field to sort by.</param>
        /// <param name="limit">Max number of groups to retrieve.</param>
        /// <param name="offset">Offset for pagination.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<IReadOnlyList<Group>> GetGroupsAsync(string session, bool? sortAsc = null, string? sortBy = null, int? limit = null, int? offset = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to GET /api/{session}/groups/join-info.
        /// Gets info about a group before joining by invite code.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="code">The invite code.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<object?> GetGroupJoinInfoAsync(string session, string code, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to POST /api/{session}/groups/join.
        /// Joins a group using code/link.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="request">The join group request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<JoinGroupResponse> JoinGroupAsync(string session, JoinGroupRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to POST /api/{session}/groups/refresh.
        /// Forces a refresh of group data from the server.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task RefreshGroupsAsync(string session, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to GET /api/{session}/groups/{id}.
        /// Retrieves a group's information by ID.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="groupId">The group ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<Group> GetGroupAsync(string session, string groupId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to DELETE /api/{session}/groups/{id}.
        /// Deletes a group (disbands).
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="groupId">The group ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task DeleteGroupAsync(string session, string groupId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to PUT /api/{session}/groups/{id}/settings/security/info-admin-only.
        /// Restricts group info editing to admins only.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="groupId">The group ID.</param>
        /// <param name="request">Security change request data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<SettingsSecurityChangeRequest> SetGroupInfoAdminOnlyAsync(string session, string groupId, SettingsSecurityChangeRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to GET /api/{session}/groups/{id}/settings/security/info-admin-only.
        /// Gets whether group info is admin-only or not.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="groupId">The group ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<SettingsSecurityChangeRequest> GetGroupInfoAdminOnlyAsync(string session, string groupId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to PUT /api/{session}/groups/{id}/settings/security/messages-admin-only.
        /// Restricts sending messages in a group to admins only.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="groupId">The group ID.</param>
        /// <param name="request">Security change request data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<SettingsSecurityChangeRequest> SetGroupMessagesAdminOnlyAsync(string session, string groupId, SettingsSecurityChangeRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to GET /api/{session}/groups/{id}/settings/security/messages-admin-only.
        /// Gets whether group messages are admin-only or not.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="groupId">The group ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<SettingsSecurityChangeRequest> GetGroupMessagesAdminOnlyAsync(string session, string groupId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to POST /api/{session}/groups/{id}/leave.
        /// Leaves a group (but does not delete it for others).
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="groupId">The group ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task LeaveGroupAsync(string session, string groupId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to PUT /api/{session}/groups/{id}/description.
        /// Sets the group description.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="groupId">The group ID.</param>
        /// <param name="request">Request containing the new description.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task SetGroupDescriptionAsync(string session, string groupId, DescriptionRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to PUT /api/{session}/groups/{id}/subject.
        /// Sets the group subject (title).
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="groupId">The group ID.</param>
        /// <param name="request">Request containing the new subject.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task SetGroupSubjectAsync(string session, string groupId, SubjectRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to GET /api/{session}/groups/{id}/invite-code.
        /// Gets the group's invite code.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="groupId">The group ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<string> GetGroupInviteCodeAsync(string session, string groupId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to POST /api/{session}/groups/{id}/invite-code/revoke.
        /// Revokes the current group invite code.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="groupId">The group ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<string> RevokeGroupInviteCodeAsync(string session, string groupId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to GET /api/{session}/groups/{id}/participants.
        /// Retrieves the participants for a group.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="groupId">The group ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<object?> GetGroupParticipantsAsync(string session, string groupId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to POST /api/{session}/groups/{id}/participants/add.
        /// Adds participants to a group.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="groupId">The group ID.</param>
        /// <param name="request">Request containing participant info.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task AddGroupParticipantsAsync(string session, string groupId, ParticipantsRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to POST /api/{session}/groups/{id}/participants/remove.
        /// Removes participants from a group.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="groupId">The group ID.</param>
        /// <param name="request">Request containing participant info.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task RemoveGroupParticipantsAsync(string session, string groupId, ParticipantsRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to POST /api/{session}/groups/{id}/admin/promote.
        /// Promotes participants to admin status.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="groupId">The group ID.</param>
        /// <param name="request">Request containing participant info.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task PromoteGroupParticipantsAsync(string session, string groupId, ParticipantsRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to POST /api/{session}/groups/{id}/admin/demote.
        /// Demotes participants from admin status.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="groupId">The group ID.</param>
        /// <param name="request">Request containing participant info.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task DemoteGroupParticipantsAsync(string session, string groupId, ParticipantsRequest request, CancellationToken cancellationToken = default);

        #endregion

        #region [ PRESENCES ]

        /// <summary>
        /// Corresponds to POST /api/{session}/presence.
        /// Sets the presence of the session to online or offline.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="request">Request containing presence info.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task SetSessionPresenceAsync(string session, SessionPresenceRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to GET /api/{session}/presence.
        /// Gets all subscribed presence info for the session.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<IReadOnlyList<ChatPresences>> GetAllPresencesAsync(string session, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to GET /api/{session}/presence/{chatId}.
        /// Gets presence info for a specific chat (also subscribes to that presence).
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="chatId">The chat ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<ChatPresences> GetPresenceAsync(string session, string chatId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to POST /api/{session}/presence/{chatId}/subscribe.
        /// Subscribes to presence events for a given chat ID.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="chatId">The chat ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task SubscribePresenceAsync(string session, string chatId, CancellationToken cancellationToken = default);

        #endregion

        #region [ LABELS ]

        /// <summary>
        /// Corresponds to GET /api/{session}/labels.
        /// Gets all labels for the session.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<IReadOnlyList<Label>> GetAllLabelsAsync(string session, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to POST /api/{session}/labels.
        /// Creates a new label.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="body">Request body containing label data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<Label> CreateLabelAsync(string session, LabelRequest body, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to PUT /api/{session}/labels/{labelId}.
        /// Updates an existing label by ID.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="labelId">The label ID.</param>
        /// <param name="body">Request body containing updated label data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<Label> UpdateLabelAsync(string session, string labelId, LabelRequest body, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to DELETE /api/{session}/labels/{labelId}.
        /// Deletes a label by ID.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="labelId">The label ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task DeleteLabelAsync(string session, string labelId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to GET /api/{session}/labels/chats/{chatId}.
        /// Gets labels for a chat.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="chatId">The chat ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<IReadOnlyList<Label>> GetLabelsForChatAsync(string session, string chatId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to PUT /api/{session}/labels/chats/{chatId}.
        /// Sets labels on a chat, replacing existing ones.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="chatId">The chat ID.</param>
        /// <param name="request">Request containing label data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task PutLabelsForChatAsync(string session, string chatId, SetLabelsRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to GET /api/{session}/labels/{labelId}/chats.
        /// Gets chats that have a specific label.
        /// </summary>
        /// <param name="session">The session name.</param>
        /// <param name="labelId">The label ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<object?> GetChatsByLabelAsync(string session, string labelId, CancellationToken cancellationToken = default);

        #endregion

        #region [ OBSERVABILITY ]

        /// <summary>
        /// Corresponds to GET /ping.
        /// Checks if the server is alive and responding.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<PingResponse> PingAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to GET /health.
        /// Performs all health checks and returns server health status.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<HealthResponse> CheckHealthAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to GET /api/server/version.
        /// Retrieves the WAHA server version.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<Environment> GetServerVersionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to GET /api/server/environment.
        /// Gets the server environment, optionally including all environment variables.
        /// </summary>
        /// <param name="all">Include all environment variables if true.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<object?> GetServerEnvironmentAsync(bool all = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to GET /api/server/status.
        /// Retrieves the server's runtime status info (uptime, pid, etc.).
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<ServerStatusResponse> GetServerStatusAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to POST /api/server/stop.
        /// Stops (and can restart) the WAHA server.
        /// </summary>
        /// <param name="request">Stop request data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<StopResponse> StopServerAsync(StopRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Corresponds to GET /api/server/debug/heapsnapshot.
        /// Returns a heap snapshot of the server's memory usage.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<byte[]> GetHeapSnapshotAsync(CancellationToken cancellationToken = default);

        #endregion
    }
}