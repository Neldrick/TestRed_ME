
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace WSModules{
    public static class WSExtension{
         public static IApplicationBuilder MapWebSocketManager(this IApplicationBuilder app, 
                                                              PathString path,
                                                              WSHandler handler)
        {
            return app.Map(path, (_app) => _app.UseMiddleware<WSMiddleware>(handler));
        }

         public static IServiceCollection AddWebSocketManager(this IServiceCollection services)
        {
            services.AddTransient<WSConnectionManager>();

            foreach(var type in Assembly.GetEntryAssembly().ExportedTypes)
            {
                if(type.GetTypeInfo().BaseType == typeof(WSHandler))
                {
                    services.AddSingleton(type);
                }
            }

            return services;
        }
    }
}