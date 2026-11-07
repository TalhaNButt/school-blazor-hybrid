namespace MyHybridApp2.Domain.Common;

/// <summary>
/// Base type for all entities.
/// Cross-cutting columns (e.g. a future SchoolId/TenantId for multi-school support) belong here.
/// </summary>
public abstract class Entity
{
    public Guid Id { get; protected set; } = Guid.CreateVersion7();
}
