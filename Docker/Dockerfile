FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /src
COPY ./src /src
RUN dotnet build -c Release -o /build

FROM mcr.microsoft.com/dotnet/runtime:6.0-alpine as base
RUN apk --no-cache add curl bash
RUN curl https://raw.githubusercontent.com/helm/helm/main/scripts/get-helm-3 | bash

FROM base as runtime
WORKDIR /app
COPY --from=build /build .
ENV CHART_PATH=/var/www/helm-repo/charts
ENTRYPOINT ["dotnet", "helm-repo.dll"]