using System.Linq;

using PxWeb.Api2.Server.Models;
using PxWeb.Converters;

namespace PxWeb.Helper.Api2
{
    public static class OutputParameterUtil
    {
        public static (string, List<string>) TranslateOutputParamters(OutputFormatType? outputFormat, string defaultOutputFormat, List<OutputFormatParamType>? outputFormatParams, out bool paramError)
        {
            paramError = false;
            string format;
            List<string> formatParams;
            try
            {
                if (outputFormat is not null)
                {
                    format = EnumConverter.ToEnumString(outputFormat.Value);
                }
                else
                {
                    format = defaultOutputFormat;
                }

                if (outputFormatParams is not null)
                {
                    formatParams = outputFormatParams.Select(p => EnumConverter.ToEnumString(p)).ToList();
                }
                else
                {
                    formatParams = new List<string>();
                }

                if ((format.Equals("px", StringComparison.OrdinalIgnoreCase) ||
                    format.Equals("json-stat2", StringComparison.OrdinalIgnoreCase) ||
                    format.Equals("px-json", StringComparison.OrdinalIgnoreCase) ||
                    format.Equals("parquet", StringComparison.OrdinalIgnoreCase)) &&
                    formatParams.Count > 0)
                {
                    paramError = true;
                }

                if (!format.Equals("csv", StringComparison.OrdinalIgnoreCase) && !paramError)
                {
                    //Check if there is a invalid parameter
                    paramError = (formatParams.Where(p => p.StartsWith("separator", StringComparison.OrdinalIgnoreCase)).ToList().Count > 0);
                }
            }
            catch (ArgumentException)
            {
                paramError = true;
                format = "";
                formatParams = new List<string>();
            }
            return (format, formatParams);
        }


    }
}
