# Crypteron CipherDb for Entity Framework Core 2.x

This is a sample application showing how to use Crypteron CipherDb to encrypt sensitive data using strong encryption and robust key management with Entity Framework Core 2.x. This sample uses the SQLite provider for Entity Framework but Crypteron CipherDb can work with any database provider compatible with EF Core (e.g. Azure SQL, MySQL, PostgreSQL, Cosmos Db etc.)

## Integration

You can see the integration points here

### Mark data as sensitive

In typical Crypteron fashion, put `[Secure]` in front the data bearing properties. For example here:

```
public class User
{
    ...

    [Secure(Opt.Search)]
    public string CreditCardNumber { get; set; }

    [Secure]
    public string SocialSecurityNumber { get; set; }

    [Secure]
    public byte[] FacePhoto { get; set; }
}
```

### Activate Crypteron CipherDB on EF Core

Call `CipherDb.EFCore3.Entities.Activate` on your EF Core DbContext as follows ...

```
public class AppDbContext : DbContext
{
    ...

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // NOTE: Crypteron.CrypteronConfig.Config.MyCrypteronAccount.AppSecret
        //       must be configured before this is executed. Depending on your 
        //       application logic, this configuration can be done using 
        //       dependency-injection or a factory method or through 
        //       the dbcontext's constructor
        CipherDb.EFCore3.Entities.Activate(modelBuilder);
    }
}
```

### Done !

That's it! You have data encryption, key management, key rotations, access control rules revocations etc all done. What typically takes months/years to design, build, test and maintain is done in under 10 minutes. 

That's the power of Crypteron.

## Configuration

In .NET Core, there are many ways to configure the database connection string or the Crypteron AppSecret. Plus one may have dependency injection and additional code to support EF Core tools (e.g. migrations). This illustrative sample app doesn't cover every combination so please adjust to your situation as needed. The most important thing you need to know is that the Crypteron AppSecret *must* be initialized *before* using the EF Core DbContext is used by your application or by EF Core tools.

## Output

```
Adding user to database ...
---------------------------
{
  "UserId": 1,
  "FullName": "Quincy",
  "CreditCardNumber": "7960-6868-2186-7230",
  "SocialSecurityNumber": "319-63-1926",
  "FacePhoto": "ceFT+Q65dS3mH0P9k0U0egpLpD8="
}

Decrypted user : read from database (access thru Entity Framework + Crypteron CipherDb) ...
--------------------------------------------------------------------------------------------
{
  "UserId": 1,
  "FullName": "Quincy",
  "CreditCardNumber": "7960-6868-2186-7230",
  "SocialSecurityNumber": "319-63-1926",
  "FacePhoto": "ceFT+Q65dS3mH0P9k0U0egpLpD8="
}

Encrypted user : access without Crypteron ...
----------------------------------------------
{
  "UserId": 1,
  "FullName": "Quincy",
  "CreditCardNumber": "zbMAAAIAGHcZ9mKUpGK2cEkvaAAAACB7Dsr3UzUvSxY2w0jc7pKqskFTFSyxn56yE/d2dpEJsTi3",
  "SocialSecurityNumber": "zbMAAAIAGLq4zOi3cxWqjwBHTQAAANk/JfZNAStqRthy5Y9IrQDcSpffhSwXzTkrjw==",
  "FacePhoto": "zbMAAAIAGCeXVFCfjNS3TVkZwgAAABLK290iTVvYjjrs4rkj83tH411JCrLcTZm8K+7ZIe5nuiyJ3w=="
}

You can also open the SQLite *.db file in the \bin\Debug\netcoreapp2.1\ folder to verify data-at-rest encryption ...

Press enter to exit ...
```

## Support

If you have any questions, please reach out to us at support(at)crypteron.com