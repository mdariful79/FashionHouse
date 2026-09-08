using Cortex.Mediator.Queries;
using FashionHouse.Application.Contracts;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Features.Categories.Query
{
    public class GetAllCategoriesByPagingQueryHandler : IQueryHandler<GetAllCategoriesByPagingQuery, (IList<Category>, int, int)>
    {
        private readonly IApplicationUnitOfWork _applicationUnitOfWork;

        public GetAllCategoriesByPagingQueryHandler(IApplicationUnitOfWork applicationUnitOfWork)
        {
            _applicationUnitOfWork = applicationUnitOfWork;
        }

        public async Task<(IList<Category>, int, int)> Handle(GetAllCategoriesByPagingQuery query,
            CancellationToken cancellationToken)
        {
            return await _applicationUnitOfWork.CategoryRepository.GetPagedCategories(query, cancellationToken);
        }
    }
}