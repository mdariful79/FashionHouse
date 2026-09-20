using Cortex.Mediator.Commands;
using FashionHouse.Application.Contracts;
using FashionHouse.Domain.Entities;
using FashionHouse.Domain.Utilities;

namespace FashionHouse.Application.Features.Addresses.Command
{
    public class AddressAddCommandHandler : ICommandHandler<AddressAddCommand, Address>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;

        public AddressAddCommandHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Address> Handle(AddressAddCommand command, CancellationToken cancellationToken)
        {
            if (command.IsDefault)
                await _unitOfWork.AddressRepository.ClearDefaultForCustomerAsync(command.CustomerId, cancellationToken);

            var address = new Address
            {
                Id = IdentityGenerator.NewSequentialGuid(),
                CustomerId = command.CustomerId,
                FullName = command.FullName,
                Phone = command.Phone,
                FullAddress = command.FullAddress,
                District = command.District,
                PostalCode = command.PostalCode,
                IsDefault = command.IsDefault
            };

            await _unitOfWork.AddressRepository.AddAsync(address, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);

            return address;
        }
    }
}