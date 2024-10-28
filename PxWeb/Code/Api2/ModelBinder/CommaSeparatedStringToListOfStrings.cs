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

            var keys = bindingContext.HttpContext.Request.Query.Keys.Where(x => x.Equals(modelName, StringComparison.InvariantCultureIgnoreCase));

            foreach (var key in keys)
            {
                string? q = bindingContext.HttpContext.Request.Query[key];
                if (q != null)
                {
                    var items = Regex.Split(q, ",(?=[^\\]]*(?:\\[|$))", RegexOptions.IgnoreCase,
                                TimeSpan.FromMilliseconds(100));

                    foreach (var item in items)
                    {
                        result.Add(CleanValue(item));
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
                return value.Substring(1, item2.Length - 2);
            }
            return item2;

        }
    }
}
