using Cortex.Mediator.Queries;
using FashionHouse.Application.Contracts;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Features.Products.Query
{
    public class GetProductByIdQueryHandler : IQueryHandler<GetProductByIdQuery, Product?>
    {
        private readonly IApplicationUnitOfWork _applicationUnitOfWork;

        public GetProductByIdQueryHandler(IApplicationUnitOfWork applicationUnitOfWork)
        {
            _applicationUnitOfWork = applicationUnitOfWork;
        }

        public async Task<Product?> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
        {
            return await _applicationUnitOfWork.ProductRepository.GetByIdAsync(query.Id, cancellationToken);
        }
    }
}