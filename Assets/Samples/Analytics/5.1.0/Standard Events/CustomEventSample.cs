using System;
using System.Collections.Generic;

namespace Unity.Services.Analytics
{
    public class MyEvent : Event
    {
        public MyEvent() : base("myEvent")
        {
        }

        public string FabulousString { set { SetParameter("fabulousString", value); } }
        public int SparklingInt { set { SetParameter("sparklingInt", value); } }
        public long TremendousLong { set { SetParameter("tremendousLong", value); } }
        public float SpectacularFloat { set { SetParameter("spectacularFloat", value); } }
        public double IncredibleDouble { set { SetParameter("incredibleDouble", value); } }
        public bool PeculiarBool { set { SetParameter("peculiarBool", value); } }
    }

    public static class CustomEventSample
    {
        public static void RecordCustomEventWithNoParameters()
        {
            // NOTE: this will show up on the dashboard as an invalid event, unless
            // you have created a schema that matches it.
            AnalyticsService.Instance.RecordEvent("myEvent");
        }

        public static void RecordCustomEventWithParameters()
        {
            // NOTE: this will show up on the dashboard as an invalid event, unless
            // you have created a schema that matches it.
            MyEvent myEvent = new MyEvent
            {
                FabulousString = "hello there",
                SparklingInt = 1337,
                TremendousLong = Int64.MaxValue,
                SpectacularFloat = 0.451f,
                IncredibleDouble = 0.000000000000000031337,
                PeculiarBool = true
            };

            AnalyticsService.Instance.RecordEvent(myEvent);
        }
    }
}
