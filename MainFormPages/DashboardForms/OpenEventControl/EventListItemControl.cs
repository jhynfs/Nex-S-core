using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NexScore.Models;

namespace NexScore.MainFormPages.DashboardForms.OpenEventControl
{
    public partial class EventListItemControl : UserControl
    {
        public EventModel? Event { get; private set; }
        public bool IsSelected { get; private set; }

        public event Action<EventListItemControl>? Selected;
        public EventListItemControl()
        {
            InitializeComponent();
            WireClickRelay();
        }

        public EventListItemControl(EventModel evt) : this()
        {
            Bind(evt);
        }
        public void Bind(EventModel evt)
        {
            Event = evt;
            _lblName.Text = evt.EventName;
            _lblDate.Text = evt.EventDate.ToString("MMM dd, yyyy");
        }

        public void SetSelected(bool selected)
        {
            IsSelected = selected;
            this.BackColor = selected
                ? Color.FromArgb(66, 70, 130)
                : Color.FromArgb(35, 37, 70);
            this.BorderStyle = selected ? BorderStyle.Fixed3D : BorderStyle.FixedSingle;
        }
        private void WireClickRelay()
        {
            // Relay clicks from all child controls to the parent handler
            this.Click += (_, __) => Selected?.Invoke(this);
            _icon.Click += (_, __) => Selected?.Invoke(this);
            _rightContainer.Click += (_, __) => Selected?.Invoke(this);
            _lblName.Click += (_, __) => Selected?.Invoke(this);
            _lblDate.Click += (_, __) => Selected?.Invoke(this);
        }
    }
}
