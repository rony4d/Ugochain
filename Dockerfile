FROM microsoft/dotnet:2.1-sdk AS build-env
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY UgoChain/*.csproj ./UgoChain/
COPY UgoChain.Api/*.csproj ./UgoChain.Api/
COPY UgoChain.Api.Client/*.csproj ./UgoChain.Api.Client/
COPY UgoChain.Api.PeerOneSever/*.csproj ./UgoChain.Api.PeerOneServer/
COPY UgoChain.Api.PeerOneClient/*.csproj ./UgoChain.Api.PeerOneClient/
COPY Ugochain.PeerOne.Features/*.csproj ./UgoChain.PeerOne.Features/
COPY UgoChain.Api.PeerTwoServer/*.csproj ./UgoChain.Api.PeerTwoServer/
COPY UgoChain.Api.PeerTwoClient/*.csproj ./UgoChain.Api.PeerTwoClient/
COPY UgoChain.PeerTwo.Features/*.csproj ./UgoChain.PeerTwo.Features/
COPY UgoChain.Tests/*.csproj ./UgoChain.Tests/
RUN dotnet restore

# copy everything else and build
COPY UgoChain/. ./UgoChain/
COPY UgoChain.Api/. ./UgoChain.Api/
COPY UgoChain.Api.Client/. ./UgoChain.Api.Client/
COPY UgoChain.Tests/. ./UgoChain.Tests/
COPY UgoChain.Api.PeerOneSever/. ./UgoChain.Api.PeerOneServer/
COPY UgoChain.Api.PeerOneClient/. ./UgoChain.Api.PeerOneClient/
COPY Ugochain.PeerOne.Features/. ./UgoChain.PeerOne.Features/
COPY UgoChain.Api.PeerTwoServer/. ./UgoChain.Api.PeerTwoServer/
COPY UgoChain.Api.PeerTwoClient/. ./UgoChain.Api.PeerTwoClient/
COPY UgoChain.PeerTwo.Features/. ./UgoChain.PeerTwo.Features/

RUN dotnet build

FROM build AS ugochain
WORKDIR /app/UgoChain
RUN dotnet ugochain

FROM build AS ugochainapi
WORKDIR /app/UgoChain.Api
RUN dotnet ugochainapi

FROM ugochainapi AS publish
WORKDIR /app/UgoChain.Api
RUN dotnet publish -o out
#RUN dotnet publish -c Release -o out

# build runtime image
FROM microsoft/aspnetcore:2.1
WORKDIR /app
COPY --from=build-env /app/UgoChain.Api/out ./
ENTRYPOINT ["dotnet", "UgoChain.Api.dll"]
