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
            // Add this line after Database.Initialize();
            string staticRoot = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
    "Works", "NexScoreX", "NexScore", "Web");

            // OR if you prefer absolute without constructing:
            // string staticRoot = @"C:\Users\<YOUR_USER>\Documents\Works\NexScoreX\NexScore\Web";

            var server = new NexScore.Infrastructure.JudgingServer(
                NexScore.Database.Events.Database,
                staticRoot
            );
            server.Start(5100);// LAN port. Open firewall for TCP 5100 inbound.

            Application.Run(new NexScore.MainForm());
        }
    }
}
