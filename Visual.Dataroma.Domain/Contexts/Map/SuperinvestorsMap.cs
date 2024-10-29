using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Visual.Dataroma.Domain.Entities;

namespace Visual.Dataroma.Domain.Contexts.Map
{
    public class SuperinvestorsMap : IEntityTypeConfiguration<Superinvestor>
    {
        public void Configure(EntityTypeBuilder<Superinvestor> builder)
        {
            builder.ToTable("superinvestor");

            builder.HasMany(s => s.Holdings)
                .WithOne()
                .HasForeignKey(s => s.SuperinvestorId);
        }
    }
}
