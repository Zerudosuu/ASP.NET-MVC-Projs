using FluentAssertions;
using LibrarySystemServer.Model;
using LibrarySystemServer.Repositories.Interfaces;
using LibrarySystemServer.Services.Implementations;
using Moq;

namespace LibrarySystemServer.Test.LibrarianServiceUnitTest {
    
public class LibrarianServiceTests {
    private readonly Mock<ILibrarianRepository> _mockRepo;
    private readonly LibrarianService _service;


    public LibrarianServiceTests()
    {
        _mockRepo =  new Mock<ILibrarianRepository>();
        _service = new LibrarianService(_mockRepo.Object);
    }
    
    [Fact]
    public async Task GetAllBooks_ShouldReturnListOfBooks()
    {
        // Arrange
        var fakeBooks = new List<Book> { new() { Id = Guid.NewGuid(), Title = "Test Book" } };
        _mockRepo.Setup(r => r.GetAllBooksAsync()).ReturnsAsync(fakeBooks);

        // Act
        var result = await _service.GetAllBooksAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Title.Should().Be("Test Book");
    }
}
}
