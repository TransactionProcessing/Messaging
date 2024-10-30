@base @shared @email
Feature: SendEmail

Background: 
	
	Given I create the following api scopes
	| Name             | DisplayName          | Description                |
	| messagingService | Messaging REST Scope | A scope for Messaging REST |

	Given the following api resources exist
	| Name     | DisplayName    | Secret  | Scopes           | UserClaims |
	| messagingService | Messaging REST | Secret1 | messagingService |            |

	Given the following clients exist
	| ClientId      | ClientName     | Secret  | Scopes    | GrantTypes  |
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

#@PRTest
#Scenario: Resend Email
#	Given I send the following Email Messages
#	| FromAddress               | ToAddresses                                       | Subject      | Body      | IsHtml |
#	| fromaddress@testemail.com | toaddress1@testemail.com                          | Test Email 1 | Test Body | true   |
#	| fromaddress@testemail.com | toaddress1@testemail.com,toaddress2@testemail.com | Test Email 1 | Test Body | true   |
#	When I resend the following messages
#	| ToAddresses                                       |
#	| toaddress1@testemail.com                          |
#	| toaddress1@testemail.com,toaddress2@testemail.com |