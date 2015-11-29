using System;
using NUnit.Framework;


namespace ServerConsole
{
    [TestFixture]
    public class TestBusinessLogic
    {
        [Test]
        public void testCalculate()
        {
            BusinessLogic logic = new BusinessLogic();
            Assert.IsNotNull(logic);
            String name = "BO Logic";
            int id = 1;
            var result = logic.CalculateJob(name, id);
            Assert.IsTrue(result);
        }
    }
}
