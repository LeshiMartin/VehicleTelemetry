using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common;

public class BaseEntity
{
    public int Id { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset UpdatedAt { get; init; }

    public static bool operator !(BaseEntity? entity) => entity is null;
    public static bool operator true(BaseEntity? entity) => entity is not null;
    public static bool operator false(BaseEntity? entity) => entity is null;
}


public abstract class BaseEntityMapper<T>: IEntityTypeConfiguration<T> where T: BaseEntity
{
    protected abstract void ConfigureAbstract(EntityTypeBuilder<T> builder);
    public void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Id).IsUnique();
        ConfigureAbstract(builder);
    }
}