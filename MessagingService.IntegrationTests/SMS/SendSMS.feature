﻿@base @shared @sms
Feature: SendSMS

Background: 
	Given the following api resources exist
	| ResourceName     | DisplayName    | Secret  | Scopes           | UserClaims |
	| messagingService | Messaging REST | Secret1 | messagingService |            |

	Given the following clients exist
	| ClientId      | ClientName     | Secret  | AllowedScopes    | AllowedGrantTypes  |
	| serviceClient | Service Client | Secret1 | messagingService | client_credentials |

	Given I have a token to access the messaging service resource
	| ClientId      | 
	| serviceClient |

@PRTest
Scenario: Send SMS
	Given I send the following SMS Messages
	| Sender     | Destination | Message        |
	| TestSender | 07777777771 | TestSMSMessage |
