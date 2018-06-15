using System;

namespace Crypteron.SampleApps.ConsoleCipherDbNh4.Domain
{
    public class NUser
    {
        public virtual int OrderId { get; set; }
        public virtual string OrderItem { get; set; }
        public virtual DateTime? Timestamp { get; set; }
        public virtual string CustomerName { get; set; }
        public virtual string SecureSearch_CreditCardNumber { get; set; }
        public virtual byte[] Secure_SocialSecurityNumber { get; set; }
        public virtual string Secure_LegacyPIN { get; set; }
    }
}
