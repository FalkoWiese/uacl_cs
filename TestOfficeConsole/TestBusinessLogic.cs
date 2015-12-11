using NUnit.Framework;
using OfficeConsole;

namespace TestOfficeConsole
{
    [TestFixture]
    public class TestBusinessLogic
    {
        [Test]
        public void CallGetStatistics()
        {
            BusinessLogic logic = new BusinessLogic();
            Assert.IsNotNull(logic);
            var statisticsResult = logic.GetStatistics(0);
            Assert.IsTrue(statisticsResult.Length > 0);
            Assert.IsTrue(statisticsResult.Contains("statistics"));
        }

    }
}
