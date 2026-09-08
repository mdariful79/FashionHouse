using Cortex.Mediator.Queries;
using FashionHouse.Application.Contracts;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Features.Categories.Query
{
    public class GetCategoryByIdQueryHandler : IQueryHandler<GetCategoryByIdQuery, Category?>
    {
        private readonly IApplicationUnitOfWork _applicationUnitOfWork;

        public GetCategoryByIdQueryHandler(IApplicationUnitOfWork applicationUnitOfWork)
        {
            _applicationUnitOfWork = applicationUnitOfWork;
        }

        public async Task<Category?> Handle(GetCategoryByIdQuery query, CancellationToken cancellationToken)
        {
            return await _applicationUnitOfWork.CategoryRepository.GetByIdAsync(query.Id, cancellationToken);
        }
    }
}