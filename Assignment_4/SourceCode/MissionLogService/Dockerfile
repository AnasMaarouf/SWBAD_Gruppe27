FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY MissionLogService.csproj .
RUN dotnet restore


COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV JWT__SigningKey: "THIS_IS_A_VERY_LONG_SECRET_KEY_AT_LEAST_32_CHARS_LONG_12345"

ENTRYPOINT ["dotnet", "MissionLogService.dll"]
