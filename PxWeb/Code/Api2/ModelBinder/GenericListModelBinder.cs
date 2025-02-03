using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PxWeb.Code.Api2.ModelBinder
{
    public class GenericListModelBinder<T> : IModelBinder
    {
        public GenericListModelBinder(Func<string, T> func)
        {
            Converter = func;
        }

        protected Func<string, T> Converter { get; set; }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {

            ArgumentNullException.ThrowIfNull(bindingContext);

            var modelName = bindingContext.ModelName;

            var result = new List<T>();

            var keys = bindingContext.HttpContext.Request.Query.Keys.Where(x => x.Equals(modelName, StringComparison.InvariantCultureIgnoreCase));

            foreach (var key in keys)
            {
                string? q = bindingContext.HttpContext.Request.Query[key];
                if (q != null)
                {
                    try
                    {
                        var list = CommaSeparatedListToListConverter.ToList<T>(q, x => Converter(x));
                        result.AddRange(list);
                    }
                    catch (InvalidOperationException)
                    {
                        bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Invalid value.");
                        bindingContext.Result = ModelBindingResult.Failed();
                        return Task.CompletedTask;
                    }
                }
            }

            bindingContext.Result = ModelBindingResult.Success(result);

            return Task.CompletedTask;
        }
    }
}

