FROM mcr.microsoft.com/dotnet/sdk:5.0 as build

ARG BUILDCONFIG=RELEASE
ARG VERSION=1.0.0
ARG EF_VERSION=5.0.0

COPY dotnet5BackendProject.csproj /build/

RUN dotnet restore ./build/dotnet5BackendProject.csproj

COPY . ./build/
WORKDIR /build/
RUN dotnet publish ./dotnet5BackendProjectContextPostgre.csproj -c $BUILDCONFIG -o out /p:Version=$VERSION

# ef-install
# ef is not put into PATH
# you can run it like: /root/.dotnet/tools/dotnet-ef database update
# but only after db startet
RUN dotnet tool install --global dotnet-ef --version $EF_VERSION

WORKDIR /app
RUN mv /build/out .
RUN rm -r /build
WORKDIR /app/out

ENTRYPOINT ["dotnet", "dotnet5BackendProjectContextPostgre.dll", "--urls", "https://0.0.0.0:5000"]
