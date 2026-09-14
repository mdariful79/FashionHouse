using Cortex.Mediator.Commands;
using FashionHouse.Application.Contracts;
using FashionHouse.Domain.Entities;
using FashionHouse.Domain.Utilities;

namespace FashionHouse.Application.Features.ProductImages.Command
{
    public class AddProductImageCommandHandler : ICommandHandler<AddProductImageCommand, ProductImage>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;

        public AddProductImageCommandHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ProductImage> Handle(AddProductImageCommand command, CancellationToken cancellationToken)
        {
            if (command.IsPrimary)
                await _unitOfWork.ProductImageRepository.ClearPrimaryFlagAsync(command.ProductId, cancellationToken);

            var image = new ProductImage
            {
                Id = IdentityGenerator.NewSequentialGuid(),
                ProductId = command.ProductId,
                ImageUrl = command.ImageUrl,
                IsPrimary = command.IsPrimary,
                DisplayOrder = command.DisplayOrder
            };

            await _unitOfWork.ProductImageRepository.AddAsync(image, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);

            return image;
        }
    }
}