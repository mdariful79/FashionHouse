using FashionHouse.Application.Features.Inventories.Query;
using FashionHouse.Domain.Entities;
using Moq;
using Shouldly;

namespace FashionHouse.Application.UnitTests.Features.Inventories.Queries
{
    public class GetInventoryByProductIdQueryHandlerTests : HandlerTestBase<GetInventoryByProductIdQueryHandler>
    {
        [Test]
        public async Task Handle_EntityExists_ReturnsEntity()
        {
            // Arrange
            var query = new GetInventoryByProductIdQuery { ProductId = Guid.NewGuid() };
            var expected = new Inventory { Id = Guid.NewGuid() };

            _inventoryRepository.Setup(x => x.GetByProductIdAsync(query.ProductId, _token)).ReturnsAsync(expected);

            // Act
            var result = await _handler.Handle(query, _token);

            // Assert
            result.ShouldBeSameAs(expected);
            _inventoryRepository.Verify(x => x.GetByProductIdAsync(query.ProductId, _token), Times.Once());
        }

        [Test]
        public async Task Handle_EntityDoesNotExist_ReturnsNull()
        {
            // Arrange
            var query = new GetInventoryByProductIdQuery { ProductId = Guid.NewGuid() };

            _inventoryRepository.Setup(x => x.GetByProductIdAsync(query.ProductId, _token)).ReturnsAsync((Inventory?)null);

            // Act
            var result = await _handler.Handle(query, _token);

            // Assert
            result.ShouldBeNull();
        }
    }
}
