using System.Threading.Tasks;

namespace FluxFile.Services.Interfaces;

public interface IFluxFileUploadService
{
    Task<string> StartUploadAsync(string fileName);
    Task<string> UploadChunkAsync(string fileName, byte[] chunk, long chunkIndex);
    Task CompleteUploadAsync(string fileIdentifier);
    Task CancelUploadAsync(string fileName);
}