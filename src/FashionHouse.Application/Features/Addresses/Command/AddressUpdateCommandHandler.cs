using Cortex.Mediator.Commands;
using FashionHouse.Application.Contracts;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Features.Addresses.Command
{
    public class AddressUpdateCommandHandler : ICommandHandler<AddressUpdateCommand, Address>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;

        public AddressUpdateCommandHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Address> Handle(AddressUpdateCommand command, CancellationToken cancellationToken)
        {
            var address = await _unitOfWork.AddressRepository.GetByIdAsync(command.Id, cancellationToken);

            if (address is null || address.CustomerId != command.CustomerId)
                throw new Exception("Address not found.");

            if (command.IsDefault && !address.IsDefault)
                await _unitOfWork.AddressRepository.ClearDefaultForCustomerAsync(command.CustomerId, cancellationToken);

            address.FullName = command.FullName;
            address.Phone = command.Phone;
            address.FullAddress = command.FullAddress;
            address.District = command.District;
            address.PostalCode = command.PostalCode;
            address.IsDefault = command.IsDefault;

            await _unitOfWork.AddressRepository.EditAsync(address, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);

            return address;
        }
    }
}