using Cortex.Mediator.Queries;
using FashionHouse.Application.Contracts;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Features.SubCategories.Query
{
    public class GetAllSubCategoriesByPagingQueryHandler : IQueryHandler<GetAllSubCategoriesByPagingQuery, (IList<SubCategory>, int, int)>
    {
        private readonly IApplicationUnitOfWork _applicationUnitOfWork;

        public GetAllSubCategoriesByPagingQueryHandler(IApplicationUnitOfWork applicationUnitOfWork)
        {
            _applicationUnitOfWork = applicationUnitOfWork;
        }

        public async Task<(IList<SubCategory>, int, int)> Handle(GetAllSubCategoriesByPagingQuery query,
            CancellationToken cancellationToken)
        {
            return await _applicationUnitOfWork.SubCategoryRepository.GetPagedSubCategories(query, cancellationToken);
        }
    }
}