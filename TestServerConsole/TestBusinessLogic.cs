using NUnit.Framework;


namespace ServerConsole
{
    [TestFixture]
    public class TestBusinessLogic
    {
        [Test]
        public void CallCalculate()
        {
            BusinessLogic logic = new BusinessLogic();
            Assert.IsNotNull(logic);
            var jobName = "Job";
            for (var state = 0; state < 10; state++)
            {
                var result = logic.CalculateJob(jobName, state);
                if(state <= 3) Assert.IsTrue(result);
                else Assert.IsFalse(result);
            }
        }

        [Test]
        public void CallJobStates()
        {
            BusinessLogic logic = new BusinessLogic();
            Assert.IsNotNull(logic);
            var result = logic.States();
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length > 0);
        }

    }
}
