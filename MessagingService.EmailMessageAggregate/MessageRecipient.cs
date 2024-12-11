﻿namespace MessagingService.EmailMessageAggregate
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public class MessageRecipient
    {
        #region Constructors

        #endregion

        #region Properties

        /// <summary>
        /// Converts to address.
        /// </summary>
        /// <value>
        /// To address.
        /// </value>
        public String ToAddress { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Creates the specified to address.
        /// </summary>
        /// <param name="toAddress">To address.</param>
        public void Create(String toAddress)
        {
            this.ToAddress = toAddress;
        }

        #endregion
    }
}