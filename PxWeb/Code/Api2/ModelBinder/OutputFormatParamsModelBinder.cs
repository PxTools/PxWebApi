using PxWeb.Api2.Server.Models;
using PxWeb.Converters;

namespace PxWeb.Code.Api2.ModelBinder
{
    public class OutputFormatParamsModelBinder : GenericListModelBinder<OutputFormatParamType>
    {
        public OutputFormatParamsModelBinder() : base(x => EnumConverter.ToEnum<OutputFormatParamType>(x))
        {
        }
    }
}
