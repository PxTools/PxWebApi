﻿using System;

using Microsoft.Extensions.Options;

using PxWeb.Code.Api2.SavedQueryBackend.FileBackend;

namespace PxWeb.UnitTests.SavedQuery
{
    [TestClass]
    public class SaveQueryFileStorgeBackendTests
    {
        [TestMethod]
        public void UpdatedRunStatistics_WhenNoFileAndNoQuery_ReturnsFalse()
        {
            // Arrange
            var settings = new SavedQueryFileStorageOptions() { Path = "SQ" };
            var options = new Mock<IOptions<SavedQueryFileStorageOptions>>();
            options.Setup(x => x.Value).Returns(settings);
            var host = new Mock<IPxHost>();
            host.Setup(h => h.RootPath).Returns(Path.GetTempPath());
            var backend = new SaveQueryFileStorgeBackend(options.Object, host.Object);

            // Act
            var result = backend.UpdateRunStatistics("no-id");

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void UpdatedRunStatistics_ThrowsArgumentExcpetion_WhenIdContainsInvalidCharacters()
        {
            // Arrange
            var settings = new SavedQueryFileStorageOptions() { Path = "SQ" };
            var options = new Mock<IOptions<SavedQueryFileStorageOptions>>();
            options.Setup(x => x.Value).Returns(settings);
            var host = new Mock<IPxHost>();
            host.Setup(h => h.RootPath).Returns(Path.GetTempPath());
            var backend = new SaveQueryFileStorgeBackend(options.Object, host.Object);

            // Assert
            Assert.ThrowsExactly<ArgumentException>(() => backend.UpdateRunStatistics("/../"));
        }


        [TestMethod]
        public void UpdatedRunStatistics_WhenNoFileButQuery_ReturnTrueAndCreatesFile()
        {
            // Arrange
            var settings = new SavedQueryFileStorageOptions() { Path = "SQ" };
            var options = new Mock<IOptions<SavedQueryFileStorageOptions>>();
            options.Setup(x => x.Value).Returns(settings);
            var host = new Mock<IPxHost>();
            host.Setup(h => h.RootPath).Returns(Path.GetTempPath());
            var backend = new SaveQueryFileStorgeBackend(options.Object, host.Object);

            var path = Path.Combine(Path.GetTempPath(), "SQ", "i");
            Directory.CreateDirectory(path);
            File.CreateText(Path.Combine(path, "id.sqa")).Close();

            // Act
            var result = backend.UpdateRunStatistics("id");

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ContainsInvalidPathChars_ReturnTrue_WhenidContainsDottedPathChars()
        {
            // Act
            var result = SaveQueryFileStorgeBackend.ContainsInvalidPathChars("/../");

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ContainsInvalidPathChars_ReturnTrue_WhenidContainsSlashPathChars()
        {
            // Act
            var result = SaveQueryFileStorgeBackend.ContainsInvalidPathChars("/test");

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ContainsInvalidPathChars_ReturnTrue_WhenidContainsBackSlashPathChars()
        {
            // Act
            var result = SaveQueryFileStorgeBackend.ContainsInvalidPathChars("\\test");

            // Assert
            Assert.IsTrue(result);
        }
    }


}
