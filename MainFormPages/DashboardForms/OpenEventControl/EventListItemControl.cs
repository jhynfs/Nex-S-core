using System;
using System.Drawing;
using System.Windows.Forms;
using NexScore.Models;

namespace NexScore.MainFormPages.DashboardForms.OpenEventControl
{
    public partial class EventListItemControl : UserControl
    {
        public EventModel? Event { get; private set; }
        public bool IsSelected { get; private set; }

        // Single-click selection
        public event Action<EventListItemControl>? Selected;
        // Double-click activation (open immediately)
        public event Action<EventListItemControl>? Activated;

        public EventListItemControl()
        {
            InitializeComponent();

            // Ensure this control can raise double-click
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
            // Single-click: select only
            void RelaySelect() => Selected?.Invoke(this);
            // Double-click: select then activate
            void RelayActivate()
            {
                Selected?.Invoke(this);
                Activated?.Invoke(this);
            }

            // Parent surface
            this.Click += (_, __) => RelaySelect();
            this.MouseDoubleClick += (_, __) => RelayActivate();

            // Child controls: make sure they also relay both
            _icon.Click += (_, __) => RelaySelect();
            _icon.MouseDoubleClick += (_, __) => RelayActivate();

            _rightContainer.Click += (_, __) => RelaySelect();
            _rightContainer.MouseDoubleClick += (_, __) => RelayActivate();

            _lblName.Click += (_, __) => RelaySelect();
            _lblName.MouseDoubleClick += (_, __) => RelayActivate();

            _lblDate.Click += (_, __) => RelaySelect();
            _lblDate.MouseDoubleClick += (_, __) => RelayActivate();
        }
    }
}