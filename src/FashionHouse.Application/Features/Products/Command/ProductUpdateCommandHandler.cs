using Cortex.Mediator.Commands;
using FashionHouse.Application.Contracts;
using FashionHouse.Application.Exceptions;
using FashionHouse.Domain.Entities;
using MapsterMapper;

namespace FashionHouse.Application.Features.Products.Command
{
    public class ProductUpdateCommandHandler : ICommandHandler<ProductUpdateCommand, Product>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductUpdateCommandHandler(IApplicationUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Product> Handle(ProductUpdateCommand command, CancellationToken cancellationToken)
        {
            var isDuplicateName = await _unitOfWork.ProductRepository.IsDuplicateProductName(
                command.ProductName, command.Id, cancellationToken);

            if (!isDuplicateName)
            {
                var product = _unitOfWork.ProductRepository.GetById(command.Id);
                product = _mapper.Map(command, product);
                product.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.ProductRepository.EditAsync(product, cancellationToken);
                await _unitOfWork.SaveAsync(cancellationToken);

                return product;
            }
            else
                throw new DuplicateDataException("Product name is duplicate");
        }
    }
}