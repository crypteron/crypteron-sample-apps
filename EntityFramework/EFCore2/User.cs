namespace Crypteron.SampleApps.EFCore2
{
    public class User
    {
        public int UserId { get; set; }

        public string FullName { get; set; }

        // example: support database side, exact-match searchable encryption 
        //          on this field 
        // var searchPrefix = SecureSearch.GetPrefix("1234-5678-9012-3456");
        // var foundUser = db.Users.Where(p => p.CreditCardNumber.StartsWith(searchPrefix)
        // read more at: https://www.crypteron.com/blog/practical-searchable-encryption-and-security/
        [Secure(Opt.Search)]
        public string CreditCardNumber { get; set; }

        [Secure]
        public string SocialSecurityNumber { get; set; }

        [Secure]
        public byte[] FacePhoto { get; set; }
    }
}
