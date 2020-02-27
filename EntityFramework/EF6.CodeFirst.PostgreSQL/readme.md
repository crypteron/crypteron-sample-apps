# Crypteron CipherDb for Entity Framework 6.3 + Postgres

This is a sample application showing how to use Crypteron CipherDb to encrypt sensitive data using strong encryption and robust key management with Entity Framework 6.3. This sample used the PostgreSQL provider for Entity Framework but Crypteron CipherDb can work with any database provider compatible with Entity Framework (e.g. Azure SQL, MySQL, PostgreSQL, Cosmos Db etc.)

> This **now works on .NET Core 3.0**, opening up many cross-platform scenarios.

## Sample Database 

This sample uses a PostgreSQL database which should be generated the first time you run this. Depending on your environment, you might want to change the connection string inside `App.config` to better suit your needs. The connection string can be any standard Entity Framework 6 connection string; there is nothing special needed to make it Crypteron compatible.

### Comments

Npgsql EF migrations support uses `uuid_generate_v4()` function to generate guids, enable it on your postgres server by

```
psql -U <postgres user>
create extension "uuid-ossp";
```

## Integration

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