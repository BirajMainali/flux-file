using System.IO;
using Microsoft.AspNetCore.Http;

namespace FluxFile.Extensions;

public static class FormFileExtensions
{
    public static byte[] ToBytes(this IFormFile file)
    {
        using var memoryStream = new MemoryStream();
        file.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }
}