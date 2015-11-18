using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using serverConsole;

namespace serverConsole
{
    public class BusinessLogic
    {
        public bool CalculateJob(String name, int id)
        {
            System.Console.WriteLine("Job Started ... " + name + " (" + id + ")");
            return true;
        }
    }
}
