using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Options;

using PxWeb.Code.BackgroundWorker;
using PxWeb.Controllers.Api2.Admin;

namespace PxWebApi.BigTests.Other
{
    [TestClass]
    public class Datetime_vs_Culture
    {
        [TestMethod]
        public void DatetimeToStringWithInvariantCulture()
        {
            var aDatetime = new DateTime(2020, 12, 24, 16, 59, 59);

            var customCulture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            customCulture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
            customCulture.DateTimeFormat.LongDatePattern = "yyyy-MM-dd";
            customCulture.DateTimeFormat.ShortTimePattern = "HH:mm:ss";
            customCulture.DateTimeFormat.LongTimePattern = "HH:mm:ss";

            Thread.CurrentThread.CurrentCulture = customCulture;

            var withCC = aDatetime.ToString();
            Assert.AreEqual("2020-12-24 16:59:59", withCC);

            // Clone the InvariantCulture
            customCulture = (CultureInfo)CultureInfo.InvariantCulture.Clone();

            // Set the desired date and time pattern
            customCulture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
            customCulture.DateTimeFormat.LongDatePattern = "yyyy-MM-dd";
            customCulture.DateTimeFormat.ShortTimePattern = "HH:mm:ss";
            customCulture.DateTimeFormat.LongTimePattern = "HH:mm:ss";

            // Apply the custom culture to the current thread
            Thread.CurrentThread.CurrentCulture = customCulture;
            Thread.CurrentThread.CurrentUICulture = customCulture;



            var withIC = aDatetime.ToString();
            Assert.AreEqual("2020-12-24 16:59:59", withIC);
        }

    }
}
