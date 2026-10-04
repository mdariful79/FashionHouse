using FashionHouse.Application.Features.Customers.Command;
using FashionHouse.Domain.Entities;
using Moq;
using Shouldly;

namespace FashionHouse.Application.UnitTests.Features.Customers.Commands
{
    public class CustomerAddCommandHandlerTests : HandlerTestBase<CustomerAddCommandHandler>
    {
        [Test]
        public async Task Handle_ValidCommand_AddsCustomerWithCommandValues()
        {
            // Arrange
            var command = new CustomerAddCommand
            {
                Id = Guid.NewGuid(),
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jane.doe@example.com",
                Phone = "01700000000"
            };

            // Act
            var result = await _handler.Handle(command, _token);

            // Assert
            result.ShouldSatisfyAllConditions(
                () => result.Id.ShouldBe(command.Id),
                () => result.FirstName.ShouldBe(command.FirstName),
                () => result.LastName.ShouldBe(command.LastName),
                () => result.Email.ShouldBe(command.Email),
                () => result.Phone.ShouldBe(command.Phone));

            _customerRepository.Verify(x => x.AddAsync(result, _token), Times.Once());
            _unitOfWork.Verify(x => x.SaveAsync(_token), Times.Once());
        }

        [Test]
        public async Task Handle_PhoneNotProvided_AddsCustomerWithNullPhone()
        {
            // Arrange
            var command = new CustomerAddCommand
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Smith",
                Email = "john.smith@example.com",
                Phone = null
            };

            // Act
            var result = await _handler.Handle(command, _token);

            // Assert
            result.Phone.ShouldBeNull();
            _customerRepository.Verify(x => x.AddAsync(It.IsAny<Customer>(), _token), Times.Once());
        }

        [Test]
        public async Task Handle_AddFails_DoesNotSave()
        {
            // Arrange
            var command = new CustomerAddCommand { Id = Guid.NewGuid(), Email = "x@example.com" };

            _customerRepository
                .Setup(x => x.AddAsync(It.IsAny<Customer>(), _token))
                .ThrowsAsync(new InvalidOperationException("add failed"));

            // Act & Assert
            await Should.ThrowAsync<InvalidOperationException>(
                async () => await _handler.Handle(command, _token));

            _unitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never());
        }
    }
}
