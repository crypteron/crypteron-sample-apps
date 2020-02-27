# Crypteron CipherDb for Entity Framework 6.3 + Postgres

This is a sample application showing how to use Crypteron CipherDb to encrypt sensitive data using strong encryption and robust key management with Entity Framework 6.3. This sample used the PostgreSQL provider for Entity Framework but Crypteron CipherDb can work with any database provider compatible with Entity Framework (e.g. Azure SQL, MySQL, PostgreSQL, Cosmos Db etc.)

## Run the demo

1. Signup for free in seconds at https://my.crypteron.com to get an `AppSecret`. Put it inside the `Program.cs` placeholder.
2. Edit `App.config` with a valid connection string for your PostgreSQL server. The sample database is auto-created on first run if it's missing.
3. Run the app by `dotnet run` from command terminal. Visual studio users can right-clicking the project -> debug -> start new instance

## Integration Explained

You can see the integration points here

### Mark data as sensitive

In typical Crypteron fashion, put `[Secure]` in front the data bearing properties or rename them to have the `Secure_` prefix in their name. For example here:

```
public class User
{
    ...

    [Secure]
    public string CustomerName { get; set; }

    public byte[] Secure_SocialSecurityNumber { get; set; }

	...
}
```

### Activate Crypteron CipherDB on Entity Framework

Call `CipherDb.Session.Create` on your EF DbContext as follows ...

```
public partial class SecDbContext : DbContext
{

    public SecDbContext() : base("Name=YourConnectionStringName")
    {
        // Crypteron CipherDB power-up this session
        CipherDb.Session.Create(this);
    }
}
```

### Done !

That's it! You have data encryption, key management, key rotations, access control rules revocations etc all done. What typically takes months/years to design, build, test and maintain is done in under 10 minutes. 

That's the power of Crypteron.

## Documentation 

You can find the official documentation at https://www.crypteron.com/docs/

## Support

If you have any questions, please reach out to us at support(at)crypteron.com
