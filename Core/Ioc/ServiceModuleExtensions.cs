﻿using Infrastructure.Field.Repository;
using Infrastructure.Form.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Ioc
{
    public static class ServiceModuleExtensions
    {
        public static void RegisterInfrastructureServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IFormRepository, FormRepository>(); 
            serviceCollection.AddTransient<IFieldRepository, FieldRepository>();
        }
    }
}
