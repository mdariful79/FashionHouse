using FashionHouse.Application.Features.Addresses.Command;
using FashionHouse.Domain.Entities;
using Moq;
using Shouldly;

namespace FashionHouse.Application.UnitTests.Features.Addresses.Commands
{
    public class AddressDeleteCommandHandlerTests : HandlerTestBase<AddressDeleteCommandHandler>
    {
        [Test]
        public async Task Handle_AddressBelongsToCustomer_RemovesAddressAndReturnsTrue()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var command = new AddressDeleteCommand { Id = Guid.NewGuid(), CustomerId = customerId };
            var address = new Address { Id = command.Id, CustomerId = customerId };

            _addressRepository.Setup(x => x.GetByIdAsync(command.Id, _token)).ReturnsAsync(address);

            // Act
            var result = await _handler.Handle(command, _token);

            // Assert
            result.ShouldBeTrue();
            _addressRepository.Verify(x => x.RemoveAsync(address, _token), Times.Once());
            _unitOfWork.Verify(x => x.SaveAsync(_token), Times.Once());
        }

        [Test]
        public async Task Handle_AddressNotFound_ThrowsAndDoesNotSave()
        {
            // Arrange
            var command = new AddressDeleteCommand { Id = Guid.NewGuid(), CustomerId = Guid.NewGuid() };

            _addressRepository.Setup(x => x.GetByIdAsync(command.Id, _token)).ReturnsAsync((Address?)null);

            // Act
            var ex = await Should.ThrowAsync<Exception>(async () => await _handler.Handle(command, _token));

            // Assert
            ex.Message.ShouldBe("Address not found.");
            _addressRepository.Verify(x => x.RemoveAsync(It.IsAny<Address>(), It.IsAny<CancellationToken>()), Times.Never());
            _unitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Test]
        public async Task Handle_AddressBelongsToAnotherCustomer_ThrowsAndDoesNotRemove()
        {
            // Arrange
            var command = new AddressDeleteCommand { Id = Guid.NewGuid(), CustomerId = Guid.NewGuid() };
            var address = new Address { Id = command.Id, CustomerId = Guid.NewGuid() };

            _addressRepository.Setup(x => x.GetByIdAsync(command.Id, _token)).ReturnsAsync(address);

            // Act
            var ex = await Should.ThrowAsync<Exception>(async () => await _handler.Handle(command, _token));

            // Assert
            ex.Message.ShouldBe("Address not found.");
            _addressRepository.Verify(x => x.RemoveAsync(It.IsAny<Address>(), It.IsAny<CancellationToken>()), Times.Never());
            _unitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never());
        }
    }
}
