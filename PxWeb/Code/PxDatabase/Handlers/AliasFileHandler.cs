﻿using Microsoft.Extensions.Logging;

namespace PXWeb.Database
{
    /// <summary>
    /// Item handler for Alias files in PX file databases
    /// </summary>
    public class AliasFileHandler : IItemHandler
    {
        private readonly ILogger _logger;
        private readonly PxApiConfigurationOptions _configOptions;

        public AliasFileHandler(PxApiConfigurationOptions configOptions, ILogger logger)
        {
            _configOptions = configOptions;
            _logger = logger;
        }

        #region IItemHandler Members

        /// <summary>
        /// Return the priority of the handler which is set to 4
        /// </summary>
        public int Priority
        {
            get { return 4; }
        }

        /// <summary>
        /// Checks if it is a Alias file
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <returns>
        /// Returns true if the filename starts with Alias and ends with .txt 
        /// oterwise it will return false
        /// </returns>
        /// <remarks>the check is case insensitive</remarks>
        public bool CanHandle(string path)
        {
            if (path.EndsWith(".txt", StringComparison.InvariantCultureIgnoreCase))
            {
                return System.IO.Path.GetFileName(path).StartsWith("Alias", StringComparison.InvariantCultureIgnoreCase);
            }
            return false;
        }

        /// <summary>
        /// Reads the Alias file and creates a AliasItem containg the information read
        /// </summary>
        /// <param name="path">Path to the Alias file</param>
        /// <returns>A AliasItem containg the information from the Alias file</returns>
        /// <remarks>
        /// If the alias file do not have a language specified in the name then the 
        /// default language specified in the settings will be used
        /// </remarks>
        public object? Handle(string path, DatabaseLogger logger)
        {
            string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
            int splittIndex = fileName.IndexOf('_');

            AliasItem aliasItem;
            string alias;
            string language;

            if (splittIndex < 0)
            {
                //No underscore in the file name use the default language
                language = _configOptions.DefaultLanguage;
            }
            else
            {
                language = fileName.Substring(splittIndex + 1);
            }
            try
            {
                alias = ReadAll(path);
            }
            catch (System.IO.IOException ex)
            {
                logger(new DatabaseMessage() { MessageType = DatabaseMessage.BuilderMessageType.Error, Message = "Could not read file " + path });
                _logger.LogWarning(ex.ToString());
                return null;
            }

            aliasItem = new AliasItem(alias, language);
            return aliasItem;
        }

        /// <summary>
        /// Reads all text from the specified file
        /// </summary>
        /// <param name="path">Path to the file to read</param>
        /// <returns>the contents of the file</returns>
        private string ReadAll(string path)
        {
            System.Text.Encoding encoding = PCAxis.Paxiom.Parsers.PXFileParser.GetEncoding(path);
            string data;

            using (System.IO.StreamReader reader =
                    new System.IO.StreamReader(
                        new System.IO.FileStream(path,
                                                 System.IO.FileMode.Open,
                                                 System.IO.FileAccess.Read,
                                                 System.IO.FileShare.Read,
                                                 2048),
                                                encoding))
            {
                data = reader.ReadToEnd();
            }
            return data;
        }

        #endregion
    }

}
