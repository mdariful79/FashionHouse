using FashionHouse.Application.Features.Addresses.Query;
using FashionHouse.Domain.Entities;
using Moq;
using Shouldly;

namespace FashionHouse.Application.UnitTests.Features.Addresses.Queries
{
    public class GetAddressByIdQueryHandlerTests : HandlerTestBase<GetAddressByIdQueryHandler>
    {
        [Test]
        public async Task Handle_EntityExists_ReturnsEntity()
        {
            // Arrange
            var query = new GetAddressByIdQuery { Id = Guid.NewGuid() };
            var expected = new Address { Id = Guid.NewGuid() };

            _addressRepository.Setup(x => x.GetByIdAsync(query.Id, _token)).ReturnsAsync(expected);

            // Act
            var result = await _handler.Handle(query, _token);

            // Assert
            result.ShouldBeSameAs(expected);
            _addressRepository.Verify(x => x.GetByIdAsync(query.Id, _token), Times.Once());
        }

        [Test]
        public async Task Handle_EntityDoesNotExist_ReturnsNull()
        {
            // Arrange
            var query = new GetAddressByIdQuery { Id = Guid.NewGuid() };

            _addressRepository.Setup(x => x.GetByIdAsync(query.Id, _token)).ReturnsAsync((Address?)null);

            // Act
            var result = await _handler.Handle(query, _token);

            // Assert
            result.ShouldBeNull();
        }
    }
}
