using FashionHouse.Application.Features.Categories.Command;
using FashionHouse.Application.Features.Categories.Query;
using FashionHouse.Domain.Entities;
using FashionHouse.Web.Areas.Admin.Models;
using Mapster;

namespace FashionHouse.Web
{
    public class MapsterConfiguration : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<CategoryModel, CategoryAddCommand>();
            config.NewConfig<CategoryModel, CategoryUpdateCommand>();
            config.NewConfig<Category, CategoryModel>();
            config.NewConfig<CategoryListModel, GetAllCategoriesByPagingQuery>();
        }
    }
}