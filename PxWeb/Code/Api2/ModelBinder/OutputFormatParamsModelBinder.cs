using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.ModelBinding;

using PxWeb.Api2.Server.Models;
using PxWeb.Converters;

namespace PxWeb.Code.Api2.ModelBinder
{
    public class OutputFormatParamsModelBinder : IModelBinder
    {

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {

            ArgumentNullException.ThrowIfNull(bindingContext);

            var modelName = bindingContext.ModelName;

            var result = new List<OutputFormatParamType>();

            var keys = bindingContext.HttpContext.Request.Query.Keys.Where(x => x.Equals(modelName, StringComparison.InvariantCultureIgnoreCase));

            foreach (var key in keys)
            {
                string? q = bindingContext.HttpContext.Request.Query[key];
                if (q != null)
                {
                    var items = Regex.Split(q, ",(?=[^\\]]*(?:\\[|$))", RegexOptions.None,
                            TimeSpan.FromMilliseconds(100));


                    foreach (var item in items)
                    {
                        try
                        {
                            result.Add(EnumConverter.ToEnum<OutputFormatParamType>(CleanValue(item)));
                        }
                        catch (InvalidOperationException)
                        {
                            bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Invalid value for enum.");
                            bindingContext.Result = ModelBindingResult.Failed();
                            return Task.CompletedTask;
                        }
                    }
                }
            }


            bindingContext.Result = ModelBindingResult.Success(result);

            return Task.CompletedTask;
        }

        private static string CleanValue(string value)
        {
            var item2 = value.Trim();
            if (item2.StartsWith('[') && item2.EndsWith(']'))
            {
                return value.Substring(1, item2.Length - 2).Trim();
            }
            return item2;

        }
    }
}
