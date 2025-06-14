using NUnit.Framework;
using SafeVault.Helpers;

namespace SafeVault.Tests
{
    [TestFixture]
    public class ValidationHelperTests
    {
        [Test]
        public void IsValidInput_WithValidInput_ReturnsTrue()
        {
            // Arrange
            string validInput = "Password123!";
            string allowedChars = "!@#$";

            // Act
            bool result = ValidationHelper.IsValidInput(validInput, allowedChars);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsValidInput_WithInvalidChars_ReturnsFalse()
        {
            // Arrange
            string invalidInput = "Password123!";
            string allowedChars = "@#$";

            // Act
            bool result = ValidationHelper.IsValidInput(invalidInput, allowedChars);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsValidInput_WithWhitespace_ReturnsFalse()
        {
            // Arrange
            string inputWithWhitespace = "Hello 123";
            string allowedChars = "!@#$";

            // Act
            bool result = ValidationHelper.IsValidInput(inputWithWhitespace, allowedChars);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsValidXSSInput_WithScriptTag_ReturnsFalse()
        {
            // Arrange
            string input = "<script>alert('XSS')</script>";

            // Act
            bool result = ValidationHelper.IsValidXSSInput(input);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsValidXSSInput_WithEventAttribute_ReturnsFalse()
        {
            // Arrange
            string input = "<div onclick='alert(1)'>Click me</div>";

            // Act
            bool result = ValidationHelper.IsValidXSSInput(input);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsValidXSSInput_WithDangerousTags_ReturnsFalse()
        {
            // Arrange
            string[] dangerousInputs = {
                "<iframe src='malicious'>",
                "<object>malicious</object>",
                "<embed src='malicious'>",
                "<link href='malicious'>",
                "<style>malicious</style>",
                "<meta content='malicious'>"
            };

            // Act & Assert
            foreach (var input in dangerousInputs)
            {
                Assert.IsFalse(ValidationHelper.IsValidXSSInput(input),
                    $"Input '{input}' should be detected as XSS");
            }
        }

        [Test]
        public void IsValidXSSInput_WithSafeInput_ReturnsTrue()
        {
            // Arrange
            string safeInput = "<p>This is safe text</p>";

            // Act
            bool result = ValidationHelper.IsValidXSSInput(safeInput);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void ContainsXss_WithVariousXssPatterns_ReturnsTrue()
        {
            // Arrange
            string[] xssInputs = {
                "<script>alert('XSS')</script>",
                "<div onclick='alert(1)'>",
                "<iframe src='javascript:alert(1)'>",
                "javascript:alert(1)",
                "eval('alert(1)')",
                "document.cookie",
                "document.write",
                "innerHTML = 'alert(1)'",
                "src='javascript:alert(1)'",
                "expression('alert(1)')",
                "vbscript:alert(1)"
            };

            // Act & Assert
            foreach (var input in xssInputs)
            {
                Assert.IsTrue(ValidationHelper.ContainsXss(input),
                    $"Input '{input}' should be detected as XSS");
            }
        }

        [Test]
        public void ContainsXss_WithSafeInput_ReturnsFalse()
        {
            // Arrange
            string safeInput = "<p>This is safe text</p>";

            // Act
            bool result = ValidationHelper.ContainsXss(safeInput);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
