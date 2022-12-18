FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["KnabEngineerRecruitmentTestWebApi/KnabEngineerRecruitmentTestWebApi.csproj", "KnabEngineerRecruitmentTestWebApi/"]
COPY ["KnabEngineerRecruitmentTestInfrastructure/KnabEngineerRecruitmentTestInfrastructure.csproj", "KnabEngineerRecruitmentTestInfrastructure/"]
RUN dotnet restore "KnabEngineerRecruitmentTestWebApi/KnabEngineerRecruitmentTestWebApi.csproj"
COPY . .
WORKDIR "/src/KnabEngineerRecruitmentTestWebApi"
RUN dotnet build "KnabEngineerRecruitmentTestWebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KnabEngineerRecruitmentTestWebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KnabEngineerRecruitmentTestWebApi.dll"]
