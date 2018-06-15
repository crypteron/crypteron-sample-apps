-- Log into your database and run the scrip below to create the 
-- table for the sample app to work with

CREATE TABLE [dbo].[Users] (
    [OrderId]                           INT             IDENTITY (1, 1) NOT NULL,
    [OrderItem]                         NVARCHAR  (120) NULL,
    [Timestamp]                         DATETIME        NULL,
    [CustomerName]                      NVARCHAR  (120) NULL,
    [SecureSearch_CreditCardNumber]     NVARCHAR  (MAX) NULL,
    [Secure_SocialSecurityNumber]       VARBINARY (MAX) NULL,
    [PIN]                               NVARCHAR  (MAX) NULL,	
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([OrderId] ASC)
);