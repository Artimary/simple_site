FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

RUN mkdir -p /src/src && mkdir -p /src/test
COPY src/simple_site.csproj /src/src/
COPY test/simple_site.test.csproj /src/test/

WORKDIR /src/src
RUN dotnet restore

WORKDIR /src/test
RUN dotnet restore

WORKDIR /src
COPY . .

WORKDIR /src/src
RUN dotnet build -c Release -o /app/build

WORKDIR /src/test
RUN dotnet publish -c Release -o /app/test-publish

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS test
WORKDIR /app
COPY --from=build /app/test-publish .
ENTRYPOINT ["dotnet", "test", "simple_site.test.dll"]