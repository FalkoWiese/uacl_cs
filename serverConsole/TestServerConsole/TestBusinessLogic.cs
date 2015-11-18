using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;


namespace serverConsole
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
