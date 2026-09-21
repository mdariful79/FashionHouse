using Cortex.Mediator.Queries;
using FashionHouse.Application.Contracts;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Features.Products.Query
{
    public class GetProductDetailsForShopQueryHandler : IQueryHandler<GetProductDetailsForShopQuery, Product?>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;

        public GetProductDetailsForShopQueryHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Product?> Handle(GetProductDetailsForShopQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.ProductRepository.GetByIdWithDetailsAsync(query.Id, cancellationToken);
        }
    }
}