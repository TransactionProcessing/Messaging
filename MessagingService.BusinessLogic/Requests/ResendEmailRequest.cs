namespace MessagingService.BusinessLogic.Requests;

using System;
using MediatR;

public class ResendEmailRequest : IRequest<String>
{
    #region Constructors

    private ResendEmailRequest(Guid connectionIdentifier,
                               Guid messageId)
    {
        this.ConnectionIdentifier = connectionIdentifier;
        this.MessageId = messageId;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the connection identifier.
    /// </summary>
    /// <value>
    /// The connection identifier.
    /// </value>
    public Guid ConnectionIdentifier { get; }

    /// <summary>
    /// Gets the message identifier.
    /// </summary>
    /// <value>
    /// The message identifier.
    /// </value>
    public Guid MessageId { get; }

    #endregion

    #region Methods

    public static ResendEmailRequest Create(Guid connectionIdentifier,
                                            Guid messageId)
    {
        return new ResendEmailRequest(connectionIdentifier, messageId);
    }

    #endregion
}