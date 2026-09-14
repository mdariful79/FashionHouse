using Cortex.Mediator.Commands;
using FashionHouse.Application.Contracts;
using FashionHouse.Application.Contracts.Services;

namespace FashionHouse.Application.Features.ProductImages.Command
{
    public class DeleteProductImageCommandHandler : ICommandHandler<DeleteProductImageCommand>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorageService;

        public DeleteProductImageCommandHandler(IApplicationUnitOfWork unitOfWork, IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _fileStorageService = fileStorageService;
        }

        public async Task Handle(DeleteProductImageCommand command, CancellationToken cancellationToken)
        {
            var image = _unitOfWork.ProductImageRepository.GetById(command.Id);
            if (image == null) return;

            await _fileStorageService.DeleteImageAsync(image.ImageUrl, "products", cancellationToken);

            await _unitOfWork.ProductImageRepository.RemoveAsync(command.Id, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
        }
    }
}