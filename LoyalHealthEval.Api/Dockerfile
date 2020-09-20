FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["LoyalHealthEval.Api/LoyalHealthEval.Api.csproj", "LoyalHealthEval.Api/"]
COPY ["ReviewGenerator.Lib/ReviewGenerator.Lib.csproj", "ReviewGenerator.Lib/"]
RUN dotnet restore "LoyalHealthEval.Api/LoyalHealthEval.Api.csproj"
COPY . .
WORKDIR "/src/LoyalHealthEval.Api"
RUN dotnet build "LoyalHealthEval.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LoyalHealthEval.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LoyalHealthEval.Api.dll"]