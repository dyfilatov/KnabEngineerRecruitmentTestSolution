FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["KnabEngineerRecruitmentTestTests/KnabEngineerRecruitmentTestTests.csproj", "KnabEngineerRecruitmentTestTests/"]
COPY ["KnabEngineerRecruitmentTestInfrastructure/KnabEngineerRecruitmentTestInfrastructure.csproj", "KnabEngineerRecruitmentTestInfrastructure/"]
RUN dotnet restore "KnabEngineerRecruitmentTestTests/KnabEngineerRecruitmentTestTests.csproj"
COPY . .
WORKDIR "/src/KnabEngineerRecruitmentTestTests"
ENTRYPOINT ["dotnet", "test"]
