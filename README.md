# FluxFile: Chunked File Upload Solution

**FluxFile** is a modular, customizable, and extensible file upload solution designed to efficiently handle large files by breaking them into manageable chunks. It allows integration with various storage systems, such as local file systems or cloud storage (e.g., Azure Blob Storage, AWS S3).

## Table of Contents

- [Key Features](#key-features)
- [Core Components](#core-components)
  - [IFluxFileStorageProvider](#1-ifluxfilestoragprovider)
  - [IFluxFileUploadService](#2-ifluxfileuploadservice)
- [API Endpoints](#api-endpoints)
  - [Starting an Upload](#1-starting-an-upload)
  - [Uploading Chunks](#2-uploading-chunks)
  - [Completing the Upload](#3-completing-the-upload)
- [Customizing Storage Providers](#customizing-storage-providers)
- [Frontend Integration: useChunkedFileUpload](#frontend-integration-usechunkedfileupload)
  - [Parameters](#parameters)
  - [Example Usage](#example-usage)
- [Getting Started](#getting-started)

---

## Key Features

- **Chunked Uploads**: Efficiently uploads large files in smaller chunks to minimize memory and network overloads.
- **Customizable Storage Providers**: Easily switch between different storage backends.
- **Pluggable Architecture**: Core services and storage providers are interchangeable, providing flexibility for specific use cases.

---

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
    Task<string> UploadChunkAsync(string fileIdentifier, byte[] chunk, long chunkIndex);
    Task CompleteUploadAsync(string fileIdentifier);
    Task CancelUploadAsync(string fileIdentifier);
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

---

## Frontend Integration: `useChunkedFileUpload`

`useChunkedFileUpload` is a custom hook designed for handling chunked file uploads in JavaScript applications.

### Parameters

- **`chunkSize`**: *(number)* Size of each chunk in bytes (default: `1 MB`).
- **`uploadDelay`**: *(function)* A function that returns a promise to introduce a delay between chunk uploads.
- **`initializeUpload`**: *(function)* Initializes the upload process and returns a unique file name.
- **`uploadChunk`**: *(function)* Handles the upload of each chunk.
- **`finalizeUpload`**: *(function)* Called when the upload is complete.
- **`reportProgress`**: *(function)* Reports upload progress as a percentage.
- **`handleError`**: *(function)* Handles errors during the upload process.

### Example Usage

```javascript
const { startUpload } = useChunkedFileUpload({
    initializeUpload: async (fileName) => {
        const response = await fetch('/upload/start', {
            method: 'POST',
            body: JSON.stringify({ fileName }),
            headers: { 'Content-Type': 'application/json' }
        });
        return await response.json(); // Return the unique file name
    },
    uploadChunk: async (uniqueFileName, chunkData, chunkIndex) => {
        const formData = new FormData();
        formData.append('fileName', uniqueFileName);
        formData.append('file', chunkData);
        formData.append('index', chunkIndex);
        
        await fetch('/upload', {
            method: 'POST',
            body: formData
        });
    },
    finalizeUpload: async (uniqueFileName) => {
        await fetch('/upload/complete', {
            method: 'POST',
            body: JSON.stringify({ fileName: uniqueFileName }),
            headers: { 'Content-Type': 'application/json' }
        });
    },
    reportProgress: (progress) => {
        console.log(`Upload progress: ${progress}%`);
    },
    handleError: (error) => {
        console.error(`Upload error: ${error}`);
    }
});

// Attach an event listener to a file input
document.getElementById('fileInput').addEventListener('change', (event) => {
    const file = event.target.files[0];
    if (file) {
        startUpload(file).then(() => {
            console.log('Upload complete!');
        }).catch((error) => {
            console.error('Upload failed:', error);
        });
    }
});
```

---

## Getting Started

1. **Clone the Repository**: Integrate the **FluxFile** system into your project.
2. **Implement a Storage Provider**: Create your storage provider to work with your chosen storage backend.
3. **Register Services**: Register the necessary services in your application setup.
4. **Set Up API Endpoints**: Utilize the provided API endpoints for handling uploads.
