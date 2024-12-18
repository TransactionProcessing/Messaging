using System;
using SimpleResults;

namespace MessagingService.BusinessLogic.RequestHandlers{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Requests;
    using Services;
    using EmailAttachment = Models.EmailAttachment;
    using FileType = Models.FileType;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler{MessagingService.BusinessLogic.Requests.SendEmailRequest, System.String}" />
    public class MessagingRequestHandler : IRequestHandler<EmailCommands.SendEmailCommand, Result<Guid>>,
                                           IRequestHandler<SMSCommands.SendSMSCommand, Result<Guid>>,
                                           IRequestHandler<EmailCommands.ResendEmailCommand,Result>,
                                           IRequestHandler<SMSCommands.ResendSMSCommand, Result>{
        #region Fields

        /// <summary>
        /// The messaging domain service
        /// </summary>
        private readonly IMessagingDomainService MessagingDomainService;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingRequestHandler" /> class.
        /// </summary>
        /// <param name="messagingDomainService">The messaging domain service.</param>
        public MessagingRequestHandler(IMessagingDomainService messagingDomainService){
            this.MessagingDomainService = messagingDomainService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        public async Task<Result<Guid>> Handle(EmailCommands.SendEmailCommand command,
                                 CancellationToken cancellationToken){
            List<EmailAttachment> attachments = new List<EmailAttachment>();

            foreach (Requests.EmailAttachment requestEmailAttachment in command.EmailAttachments){
                attachments.Add(new EmailAttachment{
                                                       FileData = requestEmailAttachment.FileData,
                                                       FileType = this.ConvertFileType(requestEmailAttachment.FileType),
                                                       Filename = requestEmailAttachment.Filename,
                                                   });
            }

            return await this.MessagingDomainService.SendEmailMessage(command.ConnectionIdentifier,
                                                               command.MessageId,
                                                               command.FromAddress,
                                                               command.ToAddresses,
                                                               command.Subject,
                                                               command.Body,
                                                               command.IsHtml,
                                                               attachments,
                                                               cancellationToken);
        }

        public async Task<Result<Guid>> Handle(SMSCommands.SendSMSCommand command,
                                               CancellationToken cancellationToken){
            return await this.MessagingDomainService.SendSMSMessage(command.ConnectionIdentifier,
                                                             command.MessageId,
                                                             command.Sender,
                                                             command.Destination,
                                                             command.Message,
                                                             cancellationToken);
        }

        public async Task<Result> Handle(EmailCommands.ResendEmailCommand command,
                                         CancellationToken cancellationToken){
            return await this.MessagingDomainService.ResendEmailMessage(command.ConnectionIdentifier, command.MessageId, cancellationToken);
        }

        public async Task<Result> Handle(SMSCommands.ResendSMSCommand command, CancellationToken cancellationToken){
            return await this.MessagingDomainService.ResendSMSMessage(command.ConnectionIdentifier, command.MessageId, cancellationToken);
        }

        private FileType ConvertFileType(Requests.FileType emailAttachmentFileType){
            switch(emailAttachmentFileType){
                case Requests.FileType.PDF:
                    return FileType.PDF;
                default:
                    return FileType.None;
            }
        }

        #endregion
    }
}