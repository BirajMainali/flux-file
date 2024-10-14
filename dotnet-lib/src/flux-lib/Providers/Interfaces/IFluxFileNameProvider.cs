namespace FluxFile.Providers.Interfaces;

public interface IFluxFileNameProvider
{
    string GetUniqueFileName(string fileName);
    string GetFileSearchPattern(string fileName);
    string GetChunkFileName(string fileName, long chunkIndex);    
}
