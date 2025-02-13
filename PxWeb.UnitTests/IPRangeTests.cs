using PxWeb.Middleware;

namespace PxWeb.UnitTests
{
    [TestClass]
    public class IPRangeTests
    {
        [TestMethod]
        public void IPv6InRange_ReturnsTrue()
        {
            Assert.IsTrue(AdminProtectionIpWhitelistMiddleware.IsInRange("::ffff:192.168.2.77", "::ffff:192.168.0.0/112"));
        }

        [TestMethod]
        public void IPv6OutsideofRange_ReturnsFalse()
        {
            Assert.IsTrue(!AdminProtectionIpWhitelistMiddleware.IsInRange("::ffff:172.17.2.77", "::ffff:192.168.0.0/112"));
        }

        [TestMethod]
        public void IPv4InRange_ReturnsTrue()
        {
            Assert.IsTrue(AdminProtectionIpWhitelistMiddleware.IsInRange("192.168.2.77", "192.168.0.0/16"));
        }

        [TestMethod]
        public void IPv4OutsideofRange_ReturnsFalse()
        {
            Assert.IsTrue(!AdminProtectionIpWhitelistMiddleware.IsInRange("172.17.2.77", "192.168.0.0/16"));
        }
    }
}
