using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProfileService.Command.Domain;

namespace ProfileService.Command.Infrastructure.Persistence.Configurations;

public class ProfileConfiguration : IEntityTypeConfiguration<Profile>
{
    public void Configure(EntityTypeBuilder<Profile> builder)
    {
        builder.ToTable("profiles");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.EmployeeNumber).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => x.EmployeeNumber).IsUnique();
        builder.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.LastName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.WorkEmail).HasMaxLength(320).IsRequired();
        builder.HasIndex(x => x.WorkEmail).IsUnique();
        builder.HasIndex(x => x.AccountId).IsUnique();
        builder.Property(x => x.PersonalEmail).HasMaxLength(320);
        builder.Property(x => x.PhoneNumber).HasMaxLength(50);
        builder.Property(x => x.Address).HasMaxLength(500);
        builder.Property(x => x.ProfilePictureUrl).HasMaxLength(2000);
        builder.Property(x => x.JobTitle).HasMaxLength(120).IsRequired();
        builder.Property(x => x.Department).HasMaxLength(120).IsRequired();
        builder.Property(x => x.EmploymentType).HasMaxLength(60).IsRequired();
        builder.Property(x => x.OrganizationRole).HasMaxLength(120).IsRequired();
        builder.Property(x => x.EmploymentStatus).HasMaxLength(60).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
    }
}
