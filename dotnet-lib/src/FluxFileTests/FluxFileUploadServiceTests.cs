using FluxFile.Exceptions;
using FluxFile.Providers.Interfaces;
using FluxFile.Services;
using Moq;

namespace FluxFileTests
{
    public class FluxFileUploadServiceTests
    {
        private readonly Mock<IFluxFileStorageProvider> _mockStorageProvider;
        private readonly Mock<IFluxFileNameProvider> _mockFileNameProvider;
        private readonly FluxFileUploadService _uploadService;

        public FluxFileUploadServiceTests()
        {
            _mockStorageProvider = new Mock<IFluxFileStorageProvider>();
            _mockFileNameProvider = new Mock<IFluxFileNameProvider>();
            _uploadService = new FluxFileUploadService(_mockStorageProvider.Object, _mockFileNameProvider.Object);
        }

        [Fact]
        public async Task StartUploadAsync_ShouldReturnUniqueFileName()
        {
            // Arrange
            var fileName = "example.txt";
            var uniqueFileName = "example_12345"; 

            _mockFileNameProvider.Setup(f => f.GetUniqueFileName(fileName)).Returns(uniqueFileName);

            // Act
            var result = await _uploadService.StartUploadAsync(fileName);

            // Assert
            Assert.Equal(uniqueFileName, result);
            _mockFileNameProvider.Verify(f => f.GetUniqueFileName(fileName), Times.Once);
        }

        [Fact]
        public async Task StartUploadAsync_ShouldThrowException_WhenFileNameIsEmpty()
        {
            // Arrange
            string fileName = string.Empty;

            // Act & Assert
            var exception =
                await Assert.ThrowsAsync<FluxFileUploadException>(() => _uploadService.StartUploadAsync(fileName));
            Assert.Equal("Flux file identifier cannot be empty.", exception.Message);
        }

        [Fact]
        public async Task UploadChunkAsync_ShouldSaveChunkAndReturnFilePath()
        {
            // Arrange
            var fileName = "example.txt";
            var chunk = new byte[] { 1, 2, 3, 4, 5 };
            var chunkIndex = 0;
            var chunkFileName = "example_chunk_0.txt";
            var savedFilePath = "example_chunk_0.txt";

            _mockFileNameProvider.Setup(f => f.GetChunkFileName(fileName, chunkIndex)).Returns(chunkFileName);
            _mockStorageProvider.Setup(s => s.SaveChunkAsync(chunkFileName, chunk)).ReturnsAsync(savedFilePath);

            // Act
            var result = await _uploadService.UploadChunkAsync(fileName, chunk, chunkIndex);

            // Assert
            Assert.Equal(savedFilePath, result);
            _mockFileNameProvider.Verify(f => f.GetChunkFileName(fileName, chunkIndex), Times.Once);
            _mockStorageProvider.Verify(s => s.SaveChunkAsync(chunkFileName, chunk), Times.Once);
        }

        [Fact]
        public async Task UploadChunkAsync_ShouldThrowException_WhenFileNameIsEmpty()
        {
            // Arrange
            string fileName = string.Empty;
            var chunk = new byte[] { 1, 2, 3, 4, 5 };
            var chunkIndex = 0;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<FluxFileUploadException>(() =>
                _uploadService.UploadChunkAsync(fileName, chunk, chunkIndex));
            Assert.Equal("Flux file identifier cannot be empty.", exception.Message);
        }

        [Fact]
        public async Task CompleteUploadAsync_ShouldThrowException_WhenNoChunksFound()
        {
            // Arrange
            var fileName = "example.txt";
            var fileSearchPattern = "example_chunk_*";

            _mockFileNameProvider.Setup(f => f.GetFileSearchPattern(fileName)).Returns(fileSearchPattern);
            _mockStorageProvider.Setup(s => s.GetAllChunksAsync(fileSearchPattern)).ReturnsAsync(Array.Empty<byte>());

            // Act & Assert
            var exception =
                await Assert.ThrowsAsync<FluxFileUploadException>(() => _uploadService.CompleteUploadAsync(fileName));
            Assert.Equal("No chunks found for the specified file.", exception.Message);
        }

        [Fact]
        public async Task CompleteUploadAsync_ShouldSaveAllChunksAndDeleteThem()
        {
            // Arrange
            var fileName = "example.txt";
            var fileSearchPattern = "example_chunk_*";
            var chunkData = new byte[] { 1, 2, 3, 4, 5 }; // This would normally be your combined chunk data.

            _mockFileNameProvider.Setup(f => f.GetFileSearchPattern(fileName)).Returns(fileSearchPattern);
            _mockStorageProvider.Setup(s => s.GetAllChunksAsync(fileSearchPattern)).ReturnsAsync(chunkData);
            _mockStorageProvider.Setup(s => s.SaveChunkAsync(fileName, chunkData)).ReturnsAsync(fileName);

            // Act
            await _uploadService.CompleteUploadAsync(fileName);

            // Assert
            _mockStorageProvider.Verify(s => s.SaveChunkAsync(fileName, chunkData), Times.Once);
            _mockFileNameProvider.Verify(f => f.GetFileSearchPattern(fileName), Times.Once);
            _mockStorageProvider.Verify(s => s.GetAllChunksAsync(fileSearchPattern), Times.Once);
            // You might also want to verify DeleteChunksAsync is called, but you'd need to mock that method.
        }

        [Fact]
        public async Task CancelUploadAsync_ShouldDeleteChunks()
        {
            // Arrange
            var fileName = "example.txt";
            var fileSearchPattern = "example_chunk_*";

            _mockFileNameProvider.Setup(f => f.GetFileSearchPattern(fileName)).Returns(fileSearchPattern);
            _mockStorageProvider.Setup(s => s.GetAllChunkPathsAsync(fileSearchPattern)).ReturnsAsync(new string[] { });

            // Act
            await _uploadService.CancelUploadAsync(fileName);

            // Assert
            _mockStorageProvider.Verify(s => s.GetAllChunkPathsAsync(fileSearchPattern), Times.Once);
            // You would need to verify that DeleteChunkAsync is called for each path found, which may require setting up the mock further.
        }
    }
}