using Cortex.Mediator.Queries;
using FashionHouse.Application.Contracts;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Features.Categories.Query
{
    public class GetActiveCategoriesQueryHandler : IQueryHandler<GetActiveCategoriesQuery, IList<Category>>
    {
        private readonly IApplicationUnitOfWork _applicationUnitOfWork;

        public GetActiveCategoriesQueryHandler(IApplicationUnitOfWork applicationUnitOfWork)
        {
            _applicationUnitOfWork = applicationUnitOfWork;
        }

        public async Task<IList<Category>> Handle(GetActiveCategoriesQuery query, CancellationToken cancellationToken)
        {
            return await _applicationUnitOfWork.CategoryRepository.GetActiveAsync(cancellationToken);
        }
    }
}