namespace Dan.Common.Interfaces;

/// <summary>
/// API Service for fetching entity registries
/// </summary>
[Obsolete("Deprecated, use Dan.Common.Services.ICcrClientService for fetching data from CCR")]
public interface IEntityRegistryApiClientService
{
    /// <summary>
    /// Get entity registry unit
    /// </summary>
    public Task<EntityRegistryUnit?> GetUpstreamEntityRegistryUnitAsync(Uri registryApiUri);
}
