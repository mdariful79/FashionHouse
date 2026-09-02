using FashionHouse.Domain.Entites;
using Mapster;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace FashionHouse.Web
{
    public class MapsterConfiguration : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            //config.NewConfig<ProductAddCommand, Product>();
            //config.NewConfig<ProductModel, ProductAddCommand>();
        }
    }
}
