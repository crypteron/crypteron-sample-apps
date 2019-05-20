
using System;

namespace Crypteron.SampleApps.ConsoleCipherDbEf6CodeFirst.Models
{
    public partial class User
    {
        public int OrderId { get; set; }
        
        public string OrderItem { get; set; }
        
        public DateTime? Timestamp { get; set; }
        
        // For Code-first one can additionally mark properties as secure
        // without renaming the property itself. Unlike the Db-First sample
        // in this Code-first sample app, we also encrypt CustomerName
        [Secure]
        public string CustomerName { get; set; }

        // Searchable encryption also supported with this syntax:
        // [Secure(Opt.Search)]
        public string SecureSearch_CreditCardNumber { get; set; }
        
        public byte[] Secure_SocialSecurityNumber { get; set; }
        
        public string Secure_LegacyPIN { get; set; }
    }
}
