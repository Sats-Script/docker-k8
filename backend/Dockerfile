# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY DevOpsSample.Api.csproj ./
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
HEALTHCHECK --interval=30s --timeout=3s --retries=3 \
  CMD curl --fail http://localhost:8080/health || exit 1
ENTRYPOINT ["dotnet", "DevOpsSample.Api.dll"]

# Build: docker build -t my-backend-image .
# Run: docker run -d -p 8080:8080 my-backend-image
# The application will be accessible on port 80.
# Ensure to configure your application settings and environment variables as needed for your deployment environment.
# Consider using environment variables or a secrets management tool to handle sensitive information securely.
# For production deployments, ensure to implement proper logging, monitoring, and security practices.
# You can build and run this Dockerfile using the following commands:
# docker build -t my-backend-image .
# docker run -d -p 8080:80 my-backend-image
# Replace 'my-backend-image' with your desired image name.
# The application will be accessible at http://localhost:8080 when running the container.# Adjust the port mapping as necessary for your environment.
# Make sure to test the containerized application
# in a staging environment before deploying to production.
# to ensure all dependencies and configurations are correctly set up.
# Review and update the Dockerfile as needed to fit your specific application requirements and deployment scenarios.
# This Dockerfile sets up a multi-stage build for a .NET 8.0 backend application.
# It first builds the application in a build stage and then publishes it in a separate stage.
# Finally, it creates a runtime image that only contains the necessary files to run the application.
# This approach helps to keep the final image size smaller and more secure by excluding build tools and unnecessary files.
# Always refer to the official .NET Docker documentation for best practices and updates.





