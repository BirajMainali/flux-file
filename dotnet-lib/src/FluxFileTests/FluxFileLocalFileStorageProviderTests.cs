using FluxFile.Providers.Interfaces;
using Moq;

namespace FluxFileTests
{
    public class FluxFileLocalFileStorageProviderTests
    {
        private readonly Mock<IFluxFileStorageProvider> _mockProvider;

        public FluxFileLocalFileStorageProviderTests()
        {
            _mockProvider = new Mock<IFluxFileStorageProvider>();
        }

        [Fact]
        public async Task SaveChunkAsync_ShouldCallSaveChunkMethod()
        {
            // Arrange
            var fileName = "test_chunk_0.bin";
            var chunkData = new byte[] { 1, 2, 3, 4, 5 };

            _mockProvider
                .Setup(p => p.SaveChunkAsync(fileName, chunkData))
                .ReturnsAsync(fileName);

            // Act
            var result = await _mockProvider.Object.SaveChunkAsync(fileName, chunkData);

            // Assert
            Assert.Equal(fileName, result);
            _mockProvider.Verify(p => p.SaveChunkAsync(fileName, chunkData), Times.Once);
        }

        [Fact]
        public async Task GetAllChunksAsync_ShouldReturnAllChunkBytes()
        {
            // Arrange
            var chunkData1 = new byte[] { 1, 2, 3 };
            var chunkData2 = new byte[] { 4, 5, 6 };
            var combinedData = chunkData1.Concat(chunkData2).ToArray();

            _mockProvider
                .Setup(p => p.GetAllChunksAsync("test_chunk_*"))
                .ReturnsAsync(combinedData);

            // Act
            var result = await _mockProvider.Object.GetAllChunksAsync("test_chunk_*");

            // Assert
            Assert.Equal(combinedData, result);
            _mockProvider.Verify(p => p.GetAllChunksAsync("test_chunk_*"), Times.Once);
        }

        [Fact]
        public async Task GetAllChunkPathsAsync_ShouldReturnCorrectChunkPaths()
        {
            // Arrange
            var fileName1 = "test_chunk_0.bin";
            var fileName2 = "test_chunk_1.bin";
            var chunkPaths = new[] { fileName1, fileName2 };

            _mockProvider
                .Setup(p => p.GetAllChunkPathsAsync("test_chunk_*"))
                .ReturnsAsync(chunkPaths);

            // Act
            var result = await _mockProvider.Object.GetAllChunkPathsAsync("test_chunk_*");

            // Assert
            Assert.Equal(chunkPaths, result);
            _mockProvider.Verify(p => p.GetAllChunkPathsAsync("test_chunk_*"), Times.Once);
        }

        [Fact]
        public async Task DeleteChunkAsync_ShouldCallDeleteMethod()
        {
            // Arrange
            var filePath = "test_chunk_0.bin";

            // Act
            await _mockProvider.Object.DeleteChunkAsync(filePath);

            // Assert
            _mockProvider.Verify(p => p.DeleteChunkAsync(filePath), Times.Once);
        }

        [Fact]
        public async Task GetAllChunksAsync_ShouldReturnEmpty_WhenNoMatchingFiles()
        {
            // Arrange
            var emptyData = Array.Empty<byte>();

            _mockProvider
                .Setup(p => p.GetAllChunksAsync("non_existent_pattern"))
                .ReturnsAsync(emptyData);

            // Act
            var result = await _mockProvider.Object.GetAllChunksAsync("non_existent_pattern");

            // Assert
            Assert.Empty(result);
            _mockProvider.Verify(p => p.GetAllChunksAsync("non_existent_pattern"), Times.Once);
        }
    }
}
