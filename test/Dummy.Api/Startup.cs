namespace Dummy.Api
{
    using System.Collections.Generic;
    using System.Reflection;
    using Be.Vlaanderen.Basisregisters.AspNetCore.Mvc.Formatters.Json;
    using Be.Vlaanderen.Basisregisters.AspNetCore.Swagger;
    using Be.Vlaanderen.Basisregisters.AspNetCore.Swagger.ReDoc;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ApiExplorer;
    using Microsoft.AspNetCore.Mvc.ApplicationModels;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Swashbuckle.AspNetCore.Swagger;
    using SwaggerOptions = Be.Vlaanderen.Basisregisters.AspNetCore.Swagger.SwaggerOptions;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient<IApiControllerSpecification, ApiControllerSpec>());

            services
                .AddMvcCore()

                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)

                .AddJsonFormatters()
                .AddJsonOptions(cfg => cfg.SerializerSettings.ConfigureDefaultForApi())

                .AddApiExplorer()

                .Services

                .AddVersionedApiExplorer(cfg =>
                {
                    cfg.GroupNameFormat = "'v'VVV";
                    cfg.SubstituteApiVersionInUrl = true;
                })

                .AddApiVersioning(cfg =>
                {
                    cfg.ReportApiVersions = true;
                })

                // These settings are required for Swagger
                .AddSwagger<Startup>(new SwaggerOptions
                {
                    ApiInfoFunc = (provider, description) => new Info
                    {
                        Version = description.ApiVersion.ToString(),
                        Title = "Example API",
                        Description = GetApiLeadingText(description),
                        Contact = new Contact
                        {
                            Name = "agentschap Informatie Vlaanderen",
                            Email = "informatie.vlaanderen@vlaanderen.be",
                            Url = "https://vlaanderen.be/informatie-vlaanderen"
                        }
                    },

                    XmlCommentPaths = new [] { typeof(Startup).GetTypeInfo().Assembly.GetName().Name },

                    AdditionalHeaderOperationFilters = new List<HeaderOperationFilter>
                    {
                        new HeaderOperationFilter("apiKey", "Optionele API key voor het verzoek.")
                    }
                });
        }

        public void Configure(
            IApplicationBuilder app,
            IApiVersionDescriptionProvider apiVersionProvider)
        {
            app
                // These settings are required for ReDoc
                .UseSwaggerDocumentation(new SwaggerDocumentationOptions
                {
                    ApiVersionDescriptionProvider = apiVersionProvider,
                    DocumentTitleFunc = groupName => $"Example API {groupName}"
                })

                .UseMvc();
        }

        private static string GetApiLeadingText(ApiVersionDescription description)
            => $"Right now you are reading the documentation for version {description.ApiVersion} of the Example API{string.Format(description.IsDeprecated ? ", **this API version is not supported any more**." : ".")}";
    }
}
