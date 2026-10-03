using Cortex.Mediator.Queries;
using FashionHouse.Application.Contracts;

namespace FashionHouse.Application.Features.Products.Query
{
    public class GetHomeProductsQueryHandler : IQueryHandler<GetHomeProductsQuery, HomeProductsDto>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;

        public GetHomeProductsQueryHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public Task<HomeProductsDto> Handle(GetHomeProductsQuery query, CancellationToken cancellationToken)
    => _unitOfWork.ProductRepository.GetHomeProductsAsync(query.Take, cancellationToken);

    }
}