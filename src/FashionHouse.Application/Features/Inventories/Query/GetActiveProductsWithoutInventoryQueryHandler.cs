using Cortex.Mediator.Queries;
using FashionHouse.Application.Contracts;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Features.Products.Query
{
    public class GetActiveProductsWithoutInventoryQueryHandler
        : IQueryHandler<GetActiveProductsWithoutInventoryQuery, IList<Product>>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;

        public GetActiveProductsWithoutInventoryQueryHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IList<Product>> Handle(GetActiveProductsWithoutInventoryQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.ProductRepository.GetActiveWithoutInventoryAsync(cancellationToken);
        }
    }
}