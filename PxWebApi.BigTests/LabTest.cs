using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Options;
using PxWeb.Code.BackgroundWorker;
using PxWeb.Controllers.Api2.Admin;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace PxWebApi.BigTests
{
    [TestClass]
    public class LabTest
    {
        [TestMethod]


        // TODO set Thread.CurrentThread.CurrentCulture  in all methods

        public void Raw()
        {
            var raw = DateTime.Now.ToString();
            Assert.AreEqual("raw,"+ CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern, raw);
        }

        [TestMethod]
        public void withInvariantCulture()
        {
            // Clone the InvariantCulture
            CultureInfo customCulture = (CultureInfo)CultureInfo.InvariantCulture.Clone();

            // Set the desired date and time pattern
            customCulture.DateTimeFormat.ShortDatePattern = "yyyy_MM_dd";
            customCulture.DateTimeFormat.LongDatePattern = "yyyy_MM_dd";
            customCulture.DateTimeFormat.ShortTimePattern = "HH:mm";
            customCulture.DateTimeFormat.LongTimePattern = "HH:mm";

            // Apply the custom culture to the current thread
            Thread.CurrentThread.CurrentCulture = customCulture;
            Thread.CurrentThread.CurrentUICulture = customCulture;

            var withIC = DateTime.Now.ToString();
            Assert.AreEqual("withIC ," + CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern, withIC);
        }

    }
}
