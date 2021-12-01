using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Description;

namespace TRMDataManager.App_Start
{
    public class AuthTokenOperation : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer)
        {
            // Add a new route in swagger for Auth Token
            swaggerDoc.paths.Add("/token", new PathItem
            {
                // Define post command for the path
                post = new Operation
                {
                    // Place route into the Auth category
                    tags = new List<string> { "Auth" },
                    // Define the type of data that will be sent through the parameters
                    consumes = new List<string>
                    {
                        "application/x-www-form-urlencoded"
                    }
                },
                // Define the parameters
                parameters = new List<Parameter>
                {
                    new Parameter
                    {
                        type = "string",
                        name = "grant_type",
                        required = true,
                        @in = "formData",
                        @default = "password"
                    },
                    new Parameter
                    {
                        type = "string",
                        name = "username",
                        required = false,
                        @in = "formData"
                    },
                    new Parameter
                    {
                        type = "string",
                        name = "password",
                        required = false,
                        @in = "formData"
                    }
                }
            });
        }
    }
}