using NexScore;
using System;
using System.Windows.Forms;

namespace NexScoreAdmin
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            Database.Initialize();

            Application.Run(new NexScore.MainForm());
        }
    }
}
