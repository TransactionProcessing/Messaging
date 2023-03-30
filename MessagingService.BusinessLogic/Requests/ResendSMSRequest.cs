namespace MessagingService.BusinessLogic.Requests;

using System;
using MediatR;

public class ResendSMSRequest : IRequest
{
    #region Constructors

    private ResendSMSRequest(Guid connectionIdentifier,
                             Guid messageId)
    {
        this.ConnectionIdentifier = connectionIdentifier;
        this.MessageId = messageId;
    }

    #endregion

    #region Properties
    
    public Guid ConnectionIdentifier { get; }

    public Guid MessageId { get; }

    #endregion

    #region Methods

    public static ResendSMSRequest Create(Guid connectionIdentifier,
                                          Guid messageId)
    {
        return new ResendSMSRequest(connectionIdentifier, messageId);
    }

    #endregion
}