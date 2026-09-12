using Cortex.Mediator.Queries;
using FashionHouse.Application.Contracts;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Features.SubCategories.Query
{
    public class GetSubCategoryByIdQueryHandler : IQueryHandler<GetSubCategoryByIdQuery, SubCategory?>
    {
        private readonly IApplicationUnitOfWork _applicationUnitOfWork;

        public GetSubCategoryByIdQueryHandler(IApplicationUnitOfWork applicationUnitOfWork)
        {
            _applicationUnitOfWork = applicationUnitOfWork;
        }

        public async Task<SubCategory?> Handle(GetSubCategoryByIdQuery query, CancellationToken cancellationToken)
        {
            return await _applicationUnitOfWork.SubCategoryRepository.GetByIdAsync(query.Id, cancellationToken);
        }
    }
}