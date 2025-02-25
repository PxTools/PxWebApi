using PxWeb.Middleware;

namespace PxWeb.UnitTests.Admin
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
            Assert.IsFalse(AdminProtectionIpWhitelistMiddleware.IsInRange("::ffff:172.17.2.77", "::ffff:192.168.0.0/112"));
        }

        [TestMethod]
        public void IPv4InRange_ReturnsTrue()
        {
            Assert.IsTrue(AdminProtectionIpWhitelistMiddleware.IsInRange("192.168.2.77", "192.168.0.0/16"));
        }

        [TestMethod]
        public void IPv4OutsideofRange_ReturnsFalse()
        {
            Assert.IsFalse(AdminProtectionIpWhitelistMiddleware.IsInRange("172.17.2.77", "192.168.0.0/16"));
        }

        [TestMethod]
        public void IPv6ExactMatch_ReturnsTrue()
        {
            Assert.IsTrue(AdminProtectionIpWhitelistMiddleware.IsInRange("::ffff:192.168.2.77", "::ffff:192.168.2.77/128"));
        }

        [TestMethod]
        public void IPv6DifferentSubnet_ReturnsFalse()
        {
            Assert.IsFalse(AdminProtectionIpWhitelistMiddleware.IsInRange("::ffff:192.168.2.77", "::ffff:192.169.0.0/112"));
        }

        [TestMethod]
        public void IPv4ExactMatch_ReturnsTrue()
        {
            Assert.IsTrue(AdminProtectionIpWhitelistMiddleware.IsInRange("192.168.2.77", "192.168.2.77/32"));
        }

        [TestMethod]
        public void IPv4DifferentSubnet_ReturnsFalse()
        {
            Assert.IsFalse(AdminProtectionIpWhitelistMiddleware.IsInRange("192.168.2.77", "192.169.0.0/16"));
        }

        [TestMethod]
        public void IPv6InRangeWithDifferentPrefixLength_ReturnsTrue()
        {
            Assert.IsTrue(AdminProtectionIpWhitelistMiddleware.IsInRange("::ffff:192.168.2.77", "::ffff:192.168.0.0/96"));
        }

        [TestMethod]
        public void IPv4InRangeWithDifferentPrefixLength_ReturnsFalse()
        {
            Assert.IsFalse(AdminProtectionIpWhitelistMiddleware.IsInRange("192.168.2.77", "192.168.0.0/24"));
        }

        [TestMethod]
        public void IPv6LoopbackInRange_ReturnsTrue()
        {
            Assert.IsTrue(AdminProtectionIpWhitelistMiddleware.IsInRange("::1", "::1/128"));
        }

        [TestMethod]
        public void IPv4LoopbackInRange_ReturnsTrue()
        {
            Assert.IsTrue(AdminProtectionIpWhitelistMiddleware.IsInRange("127.0.0.1", "127.0.0.1/32"));
        }

        [TestMethod]
        public void IPv6LoopbackOutsideOfRange_ReturnsFalse()
        {
            Assert.IsFalse(AdminProtectionIpWhitelistMiddleware.IsInRange("::1", "::2/128"));
        }

        [TestMethod]
        public void IPv4LoopbackOutsideOfRange_ReturnsFalse()
        {
            Assert.IsFalse(AdminProtectionIpWhitelistMiddleware.IsInRange("127.0.0.1", "127.0.0.2/32"));
        }
    }
}
