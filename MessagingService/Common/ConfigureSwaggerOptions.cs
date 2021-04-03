namespace MessagingService.Common
{
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc.ApiExplorer;
    using Microsoft.Extensions.Options;
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;

    /// <summary>
    /// Configures the Swagger generation options.
    /// </summary>
    /// <remarks>This allows API versioning to define a Swagger document per API version after the
    /// <see cref="IApiVersionDescriptionProvider"/> service has been resolved from the service container.</remarks>
    [ExcludeFromCodeCoverage]
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        #region Methods

        /// <inheritdoc />
        public void Configure(SwaggerGenOptions options)
        {
        }

        /// <summary>
        /// Creates the information for API version.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <returns></returns>
        private static OpenApiInfo CreateInfoForApiVersion()
        {
            OpenApiInfo info = new OpenApiInfo
                               {
                                   Title = "Messaging API",
                                   Version = "1.0",
                                   Description = "A REST Api to manage sending of various messages over different formats, currently only Email and SMS are supported.",
                                   Contact = new OpenApiContact
                                             {
                                                 Name = "Stuart Ferguson",
                                                 Email = "golfhandicapping@btinternet.com"
                                             }
                               };

            return info;
        }

        #endregion
    }

    /// <summary>
    /// Represents the Swagger/Swashbuckle operation filter used to document the implicit API version parameter.
    /// </summary>
    /// <remarks>This <see cref="IOperationFilter"/> is only required due to bugs in the <see cref="SwaggerGenerator"/>.
    /// Once they are fixed and published, this class can be removed.</remarks>
    [ExcludeFromCodeCoverage]
    public class SwaggerDefaultValues : IOperationFilter
    {
        #region Methods

        /// <summary>
        /// Applies the filter to the specified operation using the given context.
        /// </summary>
        /// <param name="operation">The operation to apply the filter to.</param>
        /// <param name="context">The current operation filter context.</param>
        public void Apply(OpenApiOperation operation,
                          OperationFilterContext context)
        {
            ApiDescription apiDescription = context.ApiDescription;

            if (operation.Parameters == null)
            {
                return;
            }

            foreach (OpenApiParameter parameter in operation.Parameters)
            {
                ApiParameterDescription description = apiDescription.ParameterDescriptions.First(p => p.Name == parameter.Name);

                if (parameter.Description == null)
                {
                    parameter.Description = description.ModelMetadata?.Description;
                }

                parameter.Required |= description.IsRequired;
            }
        }

        #endregion
    }
}