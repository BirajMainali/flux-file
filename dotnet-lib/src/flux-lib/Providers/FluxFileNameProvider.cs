using System;
using System.Collections.Concurrent;
using System.IO;
using FluxFile.Extensions;
using FluxFile.Providers.Interfaces;

namespace FluxFile.Providers;

public class FluxFileNameProvider : IFluxFileNameProvider
{
    private ConcurrentDictionary<string, bool> UsedIdentifier { get; set; } = new();

    public virtual string GetUniqueFileName(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        var uniqueIdentifier = GenerateNewIdentifier();
        return $"{fileNameWithoutExtension}_{uniqueIdentifier}{extension}".ToSnakeCase();
    }

    public virtual string GetFileSearchPattern(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        if (string.IsNullOrEmpty(extension))
        {
            throw new ArgumentException("File name must have an extension.");
        }

        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        return $"{fileNameWithoutExtension.ToSnakeCase()}_chunk_*";
    }

    public string GetChunkFileName(string fileName, long chunkIndex)
    {
        var extension = Path.GetExtension(fileName);
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        return $"{fileNameWithoutExtension.ToSnakeCase()}_chunk_{chunkIndex}{extension}";
    }

    protected virtual string GetUniqueIdentifier(string identifier)
    {
        if (!UsedIdentifier.TryAdd(identifier, true))
        {
            identifier = GenerateNewIdentifier();
        }

        UsedIdentifier.TryAdd(identifier, true);
        return identifier;
    }

    protected virtual string GenerateNewIdentifier()
    {
        var newIdentifier = Guid.NewGuid().ToString("N");
        return GetUniqueIdentifier(newIdentifier);
    }
}