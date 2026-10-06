FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
RUN apt update && apt install -y nodejs
COPY ["src/FashionHouse.Web/FashionHouse.Web.csproj","FashionHouse.Web/"]
COPY ["src/FashionHouse.Domain/FashionHouse.Domain.csproj","FashionHouse.Domain/"]
COPY ["src/FashionHouse.Application/FashionHouse.Application.csproj","FashionHouse.Application/"]
COPY ["src/FashionHouse.Infrastructure/FashionHouse.Infrastructure.csproj","FashionHouse.Infrastructure/"]
RUN dotnet restore "FashionHouse.Web/FashionHouse.Web.csproj"

COPY ./src/ .
WORKDIR "/src/FashionHouse.Web"
RUN dotnet build "FashionHouse.Web.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "FashionHouse.Web.csproj" -c Release -o /app

FROM build AS final
WORKDIR /app
COPY --from=publish /app .
EXPOSE 80
ENTRYPOINT ["dotnet", "FashionHouse.Web.dll"]




