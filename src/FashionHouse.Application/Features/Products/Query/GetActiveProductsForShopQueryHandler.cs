using Cortex.Mediator.Queries;
using FashionHouse.Application.Contracts;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Features.Products.Query
{
    public class GetActiveProductsForShopQueryHandler : IQueryHandler<GetActiveProductsForShopQuery, (IList<Product>, int, int)>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;

        public GetActiveProductsForShopQueryHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<(IList<Product>, int, int)> Handle(GetActiveProductsForShopQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.ProductRepository.GetActiveForShopAsync(query, cancellationToken);
        }
    }
}