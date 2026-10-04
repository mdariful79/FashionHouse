using FashionHouse.Application.Features.Inventories.Query;
using FashionHouse.Domain.Entities;
using Moq;
using Shouldly;

namespace FashionHouse.Application.UnitTests.Features.Inventories.Queries
{
    public class GetInventoryByIdQueryHandlerTests : HandlerTestBase<GetInventoryByIdQueryHandler>
    {
        [Test]
        public async Task Handle_EntityExists_ReturnsEntity()
        {
            // Arrange
            var query = new GetInventoryByIdQuery { Id = Guid.NewGuid() };
            var expected = new Inventory { Id = Guid.NewGuid() };

            _inventoryRepository.Setup(x => x.GetByIdAsync(query.Id, _token)).ReturnsAsync(expected);

            // Act
            var result = await _handler.Handle(query, _token);

            // Assert
            result.ShouldBeSameAs(expected);
            _inventoryRepository.Verify(x => x.GetByIdAsync(query.Id, _token), Times.Once());
        }

        [Test]
        public async Task Handle_EntityDoesNotExist_ReturnsNull()
        {
            // Arrange
            var query = new GetInventoryByIdQuery { Id = Guid.NewGuid() };

            _inventoryRepository.Setup(x => x.GetByIdAsync(query.Id, _token)).ReturnsAsync((Inventory?)null);

            // Act
            var result = await _handler.Handle(query, _token);

            // Assert
            result.ShouldBeNull();
        }
    }
}
