using System.Text.Json.Serialization;

namespace Waha
{
    #region [ SESSIONS ]

    /// <summary>
    /// Represents the session information returned from the API.
    /// </summary>
    public record Session
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = default!;

        [JsonPropertyName("me")]
        public SessionUser Me { get; set; } = default!;

        [JsonPropertyName("assignedWorker")]
        public string AssignedWorker { get; set; } = default!;

        [JsonPropertyName("status")]
        public string Status { get; set; } = default!;

        [JsonPropertyName("config")]
        public SessionConfig Config { get; set; } = default!;
    }

    /// <summary>
    /// A minimal version of session data.
    /// Used by endpoints that return a lighter session structure.
    /// </summary>
    public record SessionShort
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = default!;

        [JsonPropertyName("status")]
        public string Status { get; set; } = default!;

        [JsonPropertyName("config")]
        public SessionConfig? Config { get; set; }
    }

    /// <summary>
    /// Represents the user info of the authenticated account.
    /// </summary>
    public record SessionUser
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = default!;

        [JsonPropertyName("pushName")]
        public string PushName { get; set; } = default!;
    }

    /// <summary>
    /// Configuration details for the session.
    /// </summary>
    public record SessionConfig
    {
        [JsonPropertyName("metadata")]
        public Dictionary<string, object>? Metadata { get; set; }

        [JsonPropertyName("proxy")]
        public ProxyConfig? Proxy { get; set; }

        [JsonPropertyName("debug")]
        public bool Debug { get; set; }

        [JsonPropertyName("noweb")]
        public NowebConfig? Noweb { get; set; }

        [JsonPropertyName("webhooks")]
        public List<WebhookConfig>? Webhooks { get; set; }
    }

    /// <summary>
    /// Proxy configuration if the session requires a proxy.
    /// </summary>
    public record ProxyConfig
    {
        [JsonPropertyName("server")]
        public string Server { get; set; } = default!;

        [JsonPropertyName("username")]
        public string? Username { get; set; }

        [JsonPropertyName("password")]
        public string? Password { get; set; }
    }

    /// <summary>
    /// Configuration to manage \"no-web\" mode.
    /// </summary>
    public record NowebConfig
    {
        [JsonPropertyName("markOnline")]
        public bool MarkOnline { get; set; } = true;

        [JsonPropertyName("store")]
        public NowebStoreConfig? Store { get; set; }
    }

    /// <summary>
    /// Store configuration for contacts, chats, and messages.
    /// </summary>
    public record NowebStoreConfig
    {
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }

        [JsonPropertyName("fullSync")]
        public bool FullSync { get; set; }
    }

    /// <summary>
    /// Configuration of webhooks for the session.
    /// </summary>
    public record WebhookConfig
    {
        [JsonPropertyName("url")]
        public string Url { get; set; } = default!;

        [JsonPropertyName("events")]
        public List<string> Events { get; set; } = new();

        [JsonPropertyName("hmac")]
        public HmacConfiguration? Hmac { get; set; }

        [JsonPropertyName("retries")]
        public RetriesConfiguration? Retries { get; set; }

        [JsonPropertyName("customHeaders")]
        public List<CustomHeader>? CustomHeaders { get; set; }
    }

    /// <summary>
    /// Represents HMAC configuration if the webhook needs to sign requests.
    /// </summary>
    public record HmacConfiguration
    {
        [JsonPropertyName("key")]
        public string? Key { get; set; }
    }

    /// <summary>
    /// Configuration for retry policies.
    /// </summary>
    public record RetriesConfiguration
    {
        [JsonPropertyName("delaySeconds")]
        public double? DelaySeconds { get; set; }

        [JsonPropertyName("attempts")]
        public int? Attempts { get; set; }

        [JsonPropertyName("policy")]
        public string? Policy { get; set; }
    }

    /// <summary>
    /// Represents a custom header for webhook calls.
    /// </summary>
    public record CustomHeader
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = default!;

        [JsonPropertyName("value")]
        public string Value { get; set; } = default!;
    }

    /// <summary>
    /// Request object for creating a session.
    /// </summary>
    public record SessionCreateRequest
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = default!;

        [JsonPropertyName("start")]
        public bool? Start { get; set; } = true;

        [JsonPropertyName("config")]
        public SessionConfig? Config { get; set; }
    }

    /// <summary>
    /// Request object for updating an existing session.
    /// </summary>
    public record SessionUpdateRequest
    {
        [JsonPropertyName("config")]
        public SessionConfig? Config { get; set; }
    }

    /// <summary>
    /// DEPRECATED request for starting a session.
    /// </summary>
    [Obsolete]
    public record SessionStartRequest
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = default!;

        [JsonPropertyName("config")]
        public SessionConfig? Config { get; set; }
    }

    /// <summary>
    /// DEPRECATED request for stopping a session.
    /// </summary>
    [Obsolete]
    public record SessionStopRequest
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = default!;

        [JsonPropertyName("logout")]
        public bool Logout { get; set; } = false;
    }

    /// <summary>
    /// DEPRECATED request for logging out a session.
    /// </summary>
    [Obsolete]
    public record SessionLogoutRequest
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = default!;
    }

    #endregion

    #region [ AUTH ]

    /// <summary>
    /// Response when retrieving a QR code for authentication.
    /// </summary>
    public record AuthQrResponse
    {
        [JsonPropertyName("qrUrl")]
        public string QrUrl { get; set; } = default!;

        [JsonPropertyName("expiresIn")]
        public int ExpiresIn { get; set; }
    }

    /// <summary>
    /// Request payload for obtaining an authentication code.
    /// </summary>
    public record AuthCodeRequest
    {
        [JsonPropertyName("phoneNumber")]
        public string PhoneNumber { get; set; } = default!;

        [JsonPropertyName("method")]
        public string Method { get; set; } = default!;
    }

    /// <summary>
    /// Response payload after requesting an authentication code.
    /// </summary>
    public record AuthRequestCodeResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }

    #endregion

    #region [ CHATTING ]

    /// <summary>
    /// Represents a WhatsApp message.
    /// </summary>
    public record Message
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = default!;

        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }

        [JsonPropertyName("from")]
        public string From { get; set; } = default!;

        [JsonPropertyName("fromMe")]
        public bool FromMe { get; set; }

        [JsonPropertyName("to")]
        public string To { get; set; } = default!;

        [JsonPropertyName("participant")]
        public string? Participant { get; set; }

        [JsonPropertyName("body")]
        public string Body { get; set; } = default!;

        [JsonPropertyName("hasMedia")]
        public bool HasMedia { get; set; }

        [JsonPropertyName("media")]
        public Media? Media { get; set; }

        [JsonPropertyName("ack")]
        public int Ack { get; set; }

        [JsonPropertyName("ackName")]
        public string AckName { get; set; } = default!;

        [JsonPropertyName("location")]
        public Location? Location { get; set; }

        [JsonPropertyName("vCards")]
        public List<string>? VCards { get; set; }

        [JsonPropertyName("_data")]
        public object? Data { get; set; }

        [JsonPropertyName("replyTo")]
        public ReplyToMessage? ReplyTo { get; set; }
    }

    /// <summary>
    /// Represent a S3 Media reference. 
    /// </summary>
    public record MediaS3Reference
    {
        [JsonPropertyName("bucket")]
        public string? Bucket { get; set; }

        [JsonPropertyName("key")]
        public string? Key { get; set; }
    }

    /// <summary>
    /// Represent a media object for messages.
    /// </summary>
    public record Media
    {
        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("mimetype")]
        public string? MimeType { get; set; }

        [JsonPropertyName("filename")]
        public string? FileName { get; set; }

        [JsonPropertyName("s3")]
        public MediaS3Reference S3 { get; set; }

        [JsonPropertyName("error")]
        public string? Error { get; set; }
    }

    /// <summary>
    /// Location info in a message.
    /// </summary>
    public record Location
    {
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("latitude")]
        public string Latitude { get; set; } = default!;

        [JsonPropertyName("longitude")]
        public string Longitude { get; set; } = default!;
    }

    /// <summary>
    /// Used if the message is a reply to another message.
    /// </summary>
    public record ReplyToMessage
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = default!;

        [JsonPropertyName("participant")]
        public string? Participant { get; set; }

        [JsonPropertyName("body")]
        public string Body { get; set; } = default!;
    }

    /// <summary>
    /// Request to send a text message.
    /// </summary>
    public record SendTextRequest
    {
        [JsonPropertyName("chatId")]
        public string ChatId { get; set; } = default!;

        [JsonPropertyName("text")]
        public string Text { get; set; } = default!;

        [JsonPropertyName("linkPreview")]
        public bool LinkPreview { get; set; } = true;

        [JsonPropertyName("reply_to")]
        public string? ReplyTo { get; set; }

        [JsonPropertyName("session")]
        public string Session { get; set; } = "default";
    }

    /// <summary>
    /// Used for image sending.
    /// </summary>
    public record SendImageRequest
    {
        [JsonPropertyName("chatId")]
        public string ChatId { get; set; } = default!;

        [JsonPropertyName("file")]
        public FileData File { get; set; } = default!;

        [JsonPropertyName("reply_to")]
        public string? ReplyTo { get; set; }

        [JsonPropertyName("caption")]
        public string? Caption { get; set; }

        [JsonPropertyName("session")]
        public string Session { get; set; } = "default";
    }

    /// <summary>
    /// Used for file sending.
    /// </summary>
    public record SendFileRequest
    {
        [JsonPropertyName("chatId")]
        public string ChatId { get; set; } = default!;

        [JsonPropertyName("file")]
        public FileData File { get; set; } = default!;

        [JsonPropertyName("reply_to")]
        public string? ReplyTo { get; set; }

        [JsonPropertyName("caption")]
        public string? Caption { get; set; }

        [JsonPropertyName("session")]
        public string Session { get; set; } = "default";
    }

    /// <summary>
    /// Used for voice sending.
    /// </summary>
    public record SendVoiceRequest
    {
        [JsonPropertyName("chatId")]
        public string ChatId { get; set; } = default!;

        [JsonPropertyName("file")]
        public FileData File { get; set; } = default!;

        [JsonPropertyName("reply_to")]
        public string? ReplyTo { get; set; }

        [JsonPropertyName("session")]
        public string Session { get; set; } = "default";
    }

    /// <summary>
    /// Used for video sending.
    /// </summary>
    public record SendVideoRequest
    {
        [JsonPropertyName("chatId")]
        public string ChatId { get; set; } = default!;

        [JsonPropertyName("file")]
        public FileData File { get; set; } = default!;

        [JsonPropertyName("reply_to")]
        public string? ReplyTo { get; set; }

        [JsonPropertyName("session")]
        public string Session { get; set; } = "default";
    }

    /// <summary>
    /// File data can be remote or base64.
    /// </summary>
    public record FileData
    {
        [JsonPropertyName("mimetype")]
        public string Mimetype { get; set; } = default!;

        [JsonPropertyName("filename")]
        public string? Filename { get; set; }

        [JsonPropertyName("data")]
        public string? Data { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }
    }

    /// <summary>
    /// Send a set of buttons.
    /// </summary>
    public record SendButtonsRequest
    {
        [JsonPropertyName("chatId")]
        public string ChatId { get; set; } = default!;

        [JsonPropertyName("text")]
        public string Text { get; set; } = default!;

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("footer")]
        public string? Footer { get; set; }

        [JsonPropertyName("buttons")]
        public List<string>? Buttons { get; set; }

        [JsonPropertyName("session")]
        public string Session { get; set; } = "default";
    }

    /// <summary>
    /// Forward a message.
    /// </summary>
    public record ForwardMessageRequest
    {
        [JsonPropertyName("chatId")]
        public string ChatId { get; set; } = default!;

        [JsonPropertyName("messageId")]
        public string MessageId { get; set; } = default!;

        [JsonPropertyName("session")]
        public string Session { get; set; } = "default";
    }

    /// <summary>
    /// Mark the chat as seen.
    /// </summary>
    public record SendSeenRequest
    {
        [JsonPropertyName("chatId")]
        public string ChatId { get; set; } = default!;

        [JsonPropertyName("session")]
        public string Session { get; set; } = "default";
    }

    /// <summary>
    /// Start or stop typing.
    /// </summary>
    public record ChatRequest
    {
        [JsonPropertyName("chatId")]
        public string ChatId { get; set; } = default!;

        [JsonPropertyName("session")]
        public string Session { get; set; } = "default";
    }

    /// <summary>
    /// React to a message with an emoji.
    /// </summary>
    public record ReactionRequest
    {
        [JsonPropertyName("messageId")]
        public string MessageId { get; set; } = default!;

        [JsonPropertyName("chatId")]
        public string ChatId { get; set; } = default!;

        [JsonPropertyName("emoji")]
        public string Emoji { get; set; } = default!;

        [JsonPropertyName("session")]
        public string Session { get; set; } = "default";
    }

    /// <summary>
    /// Star or unstar a message.
    /// </summary>
    public record StarRequest
    {
        [JsonPropertyName("messageId")]
        public string MessageId { get; set; } = default!;

        [JsonPropertyName("chatId")]
        public string ChatId { get; set; } = default!;

        [JsonPropertyName("star")]
        public bool Star { get; set; }

        [JsonPropertyName("session")]
        public string Session { get; set; } = "default";
    }

    /// <summary>
    /// Send a poll.
    /// </summary>
    public record SendPollRequest
    {
        [JsonPropertyName("chatId")]
        public string ChatId { get; set; } = default!;

        [JsonPropertyName("name")]
        public string Question { get; set; } = default!;

        [JsonPropertyName("options")]
        public List<string> Options { get; set; } = new();

        [JsonPropertyName("session")]
        public string Session { get; set; } = "default";
    }

    /// <summary>
    /// Send location.
    /// </summary>
    public record SendLocationRequest
    {
        [JsonPropertyName("chatId")]
        public string ChatId { get; set; } = default!;

        [JsonPropertyName("latitude")]
        public string Latitude { get; set; } = default!;

        [JsonPropertyName("longitude")]
        public string Longitude { get; set; } = default!;

        [JsonPropertyName("session")]
        public string Session { get; set; } = "default";
    }

    /// <summary>
    /// Send link preview.
    /// </summary>
    public record SendLinkPreviewRequest
    {
        [JsonPropertyName("chatId")]
        public string ChatId { get; set; } = default!;

        [JsonPropertyName("url")]
        public string Url { get; set; } = default!;

        [JsonPropertyName("text")]
        public string Text { get; set; } = default!;

        [JsonPropertyName("session")]
        public string Session { get; set; } = "default";
    }

    /// <summary>
    /// Send contact vcard.
    /// </summary>
    public record SendContactVcardRequest
    {
        [JsonPropertyName("chatId")]
        public string ChatId { get; set; } = default!;

        [JsonPropertyName("contactName")]
        public string ContactName { get; set; } = default!;

        [JsonPropertyName("vcard")]
        public string Vcard { get; set; } = default!;

        [JsonPropertyName("session")]
        public string Session { get; set; } = "default";
    }

    /// <summary>
    /// Result from checking if a number is registered in WhatsApp.
    /// </summary>
    public record NumberExistResult
    {
        [JsonPropertyName("exists")]
        public bool Exists { get; set; }

        [JsonPropertyName("formattedNumber")]
        public string? FormattedNumber { get; set; }

        [JsonPropertyName("jid")]
        public string? Jid { get; set; }
    }

    /// <summary>
    /// DEPRECATED request for replying to a message.
    /// </summary>
    public record ReplyRequest
    {
        [JsonPropertyName("chatId")]
        public string ChatId { get; set; } = default!;

        [JsonPropertyName("reply_to")]
        public string ReplyTo { get; set; } = default!;

        [JsonPropertyName("text")]
        public string Text { get; set; } = default!;

        [JsonPropertyName("session")]
        public string Session { get; set; } = "default";
    }

    #endregion

    #region [ CHANNELS ]

    /// <summary>
    /// Represents a WhatsApp Channel.
    /// </summary>
    public record Channel
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = default!;

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("invite")]
        public string? Invite { get; set; }

        [JsonPropertyName("preview")]
        public string? Preview { get; set; }

        [JsonPropertyName("picture")]
        public string? Picture { get; set; }

        [JsonPropertyName("role")]
        public string? Role { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("verified")]
        public string? Verified { get; set; }

        [JsonPropertyName("subscribersCount")]
        public string? SubscribersCount { get; set; }
    }

    /// <summary>
    /// A single channel message preview.
    /// </summary>
    public record ChannelMessage
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = default!;

        [JsonPropertyName("body")]
        public string Body { get; set; } = default!;

        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }
    }

    /// <summary>
    /// For searching channels by text.
    /// </summary>
    public record ChannelSearchByTextRequest
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = default!;

        [JsonPropertyName("categories")]
        public List<string> Categories { get; set; } = default!;

        [JsonPropertyName("limit")]
        public int Limit { get; set; } = default!;

        [JsonPropertyName("startCursor")]
        public string StartCursor { get; set; } = default!;
    }

    /// <summary>
    /// For searching channels by "view".
    /// </summary>
    public record ChannelSearchByViewRequest
    {
        [JsonPropertyName("view")]
        public string View { get; set; } = default!;

        [JsonPropertyName("countries")]
        public List<string> Countries { get; set; } = default!;

        [JsonPropertyName("view")]
        public List<string> Categories { get; set; } = default!;

        [JsonPropertyName("limit")]
        public int Limit { get; set; } = default!;

        [JsonPropertyName("startCursor")]
        public string StartCursor { get; set; } = default!;
    }

    public record CreateChannelRequest 
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = default!;

        [JsonPropertyName("description")]
        public string Description { get; set; } = default!;

        [JsonPropertyName("picture")]
        public object Picture { get; set; } = default!;
    }

    /// <summary>
    /// A channel list result from searching.
    /// </summary>
    public record ChannelListResult
    {
        [JsonPropertyName("channels")]
        public List<Channel> Channels { get; set; } = new();
    }

    #endregion

    #region [ STATUS ]

    /// <summary>
    /// Send a text status.
    /// </summary>
    public record TextStatusRequest
    {
        [JsonPropertyName("contacts")]
        public List<string> Contacts { get; set; } = default!;

        [JsonPropertyName("text")]
        public string Text { get; set; } = default!;

        [JsonPropertyName("backgroundColor")]
        public string BackgroundColor { get; set; } = default!;

        [JsonPropertyName("font")]
        public int Font { get; set; } = default!;
    }

    /// <summary>
    /// Send an image status.
    /// </summary>
    public record ImageStatusRequest
    {
        [JsonPropertyName("file")]
        public FileData File { get; set; } = default!;

        [JsonPropertyName("caption")]
        public string? Caption { get; set; }
    }

    /// <summary>
    /// Send a voice status.
    /// </summary>
    public record VoiceStatusRequest
    {
        [JsonPropertyName("file")]
        public FileData File { get; set; } = default!;
    }

    /// <summary>
    /// Send a video status.
    /// </summary>
    public record VideoStatusRequest
    {
        [JsonPropertyName("file")]
        public FileData File { get; set; } = default!;

        [JsonPropertyName("caption")]
        public string? Caption { get; set; }
    }

    /// <summary>
    /// Request for deleting a status.
    /// </summary>
    public record DeleteStatusRequest
    {
        [JsonPropertyName("messageIds")]
        public List<string> MessageIds { get; set; } = new();
    }

    #endregion

    #region [ CHATS ]

    /// <summary>
    /// Represents basic chat data.
    /// </summary>
    public record Chat
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = default!;

        [JsonPropertyName("name")]
        public string Name { get; set; } = default!;

        [JsonPropertyName("picture")]
        public string? Picture { get; set; }

        [JsonPropertyName("lastMessage")]
        public string? LastMessage { get; set; }

        [JsonPropertyName("lastMessageTimestamp")]
        public DateTime? LastMessageTimestamp { get; set; }
    }

    /// <summary>
    /// Represents a higher-level overview of chats.
    /// (Some APIs return less/more data than <see cref="Chat"/>; adjust fields as needed.)
    /// </summary>
    public record ChatOverview
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = default!;

        [JsonPropertyName("name")]
        public string Name { get; set; } = default!;

        [JsonPropertyName("picture")]
        public string? Picture { get; set; }

        [JsonPropertyName("lastMessage")]
        public string? LastMessage { get; set; }

        [JsonPropertyName("lastMessageTimestamp")]
        public DateTime? LastMessageTimestamp { get; set; }
    }

    /// <summary>
    /// Represents the result of retrieving a chat's picture.
    /// </summary>
    public record ChatPicture
    {
        [JsonPropertyName("picture")]
        public string Picture { get; set; } = default!;
    }

    /// <summary>
    /// Represents a single message in a chat.
    /// </summary>
    public record ChatMessage
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = default!;

        [JsonPropertyName("body")]
        public string Body { get; set; } = default!;

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("sender")]
        public string Sender { get; set; } = default!;
    }

    /// <summary>
    /// Request payload for sending (creating) a chat message.
    /// </summary>
    public record CreateChatMessageRequest
    {
        [JsonPropertyName("body")]
        public string Body { get; set; } = default!;
    }

    /// <summary>
    /// Request payload for editing a chat message.
    /// </summary>
    public record EditChatMessageRequest
    {
        [JsonPropertyName("body")]
        public string Body { get; set; } = default!;
    }

    #endregion

    #region [ CONTATS ]

    /// <summary>
    /// Represents a basic contact from the WAHA /api/contacts endpoints.
    /// </summary>
    public record Contact
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = default!;

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("isBusiness")]
        public bool? IsBusiness { get; set; }
    }

    #endregion

    #region [ GROUPS ]

    /// <summary>
    /// Represents a group in the WAHA.
    /// </summary>
    public record Group
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = default!;

        [JsonPropertyName("subject")]
        public string Subject { get; set; } = default!;

        [JsonPropertyName("participants")]
        public List<GroupParticipant>? Participants { get; set; }
    }

    /// <summary>
    /// Participant in a group.
    /// </summary>
    public record GroupParticipant
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = default!;

        [JsonPropertyName("admin")]
        public string? Admin { get; set; }
    }

    /// <summary>
    /// Request to create a new group.
    /// </summary>
    public record CreateGroupRequest
    {
        [JsonPropertyName("subject")]
        public string Subject { get; set; } = default!;

        [JsonPropertyName("participants")]
        public List<string> Participants { get; set; } = new();
    }

    /// <summary>
    /// Request to join a group.
    /// </summary>
    public record JoinGroupRequest
    {
        [JsonPropertyName("code")]
        public string Code { get; set; } = default!;
    }

    /// <summary>
    /// Response from joining a group.
    /// </summary>
    public record JoinGroupResponse
    {
        [JsonPropertyName("gid")]
        public string? Gid { get; set; }

        [JsonPropertyName("subject")]
        public string? Subject { get; set; }
    }

    /// <summary>
    /// Request or response toggling group info/messages admin-only.
    /// </summary>
    public record SettingsSecurityChangeRequest
    {
        [JsonPropertyName("value")]
        public bool Value { get; set; }
    }

    /// <summary>
    /// Request to add or remove participants from group, or promote/demote them.
    /// </summary>
    public record ParticipantsRequest
    {
        [JsonPropertyName("participants")]
        public List<string> Participants { get; set; } = new();
    }

    /// <summary>
    /// Request to set the group description.
    /// </summary>
    public record DescriptionRequest
    {
        [JsonPropertyName("description")]
        public string Description { get; set; } = default!;
    }

    /// <summary>
    /// Request to set the group subject.
    /// </summary>
    public record SubjectRequest
    {
        [JsonPropertyName("subject")]
        public string Subject { get; set; } = default!;
    }

    #endregion

    #region  [ PRESENCE ]

    /// <summary>
    /// Request body for setting presence.
    /// </summary>
    public record SessionPresenceRequest
    {
        [JsonPropertyName("available")]
        public bool Available { get; set; }
    }

    /// <summary>
    /// Presence info for a single chat.
    /// </summary>
    public record ChatPresences
    {
        [JsonPropertyName("chatId")]
        public string ChatId { get; set; } = default!;

        [JsonPropertyName("isOnline")]
        public bool IsOnline { get; set; }
    }

    #endregion

    #region [ LABELS ]

    /// <summary>
    /// A label from WhatsApp Business.
    /// </summary>
    public record Label
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = default!;

        [JsonPropertyName("name")]
        public string Name { get; set; } = default!;
    }

    /// <summary>
    /// Body used for creating or updating a label.
    /// </summary>
    public record LabelRequest
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = default!;
    }

    /// <summary>
    /// Request for setting labels on a chat.
    /// </summary>
    public record SetLabelsRequest
    {
        [JsonPropertyName("labelIds")]
        public List<string> LabelIds { get; set; } = new();
    }

    #endregion

    #region [ OBSERVABILITY ]

    /// <summary>
    /// Basic ping response.
    /// </summary>
    public record PingResponse
    {
        [JsonPropertyName("time")]
        public string Time { get; set; } = default!;
    }

    /// <summary>
    /// Health check response from /health
    /// </summary>
    public record HealthResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = default!;
    }

    /// <summary>
    /// Server version info.
    /// </summary>
    public record Environment
    {
        [JsonPropertyName("version")]
        public string Version { get; set; } = default!;

        [JsonPropertyName("engine")]
        public string Engine { get; set; } = default!;

        [JsonPropertyName("tier")]
        public string Tier { get; set; } = default!;

        [JsonPropertyName("browser")]
        public string Browser { get; set; } = default!;
    }

    /// <summary>
    /// Server status response.
    /// </summary>
    public record ServerStatusResponse
    {
        [JsonPropertyName("uptime")]
        public double UptimeInSeconds { get; set; }

        [JsonPropertyName("pid")]
        public int? Pid { get; set; }
    }

    /// <summary>
    /// Request to stop the server.
    /// </summary>
    public record StopRequest
    {
        [JsonPropertyName("restart")]
        public bool Restart { get; set; }
    }

    /// <summary>
    /// Response from stopping the server.
    /// </summary>
    public record StopResponse
    {
        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }

    #endregion

    #region [ WEBHOOK ]

    /// <summary>
    /// Content from WebHook event
    /// </summary>
    public class WebhookEvent
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("session")]
        public string Session { get; set; }

        [JsonPropertyName("metadata")]
        public Dictionary<string, string> Metadata { get; set; }

        [JsonPropertyName("engine")]
        public string Engine { get; set; }

        [JsonPropertyName("event")]
        public string Event { get; set; }

        [JsonPropertyName("payload")]
        public object Payload { get; set; }

        [JsonPropertyName("me")]
        public SessionUser Me { get; set; }

        [JsonPropertyName("environment")]
        public Environment Environment { get; set; }
    }

    #endregion
}