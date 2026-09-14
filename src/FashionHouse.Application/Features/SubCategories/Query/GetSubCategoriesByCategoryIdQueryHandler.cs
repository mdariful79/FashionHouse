using Cortex.Mediator.Queries;
using FashionHouse.Application.Contracts;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Features.SubCategories.Query
{
    public class GetSubCategoriesByCategoryIdQueryHandler : IQueryHandler<GetSubCategoriesByCategoryIdQuery, IList<SubCategory>>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;

        public GetSubCategoriesByCategoryIdQueryHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IList<SubCategory>> Handle(GetSubCategoriesByCategoryIdQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.SubCategoryRepository.GetByCategoryIdAsync(query.CategoryId, cancellationToken);
        }
    }
}