using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PxWeb.Code.Api2.ModelBinder
{
    public class CommaSeparatedStringToListOfStrings : IModelBinder
    {

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {

            ArgumentNullException.ThrowIfNull(bindingContext);

            var modelName = bindingContext.ModelName;

            var result = new List<string>();

            var keys = bindingContext.HttpContext.Request.Query.Keys.Where(x => x.Equals(modelName, StringComparison.InvariantCultureIgnoreCase));

            foreach (var key in keys)
            {
                string? q = bindingContext.HttpContext.Request.Query[key];
                if (q != null)
                {
                    var list = CommaSeparatedListToListConverter.ToList<string>(q, x => x);
                    result.AddRange(list);
                }
            }

            bindingContext.Result = ModelBindingResult.Success(result);

            return Task.CompletedTask;
        }

    }
}
