using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;

using PxWeb.Code.Api2.ModelBinder;

namespace PxWeb.UnitTests.ModelBinder
{
    [TestClass]
    public class GenericModelBinderTests
    {
        [TestMethod]
        public void NoParameter_ReturnsEmptyList()
        {
            // Arrange
            var binder = new CommaSeparatedStringToListOfStrings();
            var context = new Mock<ModelBindingContext>();
            context.SetupGet(x => x.ModelName).Returns("test");
            context.SetupGet(x => x.HttpContext.Request.Query.Keys).Returns(new List<string> { "test1" });
            context.SetupGet(x => x.HttpContext.Request.Query["test1"]).Returns(new StringValues("tom"));

            ModelBindingResult result = default;
            context.SetupSet(x => x.Result = It.IsAny<ModelBindingResult>())
                   .Callback<ModelBindingResult>(r => result = r);


            // Act
            binder.BindModelAsync(context.Object);

            //Assert
            int count = result.Model is null ? 0 : ((List<string>)result.Model).Count;
            Assert.AreEqual(0, count);
        }

        [TestMethod]
        public void OneParameter_ReturnsEmptyList()
        {
            // Arrange
            var binder = new CommaSeparatedStringToListOfStrings();
            var context = new Mock<ModelBindingContext>();
            context.SetupGet(x => x.ModelName).Returns("test");
            context.SetupGet(x => x.HttpContext.Request.Query.Keys).Returns(new List<string> { "test" });
            context.SetupGet(x => x.HttpContext.Request.Query["test"]).Returns(new StringValues("tom"));

            ModelBindingResult result = default;
            context.SetupSet(x => x.Result = It.IsAny<ModelBindingResult>())
                   .Callback<ModelBindingResult>(r => result = r);


            // Act
            binder.BindModelAsync(context.Object);

            //Assert
            int count = result.Model is null ? 0 : ((List<string>)result.Model).Count;
            Assert.AreEqual(1, count);
        }
    }
}
