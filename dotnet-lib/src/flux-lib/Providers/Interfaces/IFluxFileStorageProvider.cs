using System.Threading.Tasks;

namespace FluxFile.Providers.Interfaces;

/// <summary>
/// Defines the contract for file storage providers.
/// This interface outlines methods for saving, retrieving, and deleting file chunks.
/// It serves as a base for implementing various file storage services, allowing for easy extension
/// of functionality in the future.
/// </summary>
public interface IFluxFileStorageProvider
{
    /// <summary>
    /// Saves a chunk of data to a file with the specified file name.
    /// The file name should be a system-generated name assigned at the beginning of the upload process.
    /// </summary>
    /// <param name="fileName">The system-generated name of the file where the chunk will be saved.</param>
    /// <param name="chunk">The byte array representing the chunk of data to be saved.</param>
    /// <returns>A task that represents the asynchronous save operation, with the file name as its result.</returns>
    Task<string> SaveChunkAsync(string fileName, byte[] chunk);

    /// <summary>
    /// Retrieves all chunks of data that match the specified search pattern.
    /// </summary>
    /// <param name="searchPattern">The search pattern to match files (e.g., "file_name_chunk_*").</param>
    /// <returns>A task that represents the asynchronous retrieval operation, with a byte array containing all combined chunks as its result.</returns>
    Task<byte[]> GetAllChunksAsync(string searchPattern);

    /// <summary>
    /// Deletes the specified chunk file from the storage.
    /// </summary>
    /// <param name="fileName">The name of the file to be deleted.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    Task DeleteChunkAsync(string fileName);

    /// <summary>
    /// Retrieves the paths of all chunk files that match the specified search pattern.
    /// </summary>
    /// <param name="searchPattern">The search pattern to match files (e.g., "file_name_chunk_*").</param>
    /// <returns>A task that represents the asynchronous retrieval operation, with an array of string paths as its result.</returns>
    Task<string[]> GetAllChunkPathsAsync(string searchPattern);
}