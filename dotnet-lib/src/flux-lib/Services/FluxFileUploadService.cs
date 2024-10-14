using System.Linq;
using System.Threading.Tasks;
using FluxFile.Exceptions;
using FluxFile.Providers.Interfaces;
using FluxFile.Services.Interfaces;

namespace FluxFile.Services;

/// <summary>
/// Provides functionality for handling file uploads, including starting uploads, 
/// uploading chunks, completing uploads, and canceling uploads.
/// This service interacts with storage providers to save file chunks and manage upload processes.
/// </summary>
public class FluxFileUploadService : IFluxFileUploadService
{
    private readonly IFluxFileStorageProvider _storageProvider;
    private readonly IFluxFileNameProvider _fileNameProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="FluxFileUploadService"/> class.
    /// </summary>
    /// <param name="storageProvider">An instance of <see cref="IFluxFileStorageProvider"/> for managing file storage operations.</param>
    /// <param name="fileNameProvider">An instance of <see cref="IFluxFileNameProvider"/> for generating file names.</param>
    public FluxFileUploadService(
        IFluxFileStorageProvider storageProvider,
        IFluxFileNameProvider fileNameProvider)
    {
        _storageProvider = storageProvider;
        _fileNameProvider = fileNameProvider;
    }

    /// <summary>
    /// Starts the upload process for a file and generates a unique file name.
    /// </summary>
    /// <param name="fileName">The original name of the file being uploaded.</param>
    /// <returns>A task representing the asynchronous operation, with the unique file name as its result.</returns>
    /// <exception cref="FluxFileUploadException">Thrown when the provided file name is null or empty.</exception>
    public async Task<string> StartUploadAsync(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            throw new FluxFileUploadException("Flux file identifier cannot be empty.");
        }

        var uniqueFileName = _fileNameProvider.GetUniqueFileName(fileName);
        return await Task.FromResult(uniqueFileName);
    }

    /// <summary>
    /// Uploads a chunk of data for a specified file.
    /// </summary>
    /// <param name="fileName">The name of the file being uploaded.</param>
    /// <param name="chunk">The byte array representing the data chunk to upload.</param>
    /// <param name="chunkIndex">The index of the chunk in the overall file.</param>
    /// <returns>A task representing the asynchronous operation, with the saved chunk file name as its result.</returns>
    /// <exception cref="FluxFileUploadException">Thrown when the provided file name is null or empty.</exception>
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

    /// <summary>
    /// Completes the upload process by combining all uploaded chunks into a single file.
    /// </summary>
    /// <param name="fileName">The name of the original file being uploaded.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="FluxFileUploadException">Thrown when no chunks are found for the specified file.</exception>
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

    /// <summary>
    /// Cancels the upload process and deletes all chunks associated with the specified file.
    /// </summary>
    /// <param name="fileName">The name of the file being canceled.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task CancelUploadAsync(string fileName)
    {
        var fileSearchPattern = _fileNameProvider.GetFileSearchPattern(fileName);
        await DeleteChunksAsync(fileSearchPattern);
    }

    /// <summary>
    /// Deletes all chunk files that match the specified search pattern.
    /// </summary>
    /// <param name="fileSearchPattern">The search pattern used to identify chunk files to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task DeleteChunksAsync(string fileSearchPattern)
    {
        var chunksPaths = await _storageProvider.GetAllChunkPathsAsync(fileSearchPattern);
        foreach (var fileName in chunksPaths)
        {
            await _storageProvider.DeleteChunkAsync(fileName);
        }
    }
}