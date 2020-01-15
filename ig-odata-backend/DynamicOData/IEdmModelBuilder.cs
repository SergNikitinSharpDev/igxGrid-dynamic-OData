using Microsoft.OData.Edm;

namespace PostgreODataAPI.DynamicOData
{
    public interface IEdmModelBuilder
    {
        EdmModel GetModel();
    }
}
