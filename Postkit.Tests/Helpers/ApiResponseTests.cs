using Postkit.Shared.Responses;

namespace Postkit.Tests.Helpers
{
    public class ApiResponseTests
    {
        [Fact]
        public void Constructor_SetsProperties_Correctly()
        {
            // Arrange
            var data = "test data";
            var success = true;
            var status = 201;
            var message = "Created";

            // Act
            var response = new ApiResponse<string>(success, status, message, data);

            // Assert
            Assert.Equal(success, response.Success);
            Assert.Equal(status, response.Status);
            Assert.Equal(message, response.Message);
            Assert.Equal(data, response.Data);
            Assert.True((DateTime.UtcNow - response.Timestamp).TotalSeconds < 5);
        }

        [Fact]
        public void SuccessResponse_CreatesSuccessResponse()
        {
            // Arrange
            var data = 123;
            var message = "OK";
            var statusCode = 200;

            // Act
            var response = ApiResponse<int>.SuccessResponse(data, message, statusCode);

            // Assert
            Assert.True(response.Success);
            Assert.Equal(statusCode, response.Status);
            Assert.Equal(message, response.Message);
            Assert.Equal(data, response.Data);
            Assert.True((DateTime.UtcNow - response.Timestamp).TotalSeconds < 5);
        }

        [Fact]
        public void ErrorResponse_CreatesErrorResponse()
        {
            // Arrange
            var message = "Error occurred";
            var statusCode = 404;

            // Act
            var response = ApiResponse<string>.ErrorResponse(message, statusCode);

            // Assert
            Assert.False(response.Success);
            Assert.Equal(statusCode, response.Status);
            Assert.Equal(message, response.Message);
            Assert.Null(response.Data);
            Assert.True((DateTime.UtcNow - response.Timestamp).TotalSeconds < 5);
        }

        [Fact]
        public void Message_DefaultsToEmptyString()
        {
            // Act
            var response = new ApiResponse<string>(true, 200);

            // Assert
            Assert.Equal(string.Empty, response.Message);
        }
    }
}