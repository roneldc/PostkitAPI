using Postkit.Shared.Responses;

namespace Postkit.Tests.Helpers
{
    public class PagedResponseTests
    {
        [Fact]
        public void Constructor_Initializes_Data_AsEmptyList()
        {
            // Arrange & Act
            var response = new PagedResponse<int>();

            // Assert
            Assert.NotNull(response.Data);
            Assert.Empty(response.Data);
        }

        [Fact]
        public void Constructor_Initializes_Pagination_WithDefaults()
        {
            // Arrange & Act
            var response = new PagedResponse<string>();

            // Assert
            Assert.NotNull(response.Pagination);
            Assert.Equal(0, response.Pagination.CurrentPage);
            Assert.Equal(0, response.Pagination.PageSize);
            Assert.Equal(0, response.Pagination.TotalItems);
            Assert.Equal(0, response.Pagination.TotalPages);
        }

        [Fact]
        public void CanAssign_Data_And_Pagination()
        {
            // Arrange
            var data = new List<string> { "a", "b" };
            var pagination = new PaginationMetadata
            {
                CurrentPage = 1,
                PageSize = 10,
                TotalItems = 20,
                TotalPages = 2
            };

            var response = new PagedResponse<string>
            {
                Data = data,
                Pagination = pagination
            };

            // Act & Assert
            Assert.Equal(data, response.Data);
            Assert.Equal(pagination, response.Pagination);
        }
    }
}