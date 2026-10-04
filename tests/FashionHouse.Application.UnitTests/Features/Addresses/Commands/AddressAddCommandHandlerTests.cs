using FashionHouse.Application.Features.Addresses.Command;
using FashionHouse.Domain.Entities;
using Moq;
using Shouldly;

namespace FashionHouse.Application.UnitTests.Features.Addresses.Commands
{
    public class AddressAddCommandHandlerTests : HandlerTestBase<AddressAddCommandHandler>
    {
        private static AddressAddCommand CreateCommand(bool isDefault) => new()
        {
            CustomerId = Guid.NewGuid(),
            FullName = "Jane Doe",
            Phone = "01700000000",
            FullAddress = "House 1, Road 2, Dhanmondi",
            District = "Dhaka",
            PostalCode = "1209",
            IsDefault = isDefault
        };

        [Test]
        public async Task Handle_DefaultAddress_ClearsExistingDefaultBeforeAdding()
        {
            // Arrange
            var command = CreateCommand(isDefault: true);
            var calls = new List<string>();

            _addressRepository
                .Setup(x => x.ClearDefaultForCustomerAsync(command.CustomerId, _token))
                .Callback(() => calls.Add("clear"))
                .Returns(Task.CompletedTask);
            _addressRepository
                .Setup(x => x.AddAsync(It.IsAny<Address>(), _token))
                .Callback(() => calls.Add("add"))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, _token);

            // Assert
            result.ShouldSatisfyAllConditions(
                () => result.Id.ShouldNotBe(Guid.Empty),
                () => result.CustomerId.ShouldBe(command.CustomerId),
                () => result.FullName.ShouldBe(command.FullName),
                () => result.Phone.ShouldBe(command.Phone),
                () => result.FullAddress.ShouldBe(command.FullAddress),
                () => result.District.ShouldBe(command.District),
                () => result.PostalCode.ShouldBe(command.PostalCode),
                () => result.IsDefault.ShouldBeTrue());

            calls.ShouldBe(new[] { "clear", "add" });
            _addressRepository.Verify(x => x.AddAsync(result, _token), Times.Once());
            _unitOfWork.Verify(x => x.SaveAsync(_token), Times.Once());
        }

        [Test]
        public async Task Handle_NonDefaultAddress_DoesNotClearExistingDefault()
        {
            // Arrange
            var command = CreateCommand(isDefault: false);

            // Act
            var result = await _handler.Handle(command, _token);

            // Assert
            result.IsDefault.ShouldBeFalse();

            _addressRepository.Verify(
                x => x.ClearDefaultForCustomerAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never());
            _addressRepository.Verify(x => x.AddAsync(result, _token), Times.Once());
            _unitOfWork.Verify(x => x.SaveAsync(_token), Times.Once());
        }

        [Test]
        public async Task Handle_PostalCodeNotProvided_AddsAddressWithNullPostalCode()
        {
            // Arrange
            var command = CreateCommand(isDefault: false);
            command.PostalCode = null;

            // Act
            var result = await _handler.Handle(command, _token);

            // Assert
            result.PostalCode.ShouldBeNull();
        }
    }
}
