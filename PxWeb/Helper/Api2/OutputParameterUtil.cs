using System.Linq;

using PxWeb.Api2.Server.Models;
using PxWeb.Converters;

namespace PxWeb.Helper.Api2
{
    public static class OutputParameterUtil
    {

        public static (string, List<string>) TranslateOutputParamters(OutputFormatType? outputFormat, PxApiConfigurationOptions configOptions, List<OutputFormatParamType>? outputFormatParams, out bool paramError)
        {


            paramError = false;
            string format;
            List<string> formatParams;
            try
            {
                if (outputFormat is not null)
                {
                    format = EnumConverter.ToEnumString(outputFormat.Value);

                    if (!configOptions.OutputFormats.Contains(format))
                    {
                        paramError = true;
                    }
                }
                else
                {
                    format = configOptions.DefaultOutputFormat;
                }

                if (outputFormatParams is not null)
                {
                    formatParams = outputFormatParams.Select(p => EnumConverter.ToEnumString(p)).ToList();
                }
                else
                {
                    formatParams = new List<string>();
                }


                // these output formats does not take any arguments
                if (new List<string>() { "px", "json-stat2", "px-json", "parquet" }.Contains(format) && formatParams.Count > 0)
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
