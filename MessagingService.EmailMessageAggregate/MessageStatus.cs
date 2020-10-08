namespace MessagingService.EmailMessageAggregate
{
    /// <summary>
    /// 
    /// </summary>
    public enum MessageStatus
    {
        /// <summary>
        /// The not set
        /// </summary>
        NotSet = 0,

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
        /// The bounced
        /// </summary>
        Bounced,

        /// <summary>
        /// The rejected
        /// </summary>
        Rejected,

        /// <summary>
        /// The failed
        /// </summary>
        Failed,

        /// <summary>
        /// The spam
        /// </summary>
        Spam
    }
}