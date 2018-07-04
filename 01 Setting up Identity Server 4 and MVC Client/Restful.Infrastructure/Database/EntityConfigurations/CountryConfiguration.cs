using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restful.Core.Entities;

namespace Restful.Infrastructure.Database.EntityConfigurations
{
    public class CountryConfiguration: IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.Property(x => x.EnglishName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.ChineseName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Abbreviation).HasMaxLength(5);
        }
    }
}
