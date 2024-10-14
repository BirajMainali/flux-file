using FluxFile.Providers;

namespace FluxFileTests
{
    public class FluxFileNameProviderTests
    {
        private readonly FluxFileNameProvider _fileNameProvider;

        public FluxFileNameProviderTests()
        {
            _fileNameProvider = new FluxFileNameProvider();
        }

        [Fact]
        public void GetUniqueFileName_ShouldReturnUniqueFileNameInSnakeCase()
        {
            // Arrange
            var fileName = "TestFile.txt";
            var expectedPrefix = "test_file_";

            // Act
            var uniqueFileName = _fileNameProvider.GetUniqueFileName(fileName);

            // Assert
            Assert.StartsWith(expectedPrefix, uniqueFileName);
            Assert.EndsWith(".txt", uniqueFileName);
            Assert.Contains("_", uniqueFileName); // Ensure snake case
        }

        [Fact]
        public void GetFileSearchPattern_ShouldReturnSearchPattern_WhenValidFileName()
        {
            // Arrange
            var fileName = "TestFile.txt";
            var expectedPattern = "test_file_chunk_*";

            // Act
            var searchPattern = _fileNameProvider.GetFileSearchPattern(fileName);

            // Assert
            Assert.Equal(expectedPattern, searchPattern);
        }

        [Fact]
        public void GetFileSearchPattern_ShouldThrowArgumentException_WhenNoFileExtension()
        {
            // Arrange
            var fileName = "TestFile";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => _fileNameProvider.GetFileSearchPattern(fileName));
            Assert.Equal("File name must have an extension.", exception.Message);
        }

        [Fact]
        public void GetChunkFileName_ShouldReturnCorrectChunkFileName()
        {
            // Arrange
            var fileName = "TestFile.txt";
            var chunkIndex = 1;
            var expectedChunkFileName = "test_file_chunk_1.txt";

            // Act
            var chunkFileName = _fileNameProvider.GetChunkFileName(fileName, chunkIndex);

            // Assert
            Assert.Equal(expectedChunkFileName, chunkFileName);
        }

        [Fact]
        public void GetUniqueFileName_ShouldGenerateUniqueIdentifiers()
        {
            // Arrange
            var fileName = "TestFile.txt";
            
            // Act
            var uniqueFileName1 = _fileNameProvider.GetUniqueFileName(fileName);
            var uniqueFileName2 = _fileNameProvider.GetUniqueFileName(fileName);
            // Assert
            Assert.NotEqual(uniqueFileName1, uniqueFileName2);
        }
    }
}