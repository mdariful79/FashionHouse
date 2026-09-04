using Autofac;
using Autofac.Extensions.DependencyInjection;
using Cortex.Mediator.DependencyInjection;
using FashionHouse.Infrastructure.Data;
using FashionHouse.Infrastructure.Extensions;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.File("Logs/web-log-.log", rollingInterval: RollingInterval.Day)
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    var migrationAssembly = typeof(ApplicationDbContext).Assembly;


    #region Dependency Injection

    builder.Services.AddInfrastructureDependency();

    #endregion

    #region Serilog Configuration

    builder.Host.UseSerilog((context, lc) => lc
        .MinimumLevel.Debug()
        .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .ReadFrom.Configuration(context.Configuration)
    );

    #endregion

    #region Autofac Configuration
    //builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
    //builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
    //{
    //    containerBuilder.RegisterModule(new WebModule(connectionString));
    //});

    #endregion

    #region Cortex Mediator Configuration

    //builder.Services.AddCortexMediator(
    //    new[] { typeof(Program), typeof(ProductAddCommand) },
    //    options => options.AddDefaultBehaviors()
    //);

    #endregion

    #region Mapster Configuration

    // Custom Configuration
    //var config = TypeAdapterConfig.GlobalSettings;
    //config.Scan(typeof(MapsterConfiguration).Assembly);
    //builder.Services.AddSingleton(config);
    //builder.Services.AddScoped<IMapper, ServiceMapper>();

    // Default Configuration
    builder.Services.AddMapster();

    #endregion

    #region DbContext Configuration
    builder.Services.AddDbContext(connectionString, migrationAssembly);
    #endregion

    builder.Services.AddDatabaseDeveloperPageExceptionFilter();

    #region Identity Configuration
    builder.Services.AddIdentity();
    #endregion

    builder.Services.AddControllersWithViews();
    builder.Services.AddRazorPages();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseMigrationsEndPoint();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapStaticAssets();

    app.MapControllerRoute(
    name: "Customer",
    pattern: "Customer/{controller=Customer}/{action=Index}/{id?}",
    defaults: new { area = "Customer" });

    app.MapControllerRoute(
       name: "area",
       pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}")
       .WithStaticAssets();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
        .WithStaticAssets();

    app.MapRazorPages()
       .WithStaticAssets();

    Log.Information("Starting application");

    app.Run();
}
catch (Exception e)
{
    Log.Fatal(e, "Application crashed");
}
finally
{
    Log.CloseAndFlush();
}