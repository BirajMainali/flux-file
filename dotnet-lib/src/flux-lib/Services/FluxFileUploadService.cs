using System.Linq;
using System.Threading.Tasks;
using FluxFile.Exceptions;
using FluxFile.Providers.Interfaces;
using FluxFile.Services.Interfaces;

namespace FluxFile.Services;

public class FluxFileUploadService : IFluxFileUploadService
{
    private readonly IFluxFileStorageProvider _storageProvider;
    private readonly IFluxFileNameProvider _fileNameProvider;

    public FluxFileUploadService(
        IFluxFileStorageProvider storageProvider,
        IFluxFileNameProvider fileNameProvider)
    {
        _storageProvider = storageProvider;
        _fileNameProvider = fileNameProvider;
    }

    public async Task<string> StartUploadAsync(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            throw new FluxFileUploadException("Flux file identifier cannot be empty.");
        }

        var uniqueFileName = _fileNameProvider.GetUniqueFileName(fileName);
        return await Task.FromResult(uniqueFileName);
    }

    public virtual async Task<string> UploadChunkAsync(string fileName, byte[] chunk, long chunkIndex)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            throw new FluxFileUploadException("Flux file identifier cannot be empty.");
        }

        var chunkFileName = _fileNameProvider.GetChunkFileName(fileName, chunkIndex);
        var filePath = await _storageProvider.SaveChunkAsync(chunkFileName, chunk);
        return filePath;
    }

    public async Task CompleteUploadAsync(string fileName)
    {
        var fileSearchPattern = _fileNameProvider.GetFileSearchPattern(fileName);
        var chunks = await _storageProvider.GetAllChunksAsync(searchPattern: fileSearchPattern);
        if (!chunks.Any())
        {
            throw new FluxFileUploadException("No chunks found for the specified file.");
        }

        await _storageProvider.SaveChunkAsync(fileName, chunks);
        await DeleteChunksAsync(fileSearchPattern);
    }

    public async Task CancelUploadAsync(string fileName)
    {
        var fileSearchPattern = _fileNameProvider.GetFileSearchPattern(fileName);
        await DeleteChunksAsync(fileSearchPattern);
    }


    private async Task DeleteChunksAsync(string fileSearchPattern)
    {
        var chunksPaths = await _storageProvider.GetAllChunkPathsAsync(fileSearchPattern);
        foreach (var fileName in chunksPaths)
        {
            await _storageProvider.DeleteChunkAsync(fileName);
        }
    }
}