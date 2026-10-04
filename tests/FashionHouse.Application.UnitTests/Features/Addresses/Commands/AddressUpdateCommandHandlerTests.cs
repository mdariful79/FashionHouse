using FashionHouse.Application.Features.Addresses.Command;
using FashionHouse.Domain.Entities;
using Moq;
using Shouldly;

namespace FashionHouse.Application.UnitTests.Features.Addresses.Commands
{
    public class AddressUpdateCommandHandlerTests : HandlerTestBase<AddressUpdateCommandHandler>
    {
        private static AddressUpdateCommand CreateCommand(Guid customerId, bool isDefault) => new()
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            FullName = "Jane Updated",
            Phone = "01811111111",
            FullAddress = "House 9, Road 7, Gulshan",
            District = "Dhaka North",
            PostalCode = "1212",
            IsDefault = isDefault
        };

        private static Address CreateAddress(AddressUpdateCommand command, Guid customerId, bool isDefault) => new()
        {
            Id = command.Id,
            CustomerId = customerId,
            FullName = "Old Name",
            Phone = "000",
            FullAddress = "Old address",
            District = "Old district",
            PostalCode = "0000",
            IsDefault = isDefault
        };

        [Test]
        public async Task Handle_AddressBelongsToCustomer_UpdatesAllFields()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var command = CreateCommand(customerId, isDefault: false);
            var address = CreateAddress(command, customerId, isDefault: false);

            _addressRepository.Setup(x => x.GetByIdAsync(command.Id, _token)).ReturnsAsync(address);

            // Act
            var result = await _handler.Handle(command, _token);

            // Assert
            result.ShouldSatisfyAllConditions(
                () => result.ShouldBeSameAs(address),
                () => result.FullName.ShouldBe(command.FullName),
                () => result.Phone.ShouldBe(command.Phone),
                () => result.FullAddress.ShouldBe(command.FullAddress),
                () => result.District.ShouldBe(command.District),
                () => result.PostalCode.ShouldBe(command.PostalCode),
                () => result.IsDefault.ShouldBe(command.IsDefault));

            _addressRepository.Verify(x => x.EditAsync(address, _token), Times.Once());
            _unitOfWork.Verify(x => x.SaveAsync(_token), Times.Once());
        }

        // (address is currently default, command asks for default, expected "clear other defaults" calls)
        [TestCase(false, true, 1)]
        [TestCase(true, true, 0)]
        [TestCase(false, false, 0)]
        [TestCase(true, false, 0)]
        public async Task Handle_DefaultFlag_ClearsOtherDefaultsOnlyWhenBecomingDefault(
            bool currentlyDefault, bool commandIsDefault, int expectedClearCalls)
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var command = CreateCommand(customerId, commandIsDefault);
            var address = CreateAddress(command, customerId, currentlyDefault);

            _addressRepository.Setup(x => x.GetByIdAsync(command.Id, _token)).ReturnsAsync(address);

            // Act
            await _handler.Handle(command, _token);

            // Assert
            _addressRepository.Verify(
                x => x.ClearDefaultForCustomerAsync(customerId, _token), Times.Exactly(expectedClearCalls));
        }

        [Test]
        public async Task Handle_AddressNotFound_ThrowsAndDoesNotSave()
        {
            // Arrange
            var command = CreateCommand(Guid.NewGuid(), isDefault: false);

            _addressRepository.Setup(x => x.GetByIdAsync(command.Id, _token)).ReturnsAsync((Address?)null);

            // Act
            var ex = await Should.ThrowAsync<Exception>(async () => await _handler.Handle(command, _token));

            // Assert
            ex.Message.ShouldBe("Address not found.");
            _addressRepository.Verify(x => x.EditAsync(It.IsAny<Address>(), It.IsAny<CancellationToken>()), Times.Never());
            _unitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Test]
        public async Task Handle_AddressBelongsToAnotherCustomer_ThrowsAndDoesNotModifyAddress()
        {
            // Arrange
            var command = CreateCommand(Guid.NewGuid(), isDefault: true);
            var address = CreateAddress(command, customerId: Guid.NewGuid(), isDefault: false);

            _addressRepository.Setup(x => x.GetByIdAsync(command.Id, _token)).ReturnsAsync(address);

            // Act
            var ex = await Should.ThrowAsync<Exception>(async () => await _handler.Handle(command, _token));

            // Assert
            ex.Message.ShouldBe("Address not found.");
            address.FullName.ShouldBe("Old Name");
            _addressRepository.Verify(
                x => x.ClearDefaultForCustomerAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never());
            _unitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never());
        }
    }
}
