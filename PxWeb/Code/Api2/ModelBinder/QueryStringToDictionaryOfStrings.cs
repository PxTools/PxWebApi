using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PxWeb.Code.Api2.ModelBinder
{
    public class QueryStringToDictionaryOfStrings : IModelBinder
    {


        Task IModelBinder.BindModelAsync(ModelBindingContext bindingContext)
        {

            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            var modelName = bindingContext.ModelName;

            var result = new Dictionary<string, List<string>>();

            var keys = bindingContext.HttpContext.Request.Query.Keys.ToList().Where(x => x.StartsWith(modelName, StringComparison.InvariantCultureIgnoreCase)).ToList();

            foreach (var key in keys)
            {
                //the valuecodes-parameters are not mandatory

                if (key != null)
                {
                    //check that the key starts with [ after the modelname and ends with ]  and is not just valuecodes[]
                    if (!(key.ToLower().StartsWith(modelName + "[") && key.EndsWith("]") && key.Length > modelName.Length + 2))
                    {
                        bindingContext.ModelState.AddModelError(bindingContext.ModelName, "valuecodes-parameter should be like valuecodes[<variable>]. " + key + " is not.");
                        bindingContext.Result = ModelBindingResult.Failed();
                        return Task.CompletedTask;
                    }

                    //strip of the variable name format values[variableName]
                    string variableName = key.Substring(modelName.Length + 1, key.Length - (modelName.Length + 2));


                    string? q = bindingContext.HttpContext.Request.Query[key];
                    if (q != null)
                    {
                        var items = Regex.Split(q, ",(?=[^\\]]*(?:\\[|$))", RegexOptions.None, TimeSpan.FromMilliseconds(100));
                        var itemsList = new List<string>();
                        foreach (var item in items)
                        {
                            var item2 = item.Trim();
                            if (item.StartsWith("[") && item.EndsWith("]"))
                            {
                                itemsList.Add(item.Substring(1, item.Length - 2));
                            }
                            else
                            {
                                itemsList.Add(item2);
                            }
                        }
                        result.Add(variableName, itemsList);
                    }
                }
            }


            bindingContext.Result = ModelBindingResult.Success(result);

            return Task.CompletedTask;
        }
    }
}

