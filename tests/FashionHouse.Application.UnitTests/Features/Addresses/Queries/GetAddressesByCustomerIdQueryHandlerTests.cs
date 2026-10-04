using FashionHouse.Application.Features.Addresses.Query;
using FashionHouse.Domain.Entities;
using Moq;
using Shouldly;

namespace FashionHouse.Application.UnitTests.Features.Addresses.Queries
{
    public class GetAddressesByCustomerIdQueryHandlerTests : HandlerTestBase<GetAddressesByCustomerIdQueryHandler>
    {
        [Test]
        public async Task Handle_ValidQuery_ReturnsItemsFromRepository()
        {
            // Arrange
            var query = new GetAddressesByCustomerIdQuery { CustomerId = Guid.NewGuid() };
            IList<Address> expected = new List<Address>
            {
                new Address { Id = Guid.NewGuid() },
                new Address { Id = Guid.NewGuid() }
            };

            _addressRepository.Setup(x => x.GetByCustomerIdAsync(query.CustomerId, _token)).ReturnsAsync(expected);

            // Act
            var result = await _handler.Handle(query, _token);

            // Assert
            result.ShouldBeSameAs(expected);
            _addressRepository.Verify(x => x.GetByCustomerIdAsync(query.CustomerId, _token), Times.Once());
        }

        [Test]
        public async Task Handle_RepositoryReturnsEmptyList_ReturnsEmptyList()
        {
            // Arrange
            var query = new GetAddressesByCustomerIdQuery { CustomerId = Guid.NewGuid() };
            IList<Address> expected = new List<Address>();

            _addressRepository.Setup(x => x.GetByCustomerIdAsync(query.CustomerId, _token)).ReturnsAsync(expected);

            // Act
            var result = await _handler.Handle(query, _token);

            // Assert
            result.ShouldBeEmpty();
        }
    }
}
