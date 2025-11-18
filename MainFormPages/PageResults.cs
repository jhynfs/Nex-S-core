using System;
using System.Windows.Forms;
using NexScore.Models;

namespace NexScore.MainFormPages
{
    public partial class PageResults : UserControl
    {
        public PageResults()
        {
            InitializeComponent();

            AppSession.CurrentEventChanged += OnCurrentEventChanged;
            if (AppSession.CurrentEvent != null)
                OnCurrentEventChanged(AppSession.CurrentEvent);
        }

        private void OnCurrentEventChanged(EventModel evt)
        {
            // Load and compute results for this event
            // Example: query scores, aggregate, and bind to grid/chart
            // resultsGrid.DataSource = await ResultsService.LoadResultsAsync(evt.Id);
        }
    }
}