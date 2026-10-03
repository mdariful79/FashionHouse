using Cortex.Mediator.Queries;
using FashionHouse.Application.Contracts;

namespace FashionHouse.Application.Features.Products.Query
{
    public class GetShopFilterOptionsQueryHandler : IQueryHandler<GetShopFilterOptionsQuery, ShopFilterOptionsDto>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;

        public GetShopFilterOptionsQueryHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<ShopFilterOptionsDto> Handle(GetShopFilterOptionsQuery query, CancellationToken cancellationToken)
            => _unitOfWork.ProductRepository.GetShopFilterOptionsAsync(query.CategoryId, cancellationToken);
    }
}