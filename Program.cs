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
            
            string staticRoot = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
    "Works", "NexScoreY", "NexScore", "Web");

            var server = new NexScore.Infrastructure.JudgingServer(
                NexScore.Database.Events.Database,
                staticRoot
            );
            server.Start(5100);// LAN port.

            Application.Run(new NexScore.LogInForm());
        }
    }
}
