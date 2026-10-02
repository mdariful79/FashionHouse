using Cortex.Mediator.Commands;
using FashionHouse.Application.Contracts;
using FashionHouse.Domain.Entities;
using FashionHouse.Domain.Utilities;
using MapsterMapper;

namespace FashionHouse.Application.Features.Products.Command
{
    public class ProductAddCommandHandler : ICommandHandler<ProductAddCommand, Product>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductAddCommandHandler(IApplicationUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Product> Handle(ProductAddCommand command, CancellationToken cancellationToken)
        {
            var product = _mapper.Map<Product>(command);
            product.Id = IdentityGenerator.NewSequentialGuid();
            product.CreatedAt = DateTime.UtcNow;

            // Set explicitly so we don't depend on Mapster's collection mapping
            product.Colors = command.Colors.Distinct().OrderBy(c => c).ToList();
            product.Sizes = command.Sizes.Distinct().OrderBy(s => s).ToList();

            await _unitOfWork.ProductRepository.AddAsync(product, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);

            return product;
        }
    }
}