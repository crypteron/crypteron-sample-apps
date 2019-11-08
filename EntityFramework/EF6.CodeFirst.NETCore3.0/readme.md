# Crypteron CipherDb for Entity Framework 6.3 on .NET Core 3.x

This is a sample application showing how to use Crypteron CipherDb to encrypt sensitive data using strong encryption and robust key management with Entity Framework 6.3 which now works on .NET Core 3.0, opening up many cross-platform scenarios. This sample used the SQL LocalDb provider for Entity Framework but Crypteron CipherDb can work with any database provider compatible with EF Core (e.g. Azure SQL, MySQL, PostgreSQL, Cosmos Db etc.)

## Sample Database 

This sample uses a localdb SQL database which should be generated the first time you run this. Depending on your environment, you might want to change the connection string inside `App.config` to better suit your needs. The connection string can be any standard Entity Framework 6 connection string; there is nothing special needed to make it Crypteron compatible.

## Support

If you have any questions, please reach out to us at support(at)crypteron.com