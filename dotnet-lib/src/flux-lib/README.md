# FluxFile: Chunked File Upload Solution

**FluxFile** is a modular, customizable, and extensible file upload solution designed to efficiently handle large files by breaking them into manageable chunks. It allows integration with various storage systems, such as local file systems or cloud storage (e.g., Azure Blob Storage, AWS S3).

## Table of Contents

- [Features](#key-features)
- [Core Components](#core-components)
  - [IFluxFileStorageProvider](#1-ifluxfilestoragprovider)
  - [IFluxFileUploadService](#2-ifluxfileuploadservice)
- [API Endpoints](#api-endpoints)
  - [Starting an Upload](#1-starting-an-upload)
  - [Uploading Chunks](#2-uploading-chunks)
  - [Completing the Upload](#3-completing-the-upload)
- [Customizing Storage Providers](#customizing-storage-providers)

## Features

- **Chunked Uploads**: Efficiently uploads large files in smaller chunks to minimize memory and network overloads.
- **Customizable Storage Providers**: Easily switch between different storage backends.
- **Pluggable Architecture**: Core services and storage providers are interchangeable, providing flexibility for specific use cases.

---

## Configuration

```csharp
builder.Services.AddFluxFile("local-fs-folder-or-null-or-empty-if-could-service");
```

## Core Components

### 1. `IFluxFileStorageProvider`

This interface defines the contract for any file storage system used to save, retrieve, and manage file chunks.

```csharp
public interface IFluxFileStorageProvider
{
    Task<string> SaveChunkAsync(string fileName, byte[] chunk);
    Task<byte[]> GetAllChunksAsync(string searchPattern);
    Task DeleteChunkAsync(string fileName);
    Task<string[]> GetAllChunkPathsAsync(string searchPattern);
}
```

### 2. `IFluxFileUploadService`

This service manages the high-level file upload process, working with the storage provider to handle chunk uploads, completion, and cancellation.

```csharp
public interface IFluxFileUploadService
{
    Task<string> StartUploadAsync(string fileName);
    Task<string> UploadChunkAsync(string fileName, byte[] chunk, long chunkIndex);
    Task CompleteUploadAsync(string fileName);
    Task CancelUploadAsync(string fileName);
}
```

---

## API Endpoints

### 1. **Starting an Upload**

Initialize the upload process and generate a unique identifier for the upload session.

```csharp
app.MapPost("/upload/start", async ([FromForm] string fileName, IFluxFileUploadService uploadService) =>
{
    var uniqueFileName = await uploadService.StartUploadAsync(fileName);
    return Results.Ok(uniqueFileName);
}).DisableAntiforgery();
```

### 2. **Uploading Chunks**

Upload each chunk sequentially to the backend. 

```csharp
app.MapPost("/upload", async ([FromForm] string fileName, [FromForm] IFormFile file, [FromForm] long index,
    IFluxFileUploadService uploadService) =>
{
    var chunkFilePath = await uploadService.UploadChunkAsync(fileName, file.ToBytes(), index);
    return Results.Ok(chunkFilePath);
}).DisableAntiforgery();
```

### 3. **Completing the Upload**

Finalize the upload by combining the chunks into the complete file.

```csharp
app.MapPost("/upload/complete", async ([FromForm] string fileName, IFluxFileUploadService uploadService) =>
{
    await uploadService.CompleteUploadAsync(fileName);
    return Results.Ok();
}).DisableAntiforgery();
```

---

## Customizing Storage Providers

Implement the `IFluxFileStorageProvider` interface to integrate with any storage solution.

### Example: Azure Blob Storage Provider

```csharp
public class AzureBlobStorageProvider : IFluxFileStorageProvider
{
    // Implementation details
}
```

### Registering the Provider

Register the storage provider during application setup:

```csharp
builder.Services.AddTransient<IFluxFileStorageProvider>(provider => 
{
    // Configure the Azure provider
});
```