using Autofac;

namespace FashionHouse.Web
{
    public class WebModule : Module
    {
        private readonly string _connectionString;

        public WebModule(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
        }
    }
}

