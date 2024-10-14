using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluxFile.Providers.Interfaces;

namespace FluxFile.Providers;

public class FluxFileLocalFileStorageProvider : IFluxFileStorageProvider
{
    private readonly string _basePath;

    public FluxFileLocalFileStorageProvider(string basePath)
    {
        _basePath = basePath;
        if (!Directory.Exists(_basePath))
        {
            Directory.CreateDirectory(_basePath);
        }
    }

    public Task<string> SaveChunkAsync(string fileName, byte[] chunk)
    {
        var filePath = Path.Combine(_basePath, fileName);
        File.WriteAllBytes(filePath, chunk);
        return Task.FromResult(fileName);
    }

    public async Task<byte[]> GetAllChunksAsync(string searchPattern)
    {
        var chunkPaths = Directory.GetFiles(_basePath, searchPattern);
        var chunks = chunkPaths.SelectMany(ReadAllBytes).ToArray();
        return await Task.FromResult(chunks);
    }

    public async Task<string[]> GetAllChunkPathsAsync(string searchPattern)
    {
        var chunkPaths = Directory.GetFiles(_basePath, searchPattern);
        return await Task.FromResult(chunkPaths);
    }

    private byte[] ReadAllBytes(string path)
    {
        using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096);
        using var memoryStream = new MemoryStream();
        fileStream.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }

    public Task DeleteChunkAsync(string filePath)
    {
        File.Delete(filePath);
        return Task.CompletedTask;
    }
}