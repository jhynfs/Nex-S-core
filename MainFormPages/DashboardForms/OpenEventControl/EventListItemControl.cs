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

        // Raised on single-click selection
        public event Action<EventListItemControl>? Selected;

        // Raised on double-click to request opening/confirming the event
        public event Action<EventListItemControl>? Activated;

        public EventListItemControl()
        {
            InitializeComponent();

            // Make sure the control surface raises DoubleClick
            this.SetStyle(ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, true);

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
            void RelaySelect() => Selected?.Invoke(this);
            void RelayActivate()
            {
                // Ensure selection then request activation/open
                Selected?.Invoke(this);
                Activated?.Invoke(this);
            }

            // Parent surface
            this.Click += (_, __) => RelaySelect();
            this.DoubleClick += (_, __) => RelayActivate();

            // Child surfaces also relay both click and double-click
            _icon.Click += (_, __) => RelaySelect();
            _icon.DoubleClick += (_, __) => RelayActivate();

            _rightContainer.Click += (_, __) => RelaySelect();
            _rightContainer.DoubleClick += (_, __) => RelayActivate();

            _lblName.Click += (_, __) => RelaySelect();
            _lblName.DoubleClick += (_, __) => RelayActivate();

            _lblDate.Click += (_, __) => RelaySelect();
            _lblDate.DoubleClick += (_, __) => RelayActivate();
        }
    }
}