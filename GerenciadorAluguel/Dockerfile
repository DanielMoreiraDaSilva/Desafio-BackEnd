FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
EXPOSE 8080

WORKDIR /app

COPY *.sln .
COPY . .
RUN dotnet restore

WORKDIR /app/Api
RUN dotnet publish -c Release -o out

WORKDIR /app/Business
RUN dotnet publish -c Release -o out

WORKDIR /app/Core
RUN dotnet publish -c Release -o out

WORKDIR /app/Data
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/Api/out ./
COPY --from=build /app/Business/out ./Business
COPY --from=build /app/Core/out ./Core
COPY --from=build /app/Data/out ./Data

ENTRYPOINT ["dotnet", "Api.dll", "--launch-profile Docker"]