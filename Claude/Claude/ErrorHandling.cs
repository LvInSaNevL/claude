using System;
using System.Collections.Generic;
using System.Text;

namespace Claude
{
    class ErrorHandling
    {
        public static void ReRunInstaller()
        {
            Installer wizard = new Installer();
            wizard.Show();
        }

        public static void Logger(object message)
        {
            Console.WriteLine(message.ToString());
        }
    }
}
