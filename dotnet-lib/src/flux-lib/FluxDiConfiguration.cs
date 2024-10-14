using FluxFile.Providers;
using FluxFile.Providers.Interfaces;
using FluxFile.Services;
using FluxFile.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FluxFile;

/// <summary>
/// Provides dependency injection configuration for the FluxFile library.
/// This static class contains methods to register necessary services and providers for file upload functionality.
/// </summary>
public static class FluxDiConfiguration
{
    /// <summary>
    /// Registers the necessary services and providers for the FluxFile library into the provided service collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the services will be added.</param>
    /// <param name="uploadDirectory">An optional directory path where uploaded files will be stored. If not provided, defaults to "uploads".</param>
    /// <returns>The updated <see cref="IServiceCollection"/> with the added services.</returns>
    public static IServiceCollection AddFluxFile(this IServiceCollection services, string? uploadDirectory = null)
    {
        uploadDirectory ??= "uploads"; // Default to "uploads" if no directory is provided.
        services.AddScoped<IFluxFileUploadService, FluxFileUploadService>();
        services.AddSingleton<IFluxFileStorageProvider>(new FluxFileLocalFileStorageProvider(uploadDirectory));
        services.AddScoped<IFluxFileNameProvider, FluxFileNameProvider>();
        return services;
    }
}