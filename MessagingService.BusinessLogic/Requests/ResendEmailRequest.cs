﻿namespace MessagingService.BusinessLogic.Requests;

using System;
using MediatR;

public class ResendEmailRequest : IRequest
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

    public Guid ConnectionIdentifier { get; }

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