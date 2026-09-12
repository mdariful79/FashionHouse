using FashionHouse.Domain.Contracts;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Contracts.Repositories
{
    public interface ISubCategoryRepository : IRepository<SubCategory, Guid>
    {
        Task<bool> IsDuplicateSubCategoryName(string name, Guid categoryId, Guid? id, CancellationToken cancellationToken);

        Task<(IList<SubCategory>, int, int)> GetPagedSubCategories(
            Features.SubCategories.Query.GetAllSubCategoriesByPagingQuery query,
            CancellationToken cancellationToken);
    }
}