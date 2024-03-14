using System.Linq;
using System.Runtime.Serialization;

using PxWeb.Api2.Server.Models;

namespace PxWeb.Converters
{
    public class EnumConverter
    {
        public static Table.CategoryEnum ToCategoryEnum(string category)
        {
            Table.CategoryEnum enumCategory = new Table.CategoryEnum();
            var enumType = typeof(Table.CategoryEnum);

            foreach (var name in Enum.GetNames(enumType))
            {
                var type = enumType.GetField(name);

                if (type == null)
                {
                    throw new System.ArgumentNullException("Enum type not found");
                }

                var enumMemberAttribute = ((EnumMemberAttribute[])type.GetCustomAttributes(typeof(EnumMemberAttribute), true)).Single();

                if (enumMemberAttribute.Value == category)
                {
                    Table.CategoryEnum categoryEnum = (Table.CategoryEnum)Enum.Parse(enumType, name);
                    return enumCategory = categoryEnum;
                }
            }

            return enumCategory;
        }
    }
}
