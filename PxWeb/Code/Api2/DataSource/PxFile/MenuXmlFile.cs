using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

using DocumentFormat.OpenXml.Wordprocessing;

using Px.Abstractions.Interfaces;

namespace PxWeb.Code.Api2.DataSource.PxFile
{

    public class MenuXmlFile
    {
        private readonly IPxHost _hostingEnvironment;
        public MenuXmlFile(IPxHost hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }


        /// <summary>
        /// Get the whole Menu.xml as an XmlDocument
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">If file does not exist</exception>
        public XmlDocument GetAsXmlDocument()
        {
            string xmlFilePath = Path.Combine(_hostingEnvironment.RootPath, "Database", "Menu.xml");

            if (!System.IO.File.Exists(xmlFilePath))
            {
                throw new Exception(String.Format("File {0}, does not exists.", xmlFilePath));
            }

            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(xmlFilePath);
            return xdoc;
        }


        /// <summary>
        /// Get the Language-element for the given language as an XmlDocument
        /// </summary>
        /// <param name="language"></param>
        /// <returns>If file does not exist</returns>
        internal XmlDocument GetLanguageAsXmlDocument(string language)
        {
            string myOut = string.Empty;

            XmlDocument xdoc = GetAsXmlDocument();

            string xpathExpression = "//Language[@lang=$language]";
            XPathExpression xpath = XPathExpression.Compile(xpathExpression);

            XsltArgumentList varList = new XsltArgumentList();
            varList.AddParam("language", string.Empty, language);

            CustomXPathContext xpathContext = new CustomXPathContext(new NameTable(), varList);
            xpath.SetContext(xpathContext);

            XPathNavigator? nav = xdoc.CreateNavigator();
            if (nav != null)
            {
                XPathNodeIterator iter = nav.Select(xpath);

                if (!iter.Count.Equals(0))
                {
                    while (iter.MoveNext())
                    {
                        XPathNavigator? node = iter.Current;

                        if (node != null)
                        {
                            myOut = node.OuterXml;

                        }
                    }
                }
            }

            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(myOut);
            return xmldoc;
        }
    }
}
