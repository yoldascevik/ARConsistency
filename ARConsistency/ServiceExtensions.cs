using System;
using System.Linq;
using ARConsistency.ContractResolver;
using ARConsistency.Helpers;
using ARConsistency.ResponseModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ARConsistency
{
    public static class ServiceExtensions
    {
        public static IMvcBuilder AddApiResponseConsistency(this IMvcBuilder builder, Action<ResponseOptions> responseOptionsBuilder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            IServiceCollection services = builder.Services;

            ConfigureResponseOptions(services, responseOptionsBuilder);
            SuppressBadRequestResponse(services);
            SetApiResponseTypeNameHandler(builder);

            return builder;
        }

        public static IApplicationBuilder UseApiResponseConsistency(this IApplicationBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.UseMiddleware<ConsistencyMiddleware>(); ;
        }

        #region Private Helper Methods
        private static void ConfigureResponseOptions(IServiceCollection services,  Action<ResponseOptions> responseOptionsBuilder)
        {
            ResponseOptions options = new ResponseOptions();
            responseOptionsBuilder.Invoke(options);

            services.AddSingleton(options);
        }

        private static void SuppressBadRequestResponse(IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var validationErrors = context.ModelState.Keys
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
