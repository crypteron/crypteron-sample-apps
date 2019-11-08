namespace Crypteron.SampleApps.EF6.DbFirst
{
    /// <summary>
    /// We create a secure DB context once and use this instead everywhere
    /// </summary>
    public class SecDbContext : PlainDbContext
    {
        public SecDbContext()
        {
            // Crypteron power-up this Session
            CipherDb.Session.Create(this);
        }
    }
}
