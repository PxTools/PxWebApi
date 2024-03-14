using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PxWeb.Code.Api2.NewtonsoftConfiguration
{
    /// <summary>
    /// This resolver is used to control the order of object properties when serialising to json.
    /// Properties of base classes will be displayed before properties of sub classes.
    /// </summary>
    public class BaseFirstContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var props = base.CreateProperties(type, memberSerialization);
            if (props == null)
            {
                throw new ArgumentNullException();
            }
            return props.OrderBy(p => p.DeclaringType.BaseTypesAndSelf().Count()).ToList();

        }

    }

}
