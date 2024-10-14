using FluxFile.Extensions;

namespace FluxFileTests
{
    public class StringExtensionTests
    {
        [Theory]
        [InlineData("CamelCase", "camel_case")]
        [InlineData("snake_case", "snake_case")]
        [InlineData("Space Case", "space_case")]
        [InlineData("MixedCase String", "mixed_case_string")]
        [InlineData("Already_Snake_Case", "already_snake_case")]
        [InlineData("UpperCase", "upper_case")]
        [InlineData("This Is A Test", "this_is_a_test")]
        [InlineData("  Leading And Trailing Spaces  ", "leading_and_trailing_spaces")]
        [InlineData("Multiple    Spaces", "multiple_spaces")]
        [InlineData("A", "a")]
        [InlineData("", "")]
        public void ToSnakeCase_ShouldReturnExpectedResult(string input, string expected)
        {
            var result = input.ToSnakeCase();
            Assert.Equal(expected, result);
        }
    }
}