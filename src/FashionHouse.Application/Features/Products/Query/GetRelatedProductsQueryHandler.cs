using Cortex.Mediator.Queries;
using FashionHouse.Application.Contracts;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Features.Products.Query
{
    public class GetRelatedProductsQueryHandler : IQueryHandler<GetRelatedProductsQuery, IList<Product>>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;

        public GetRelatedProductsQueryHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IList<Product>> Handle(GetRelatedProductsQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.ProductRepository.GetRelatedAsync(query.CategoryId, query.ExcludeProductId, query.Take, cancellationToken);
        }
    }
}