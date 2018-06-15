using System.Data.Entity.ModelConfiguration;

namespace Crypteron.SampleApps.ConsoleCipherDbEf6CodeFirst.Models.Mapping
{
    public class UserMap : EntityTypeConfiguration<User>
    {
        public UserMap()
        {
            // Primary Key
            HasKey(t => t.OrderId);

            // Properties
            Property(t => t.OrderItem)
                .HasMaxLength(120);

            Property(t => t.CustomerName)
                .HasMaxLength(120);

            Property(t => t.Secure_SocialSecurityNumber)
                .HasMaxLength(120);

            Property(t => t.Secure_LegacyPIN)
                .HasMaxLength(120);

            // Table & Column Mappings
            ToTable("Users");
            Property(t => t.OrderId).HasColumnName("OrderId");
            Property(t => t.OrderItem).HasColumnName("OrderItem");
            Property(t => t.Timestamp).HasColumnName("Timestamp");
            Property(t => t.CustomerName).HasColumnName("CustomerName");
            Property(t => t.SecureSearch_CreditCardNumber).HasColumnName("SecureSearch_CreditCardNumber");
            Property(t => t.Secure_SocialSecurityNumber).HasColumnName("Secure_SocialSecurityNumber");
            Property(t => t.Secure_LegacyPIN).HasColumnName("PIN");
        }
    }
}
