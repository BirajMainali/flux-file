using System.Threading.Tasks;

namespace FluxFile.Providers.Interfaces;

public interface IFluxFileStorageProvider
{
    Task<string> SaveChunkAsync(string fileName, byte[] chunk);
    Task<byte[]> GetAllChunksAsync(string searchPattern);
    Task DeleteChunkAsync(string fileName);
    Task<string[]> GetAllChunkPathsAsync(string searchPattern);
}