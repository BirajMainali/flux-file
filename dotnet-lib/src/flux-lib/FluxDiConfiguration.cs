using FluxFile.Providers;
using FluxFile.Providers.Interfaces;
using FluxFile.Services;
using FluxFile.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FluxFile;

public static class FluxDiConfiguration
{
    public static IServiceCollection AddFluxFile(this IServiceCollection services, string? uploadDirectory = null)
    {
        uploadDirectory ??= "uploads";
        services.AddScoped<IFluxFileUploadService, FluxFileUploadService>();
        services.AddSingleton<IFluxFileStorageProvider>(new FluxFileLocalFileStorageProvider(uploadDirectory));
        services.AddScoped<IFluxFileNameProvider, FluxFileNameProvider>();
        return services;
    }
}