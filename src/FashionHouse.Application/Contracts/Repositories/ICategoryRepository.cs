using FashionHouse.Domain.Contracts;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Contracts.Repositories
{
    public interface ICategoryRepository : IRepository<Category, Guid>
    {
        Task<bool> IsDuplicateCategoryName(string name, Guid? id, CancellationToken cancellationToken);

        Task<(IList<Category>, int, int)> GetPagedCategories(
            Features.Categories.Query.GetAllCategoriesByPagingQuery query,
            CancellationToken cancellationToken);
    }
}