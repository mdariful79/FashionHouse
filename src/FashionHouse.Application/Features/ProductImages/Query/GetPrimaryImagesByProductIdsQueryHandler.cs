using Cortex.Mediator.Queries;
using FashionHouse.Application.Contracts;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Features.ProductImages.Query
{
    public class GetPrimaryImagesByProductIdsQueryHandler : IQueryHandler<GetPrimaryImagesByProductIdsQuery, IDictionary<Guid, ProductImage>>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;

        public GetPrimaryImagesByProductIdsQueryHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IDictionary<Guid, ProductImage>> Handle(GetPrimaryImagesByProductIdsQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.ProductImageRepository.GetPrimaryImagesByProductIdsAsync(query.ProductIds, cancellationToken);
        }
    }
}