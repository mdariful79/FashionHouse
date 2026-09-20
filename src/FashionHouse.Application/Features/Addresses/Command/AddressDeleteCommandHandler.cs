using Cortex.Mediator.Commands;
using FashionHouse.Application.Contracts;

namespace FashionHouse.Application.Features.Addresses.Command
{
    public class AddressDeleteCommandHandler : ICommandHandler<AddressDeleteCommand, bool>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;

        public AddressDeleteCommandHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(AddressDeleteCommand command, CancellationToken cancellationToken)
        {
            var address = await _unitOfWork.AddressRepository.GetByIdAsync(command.Id, cancellationToken);

            if (address is null || address.CustomerId != command.CustomerId)
                throw new Exception("Address not found.");

            await _unitOfWork.AddressRepository.RemoveAsync(address, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);

            return true;
        }
    }
}