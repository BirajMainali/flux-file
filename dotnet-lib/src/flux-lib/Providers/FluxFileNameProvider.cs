using System;
using System.Collections.Concurrent;
using System.IO;
using FluxFile.Extensions;
using FluxFile.Providers.Interfaces;

namespace FluxFile.Providers;

/// <summary>
/// Provides functionality for generating unique file names and search patterns for file chunks.
/// This class ensures that file names are unique by appending a unique identifier to the original file name.
/// It also generates search patterns for chunked files based on the original file name.
/// </summary>
public class FluxFileNameProvider : IFluxFileNameProvider
{
    private ConcurrentDictionary<string, bool> UsedIdentifier { get; set; } = new();

    /// <summary>
    /// Generates a unique file name by appending a unique identifier to the original file name.
    /// The identifier is generated using a GUID, and the file name is converted to snake_case format.
    /// </summary>
    /// <param name="fileName">The original file name for which a unique name is to be generated.</param>
    /// <returns>A unique file name as a string.</returns>
    public virtual string GetUniqueFileName(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        var uniqueIdentifier = GenerateNewIdentifier();
        return $"{fileNameWithoutExtension}_{uniqueIdentifier}{extension}".ToSnakeCase();
    }

    /// <summary>
    /// Generates a search pattern for chunked files based on the original file name.
    /// The search pattern includes a wildcard for chunk identifiers.
    /// </summary>
    /// <param name="fileName">The original file name used to generate the search pattern.</param>
    /// <returns>A search pattern string for chunked files.</returns>
    /// <exception cref="ArgumentException">Thrown when the provided file name does not have an extension.</exception>
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

    /// <summary>
    /// Generates the chunk file name based on the original file name and the index of the chunk.
    /// The chunk file name is formatted as "{original_file_name}_chunk_{index}{extension}".
    /// </summary>
    /// <param name="fileName">The original file name used to generate the chunk file name.</param>
    /// <param name="chunkIndex">The index of the chunk.</param>
    /// <returns>The generated chunk file name as a string.</returns>
    public string GetChunkFileName(string fileName, long chunkIndex)
    {
        var extension = Path.GetExtension(fileName);
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        return $"{fileNameWithoutExtension.ToSnakeCase()}_chunk_{chunkIndex}{extension}";
    }

    /// <summary>
    /// Checks if a given identifier is unique. If not, it generates a new unique identifier.
    /// This method is used internally to ensure that the identifiers remain unique.
    /// </summary>
    /// <param name="identifier">The identifier to check for uniqueness.</param>
    /// <returns>A unique identifier as a string.</returns>
    protected virtual string GetUniqueIdentifier(string identifier)
    {
        if (!UsedIdentifier.TryAdd(identifier, true))
        {
            identifier = GenerateNewIdentifier();
        }

        UsedIdentifier.TryAdd(identifier, true);
        return identifier;
    }

    /// <summary>
    /// Generates a new unique identifier using a GUID.
    /// The identifier is processed to ensure it is unique by calling <see cref="GetUniqueIdentifier"/> method.
    /// </summary>
    /// <returns>A new unique identifier as a string.</returns>
    protected virtual string GenerateNewIdentifier()
    {
        var newIdentifier = Guid.NewGuid().ToString("N");
        return GetUniqueIdentifier(newIdentifier);
    }
}