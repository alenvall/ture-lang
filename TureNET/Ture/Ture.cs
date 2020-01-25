using System;

namespace Ture
{
    class Ture
    {
        private static Logger _log;

        static void Main(string[] args)
        {
            _log = new Logger();

            _log.Info("Woof!");
        }
    }
}
