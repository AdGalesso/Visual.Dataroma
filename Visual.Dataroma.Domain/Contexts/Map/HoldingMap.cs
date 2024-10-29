using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Visual.Dataroma.Domain.Entities;

namespace Visual.Dataroma.Domain.Contexts.Map
{
    public class HoldingMap : IEntityTypeConfiguration<Holding>
    {
        public void Configure(EntityTypeBuilder<Holding> builder)
        {
            builder.ToTable("holding");
        }
    }
}
