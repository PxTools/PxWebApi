using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace PxWeb.Code.Api2.DataSource.PxFile
{
    public class CustomXPathContext : System.Xml.Xsl.XsltContext
    {
        //From https://learn.microsoft.com/en-us/dotnet/standard/data/xml/user-defined-functions-and-variables


        // XsltArgumentList to store names and values of user-defined variables.
        private readonly XsltArgumentList argList;


        public CustomXPathContext(NameTable nt, XsltArgumentList args) : base(nt)
        {
            argList = args;
        }


        // Function to resolve references to user-defined XPath
        // extension variables in XPath query.
        public override System.Xml.Xsl.IXsltContextVariable ResolveVariable(string prefix, string name)
        {
            // Create an instance of an XPathExtensionVariable
            // (custom IXsltContextVariable implementation) object
            //  by supplying the name of the user-defined variable to resolve.
            XPathExtensionVariable var = new XPathExtensionVariable(prefix, name);

            // The Evaluate method of the returned object will be used at run time
            // to resolve the user-defined variable that is referenced in the XPath
            // query expression.
            return var;
        }

        // Function to resolve references to user-defined XPath extension
        // functions in XPath query expressions evaluated by using an
        // instance of this class as the XsltContext.
        public override System.Xml.Xsl.IXsltContextFunction ResolveFunction(
                                    string prefix, string name,
                                    System.Xml.XPath.XPathResultType[] argTypes)
        {
            // Return null if none of the functions match name.
#pragma warning disable CS8603 // Possible null reference return.
            return null;
#pragma warning restore CS8603 // Possible null reference return.
        }


        public override bool PreserveWhitespace(System.Xml.XPath.XPathNavigator node)
        {
            return false;
        }

        // empty implementation, returns 0.
        public override int CompareDocument(string baseUri, string nextbaseUri)
        {
            return 0;
        }

        public override bool Whitespace
        {
            get
            {
                return true;
            }
        }

        // The XsltArgumentList property is accessed by the Evaluate method of the
        // XPathExtensionVariable object that the ResolveVariable method returns. It is used
        // to resolve references to user-defined variables in XPath query expressions.
        public XsltArgumentList ArgList
        {
            get
            {
                return argList;
            }
        }
    }

    // The interface used to resolve references to user-defined variables
    // in XPath query expressions at run time. An instance of this class
    // is returned by the overridden ResolveVariable function of the
    // custom XsltContext class.
    public class XPathExtensionVariable : IXsltContextVariable
    {
        // Namespace of user-defined variable.
        private readonly string prefix;
        // The name of the user-defined variable.
        private readonly string varName;

        // Constructor used in the overridden ResolveVariable function of custom XsltContext.
        public XPathExtensionVariable(string prefix, string varName)
        {
            this.prefix = prefix;
            this.varName = varName;
        }

        // Function to return the value of the specified user-defined variable.
        // The GetParam method of the XsltArgumentList property of the active
        // XsltContext object returns value assigned to the specified variable.
        public object Evaluate(System.Xml.Xsl.XsltContext xsltContext)
        {
            XsltArgumentList vars = ((CustomXPathContext)xsltContext).ArgList;
            //if(varName.Equals("language"))
            //{
            //    var theValue = vars.GetParam("language", "");
            //    //Say bang here
            //}
            //else
            //{
            //   throw new NotImplementedException("We do not support other var than language");
            //}
            Object? myOut = vars.GetParam(varName, prefix);
            if (myOut == null)
            {
                throw new Exception("Something went wrong!");
            }

            return myOut;
        }

        // Determines whether this variable is a local XSLT variable.
        // Needed only when using a style sheet.
        public bool IsLocal
        {
            get
            {
                return false;
            }
        }

        // Determines whether this parameter is an XSLT parameter.
        // Needed only when using a style sheet.
        public bool IsParam
        {
            get
            {
                return false;
            }
        }

        public System.Xml.XPath.XPathResultType VariableType
        {
            get
            {
                return XPathResultType.Any;
            }
        }
    }
}
