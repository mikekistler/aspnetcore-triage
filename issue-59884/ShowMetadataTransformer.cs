using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

// This transformer adds an `x-metadata` extension to the OpenAPI document containing all the metadata
// associated with the endpoint that is accessible to the transformer.
public class ShowMetadataTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        // This is where all the endpoint metadata should be available
        var metadata = context.Description.ActionDescriptor.EndpointMetadata;
        if (metadata != null)
        {
            OpenApiObject metadataObj =  new OpenApiObject();
            foreach (var data in metadata)
            {
                metadataObj.Add(data.GetType().ToString(), new OpenApiString(data.ToString()));
            }
            operation.Extensions.Add("x-metadata", new OpenApiObject(metadataObj));
        }
        return Task.CompletedTask;
    }
}