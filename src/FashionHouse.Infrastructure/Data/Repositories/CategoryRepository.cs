using FashionHouse.Application.Contracts.Repositories;
using FashionHouse.Application.Features.Categories.Query;
using FashionHouse.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FashionHouse.Infrastructure.Data.Repositories
{
    public class CategoryRepository : Repository<Category, Guid>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<(IList<Category>, int, int)> GetPagedCategories(GetAllCategoriesByPagingQuery query,
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

        public async Task<bool> IsDuplicateCategoryName(string name, Guid? id, CancellationToken cancellationToken)
        {
            if (!id.HasValue)
                return (await GetCountAsync(x => x.Name == name, cancellationToken)) > 0;
            else
                return (await GetCountAsync(x => x.Name == name && x.Id != id.Value, cancellationToken)) > 0;
        }

        public async Task<IList<Category>> GetActiveAsync(CancellationToken cancellationToken)
        {
            var (items, _, _) = await GetDynamicAsync(
                x => x.IsActive,
                "Name",
                null,
                1,
                int.MaxValue,
                false,
                cancellationToken);

            return items;
        }
    }
}