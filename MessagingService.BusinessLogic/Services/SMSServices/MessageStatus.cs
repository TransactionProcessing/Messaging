namespace MessagingService.BusinessLogic.Services.SMSServices
{
    public enum MessageStatus
    {
        /// <summary>
        /// The not set
        /// </summary>
        NotSet = 0,

        /// <summary>
        /// The in progress
        /// </summary>
        InProgress,

        /// <summary>
        /// The sent
        /// </summary>
        Sent,

        /// <summary>
        /// The delivered
        /// </summary>
        Delivered,

        /// <summary>
        /// The expired
        /// </summary>
        Expired,

        /// <summary>
        /// The rejected
        /// </summary>
        Rejected,
        /// <summary>
        /// The undeliverable
        /// </summary>
        Undeliverable,

        /// <summary>
        /// The incoming
        /// </summary>
        Incoming
    }
}