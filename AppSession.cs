using System;
using NexScore.Models;

namespace NexScore
{
    public static class AppSession
    {
        private static EventModel? _currentEvent;

        public static EventModel? CurrentEvent => _currentEvent;

        public static event Action<EventModel>? CurrentEventChanged;

        public static void SetCurrentEvent(EventModel evt)
        {
            _currentEvent = evt;
            CurrentEventChanged?.Invoke(evt);
        }
    }
}