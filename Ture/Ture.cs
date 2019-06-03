using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ture
{
    public class Ture
    {
        static void Main(string[] args)
        {
            LogManager.LoadConfiguration("Nlog.config");
            var log = LogManager.GetCurrentClassLogger();
            log.Debug("Woof!");

            Console.ReadLine();
        }
    }
}
