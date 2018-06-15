namespace Crypteron.SampleApps.ConsoleCipherDbEf6DbFirst
{
    /// <summary>
    /// We create a secure DB context once and use this instead everywhere
    /// </summary>
    public class SecDbContext : PlainDbContext
    {
        public SecDbContext()
        {
            // Crypteron power-up this Session
            Crypteron.CipherDb.Session.Create(this);
        }
    }
}
