using FluxFile.Exceptions;

namespace FluxFileTests
{
    public class FluxFileUploadExceptionTests
    {
        [Fact]
        public void Constructor_ShouldSetMessage()
        {
            // Arrange
            var expectedMessage = "This is a test exception message.";

            // Act
            var exception = new FluxFileUploadException(expectedMessage);

            // Assert
            Assert.Equal(expectedMessage, exception.Message);
        }

        [Fact]
        public void Constructor_ShouldSetMessage_WhenMessageIsEmpty()
        {
            // Arrange
            var expectedMessage = "";

            // Act
            var exception = new FluxFileUploadException(expectedMessage);

            // Assert
            Assert.Equal(expectedMessage, exception.Message);
        }
    }
}