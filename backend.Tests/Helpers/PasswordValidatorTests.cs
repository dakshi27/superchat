using Xunit;
using FluentAssertions;
using backend.Helpers;

namespace backend.Tests.Helpers
{
    public class PasswordValidatorTests
    {
        [Theory]
        [InlineData("StrongPass1!", true)]         // meets all criteria
        [InlineData("weakpass", false)]            // no uppercase, digit, special char
        [InlineData("SHORT1!", false)]             // too short
        [InlineData("NoSpecialChar1", false)]      // missing special character
        [InlineData("NoDigit!", false)]            // missing digit
        [InlineData("12345678!", false)]           // no letters
        [InlineData("ALLUPPERCASE1!", false)]      // no lowercase
        [InlineData("alllowercase1!", false)]      // no uppercase
        [InlineData("", false)]                    // empty string
        [InlineData("     ", false)]               // whitespace only
        [InlineData(null, false)]                  // null input
        public void IsStrongPassword_ShouldReturnExpectedResult(string input, bool expected)
        {
            // Act
            var result = PasswordValidator.IsStrongPassword(input);

            // Assert
            result.Should().Be(expected);
        }
    }
}
