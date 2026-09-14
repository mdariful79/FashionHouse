using Cortex.Mediator.Commands;
using FashionHouse.Application.Contracts;

namespace FashionHouse.Application.Features.ProductImages.Command
{
    public class SetPrimaryProductImageCommandHandler : ICommandHandler<SetPrimaryProductImageCommand>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;

        public SetPrimaryProductImageCommandHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(SetPrimaryProductImageCommand command, CancellationToken cancellationToken)
        {
            await _unitOfWork.ProductImageRepository.ClearPrimaryFlagAsync(command.ProductId, cancellationToken);

            var image = _unitOfWork.ProductImageRepository.GetById(command.Id);
            if (image != null)
                image.IsPrimary = true;

            await _unitOfWork.SaveAsync(cancellationToken);
        }
    }
}