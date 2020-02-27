using Npgsql;
using System.Data.Entity;

class NpgSqlConfiguration : DbConfiguration
{
    public NpgSqlConfiguration()
    {
        var name = "Npgsql";

        SetProviderFactory(providerInvariantName: name,
        providerFactory: NpgsqlFactory.Instance);

        SetProviderServices(providerInvariantName: name,
        provider: NpgsqlServices.Instance);

        SetDefaultConnectionFactory(connectionFactory: new NpgsqlConnectionFactory());
    }
}