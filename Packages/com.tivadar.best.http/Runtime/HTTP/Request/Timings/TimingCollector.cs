using System;
using System.Collections.Generic;

using Best.HTTP.Hosts.Connections;
using Best.HTTP.Shared;
using Best.HTTP.Shared.PlatformSupport.Text;

namespace Best.HTTP.Request.Timings
{
    struct PartialEvent
    {
        public string EventName;
        public DateTime StartedAt;

        public PartialEvent(string eventName, DateTime startedAt)
        {
            this.EventName = eventName;
            this.StartedAt = startedAt;
        }

        public bool IsSet() => !string.IsNullOrEmpty(EventName) && StartedAt != DateTime.MinValue;

        public override string ToString() => $"[PartialEvent '{EventName}', {StartedAt.ToString("hh:mm:ss.fffffff")}]";
    }

    /// <summary>
    /// Helper class to store, calculate and manage request related events and theirs duration, referenced by <see cref="HTTPRequest.Timing"/> field.
    /// </summary>
    public sealed class TimingCollector
    {
        public HTTPRequest ParentRequest { get; }

        /// <summary>
        /// When the TimingCollector instance created.
        /// </summary>
        public DateTime Created { get; private set; }

        /// <summary>
        /// When the closing Finish event is sent.
        /// </summary>
        public DateTime Finished { get; private set; }

        /// <summary>
        /// List of added events.
        /// </summary>
        public List<TimingEvent> Events { get; private set; }

        private PartialEvent _partialEvent;

        internal TimingCollector(HTTPRequest parentRequest)
        {
            this.ParentRequest = parentRequest;
            this.Created = DateTime.UtcNow;
            this.Finished = DateTime.MinValue;
            this._partialEvent = new PartialEvent(TimingEventNames.Initial, this.Created);
        }

        public void StartNext(string eventName) => RequestEventHelper.EnqueueRequestEvent(new RequestEventInfo(this.ParentRequest, new TimingEventInfo(this.ParentRequest, TimingEvents.StartNext, eventName)));

        internal void AddEvent(TimingEventInfo timingEvent)
        {
            if (HTTPManager.Logger.IsDiagnostic)
                HTTPManager.Logger.Information(nameof(TimingCollector), $"AddEvent({timingEvent}, {this._partialEvent})", ParentRequest.Context);

            switch(timingEvent.Event)
            {
                case TimingEvents.StartNext:
                    if (this._partialEvent.IsSet())
                    {
                        // If it's the same event name as the last one, merge the two event by not doing anything now.
                        if (timingEvent.Name.Equals(this._partialEvent.EventName, StringComparison.OrdinalIgnoreCase))
                            break;

                        AddEvent(this._partialEvent.EventName, this._partialEvent.StartedAt, timingEvent.Time - this._partialEvent.StartedAt);
                    }

                    if (this.Finished != DateTime.MinValue)
                        AddEvent(timingEvent.Name, timingEvent.Time, timingEvent.Time - this.Events[^1].Start);
                    else if (timingEvent.Name != null)
                        this._partialEvent = new PartialEvent(timingEvent.Name, timingEvent.Time);
                    break;

                case TimingEvents.Finish:
                    AddEvent(this._partialEvent.EventName, this._partialEvent.StartedAt, timingEvent.Time - this._partialEvent.StartedAt);

                    this._partialEvent = default;
                    var now = DateTime.UtcNow;
                    AddEvent(TimingEventNames.Finished, now, now - this.Created);

                    this.Finished = now;

                    if (HTTPManager.Logger.IsDiagnostic)
                        HTTPManager.Logger.Information(nameof(TimingCollector), this.ToString(),ParentRequest.Context);
                    break;
            }
        }

        /// <summary>
        /// When the event happened and for how long.
        /// </summary>
        internal void AddEvent(string name, DateTime when, TimeSpan duration)
        {
            if (this.Events == null)
                this.Events = new List<TimingEvent>(16);

            // Events can have zero ms durations, this block would set the same duration as the previous one,
            //  and that's clearly wrong.
            /*if (duration == TimeSpan.Zero)
            {
                DateTime prevEventAt = this.Created;
                if (this.Events.Count > 0)
                    prevEventAt = this.Events[this.Events.Count - 1].Start;
                duration = when - prevEventAt;
            }*/

            //if (name != TimingEventNames.Queued)
            //{
            //    int idx = this.Events.FindIndex(e => e.Name.Equals(name));
            //    if (idx != -1)
            //        HTTPManager.Logger.Warning(nameof(TimingCollector), $"'{name}' already present at idx '{idx}'({this.Events[idx].Start.ToString("hh:mm:ss.fffffff")}) new event time: {when.ToString("hh:mm:ss.fffffff")}! Current timing table: {this.ToString()}", this.ParentRequest.Context);
            //}

            this.Events.Add(new TimingEvent(name, when, duration));
        }

        public TimingEvent FindFirst(string name)
        {
            if (this.Events == null)
                return TimingEvent.Empty;

            for (int i = 0; i < this.Events.Count; ++i)
            {
                if (this.Events[i].Name == name)
                    return this.Events[i];
            }

            return TimingEvent.Empty;
        }

        public TimingEvent FindLast(string name)
        {
            if (this.Events == null)
                return TimingEvent.Empty;

            for (int i = this.Events.Count - 1; i >= 0; --i)
            {
                if (this.Events[i].Name == name)
                    return this.Events[i];
            }

            return TimingEvent.Empty;
        }

        public TimingEvent FindNext(TimingEvent relativeTo, string name)
        {
            int from = 0;

            if (relativeTo != TimingEvent.Empty)
            {
                for (int i = 0; i < this.Events.Count; i++)
                {
                    var ev = this.Events[i];

                    if (relativeTo.Equals(ev))
                    {
                        from = i + 1;
                        break;
                    }
                }
            }

            for (int i = from; i < this.Events.Count; i++)
            {
                var ev = this.Events[i];

                if (ev.Name == name)
                    return ev;
            }

            return TimingEvent.Empty;
        }

        public override string ToString()
        {
            var sb = StringBuilderPool.Get(0);

            sb.Append("{\"Created\": \"");
            sb.Append(this.Created.ToString("yyyy-MM-dd hh:mm:ss.fffffff"));

            sb.Append("\",\"Finished\": \"");
            sb.Append(this.Finished.ToString("yyyy-MM-dd hh:mm:ss.fffffff"));

            if (this.Events != null)
            {
                sb.Append("\", \"Events\": ");
                sb.Append('[');

                for (int i = 0; i < this.Events.Count; ++i)
                {
                    var @event = this.Events[i];

                    sb.Append("{\"Name\": \"");
                    sb.Append(@event.Name);
                    sb.Append("\", \"Duration\": \"");
                    sb.Append(@event.Duration);
                    sb.Append("\"}");

                    if (i < this.Events.Count - 1)
                        sb.Append(',');
                }

                sb.Append(']');
            }

            sb.Append('}');

            return StringBuilderPool.ReleaseAndGrab(sb);
        }
    }
}
