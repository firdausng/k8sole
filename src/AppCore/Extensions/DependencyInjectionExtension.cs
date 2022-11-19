using AppCore.Services.K8s;
using AppCore.Services.K8s.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AppCore.Extensions;
public static class DependencyInjectionExtension
{
    public static IServiceCollection AddAppCore(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton<K8sService>();
        services.AddSingleton<K8sContextService>();
        services.AddSingleton<CurrentK8SContext>();
        services.AddSingleton<NamespaceService>();
        services.AddSingleton<ServicesService>();
        services.AddSingleton<PodService>();
        services.AddSingleton<DeploymentService>();

        return services;
    }
}