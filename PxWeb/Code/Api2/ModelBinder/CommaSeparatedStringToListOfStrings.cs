using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PxWeb.Code.Api2.ModelBinder
{
    public class CommaSeparatedStringToListOfStrings : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {

            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            var modelName = bindingContext.ModelName;

            var result = new List<string>();

            var keys = bindingContext.HttpContext.Request.Query.Keys.ToList().Where(x => x.Equals(modelName, StringComparison.InvariantCultureIgnoreCase)).ToList();

            foreach (var key in keys)
            {
                if (key != null)
                {
                    string? q = bindingContext.HttpContext.Request.Query[key];
                    if (q != null)
                    {
                        var items = Regex.Split(q, ",(?=[^\\]]*(?:\\[|$))");

                        foreach (var item in items)
                        {
                            var item2 = item.Trim();
                            if (item.StartsWith("[") && item.EndsWith("]"))
                            {
                                result.Add(item.Substring(1, item.Length - 2));
                            }
                            else
                            {
                                result.Add(item2);
                            }
                        }
                    }
                }
            }


            bindingContext.Result = ModelBindingResult.Success(result);

            return Task.CompletedTask;
        }
    }
}
