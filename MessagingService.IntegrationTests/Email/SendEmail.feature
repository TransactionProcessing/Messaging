@base @shared @email
Feature: SendEmail

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
Scenario: Send Email
	Given I send the following Email Messages
	| FromAddress               | ToAddresses                                       | Subject      | Body      | IsHtml |
	| fromaddress@testemail.com | toaddress1@testemail.com                          | Test Email 1 | Test Body | true   |
	| fromaddress@testemail.com | toaddress1@testemail.com,toaddress2@testemail.com | Test Email 1 | Test Body | true   |
