using System;
using System.Collections.Generic;
using System.Linq;
using ARConsistency.Abstractions;
using ARConsistency.Configuration;
using ARConsistency.ContractResolver;
using ARConsistency.Helpers;
using ARConsistency.ResponseModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace ARConsistency
{
    public static class ServiceCollectionExtension
    {
        public static IMvcBuilder AddApiResponseConsistency(this IMvcBuilder builder, 
            Action<ArcConfiguration> configurationBuilder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            
            if (configurationBuilder == null)
                throw new ArgumentNullException(nameof(configurationBuilder));

            IServiceCollection services = builder.Services;
            
            Configure(services, configurationBuilder);
            SuppressBadRequestResponse(services);
            SetApiResponseTypeNameHandler(builder);

            return builder;
        }

        public static IApplicationBuilder UseApiResponseConsistency(this IApplicationBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.UseMiddleware<ConsistencyMiddleware>();
        }

        #region Private Helper Methods
        private static void Configure(IServiceCollection services,  Action<ArcConfiguration> configurationBuilder)
        {
            var configuration = new ArcConfiguration();
            configurationBuilder.Invoke(configuration);
            services.AddSingleton(configuration.ResponseOptions);
            services.AddSingleton(configuration.ExceptionStatusCodeHandler);
        }
        
        private static void SuppressBadRequestResponse(IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    List<ValidationError> validationErrors = context.ModelState.Keys
                        .SelectMany(key => context.ModelState[key].Errors.Select(x => new ValidationError(key, x.ErrorMessage)))
                        .ToList();

                    ApiError response = new ApiError(ResponseMessage.ValidationError, validationErrors);
                    return new BadRequestObjectResult(response);
                };
            });
        }

        private static void SetApiResponseTypeNameHandler(IMvcBuilder builder)
        {
            builder.AddNewtonsoftJson(opt =>
            {
                opt.SerializerSettings.TypeNameHandling = TypeNameHandling.Objects;
                opt.SerializerSettings.ContractResolver = SuppressItemTypeNameContractResolver.Instance;
            });
        }
        #endregion
    }
}
