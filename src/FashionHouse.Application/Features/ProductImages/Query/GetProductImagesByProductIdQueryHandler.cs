using Cortex.Mediator.Queries;
using FashionHouse.Application.Contracts;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Features.ProductImages.Query
{
    public class GetProductImagesByProductIdQueryHandler : IQueryHandler<GetProductImagesByProductIdQuery, IList<ProductImage>>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;

        public GetProductImagesByProductIdQueryHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IList<ProductImage>> Handle(GetProductImagesByProductIdQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.ProductImageRepository.GetByProductIdAsync(query.ProductId, cancellationToken);
        }
    }
}