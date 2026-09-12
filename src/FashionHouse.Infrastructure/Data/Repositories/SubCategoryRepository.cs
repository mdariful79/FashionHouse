using FashionHouse.Application.Contracts.Repositories;
using FashionHouse.Application.Features.SubCategories.Query;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Infrastructure.Data.Repositories
{
    public class SubCategoryRepository : Repository<SubCategory, Guid>, ISubCategoryRepository
    {
        public SubCategoryRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<(IList<SubCategory>, int, int)> GetPagedSubCategories(GetAllSubCategoriesByPagingQuery query,
            CancellationToken cancellationToken)
        {
            return await GetDynamicAsync(
                x => query.SearchText == null || x.Name.Contains(query.SearchText),
                query.SortText,
                null,
                query.PageIndex,
                query.PageSize,
                true,
                cancellationToken);
        }

        public async Task<bool> IsDuplicateSubCategoryName(string name, Guid categoryId, Guid? id, CancellationToken cancellationToken)
        {
            if (!id.HasValue)
                return (await GetCountAsync(x => x.Name == name && x.CategoryId == categoryId, cancellationToken)) > 0;
            else
                return (await GetCountAsync(x => x.Name == name && x.CategoryId == categoryId && x.Id != id.Value, cancellationToken)) > 0;
        }
    }
}