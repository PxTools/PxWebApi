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

                if (!format.Equals("CSV", StringComparison.OrdinalIgnoreCase))
                {
                    //Check if there is a invalid parameter
                    paramError = (formatParams.Select(p => p.StartsWith("separator", StringComparison.OrdinalIgnoreCase)).ToList().Count > 0);
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
