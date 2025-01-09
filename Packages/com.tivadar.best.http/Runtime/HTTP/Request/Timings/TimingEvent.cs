using System;

namespace Best.HTTP.Request.Timings
{
    /// <summary>
    /// Struct to hold information about one timing event recorded for a <see cref="HTTPRequest"/>. Timing events are managed by the <see cref="TimingCollector"/>.
    /// </summary>
    public struct TimingEvent : IEquatable<TimingEvent>
    {
        public static readonly TimingEvent Empty = new TimingEvent(null, TimeSpan.Zero);

        /// <summary>
        /// Name of the event
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Duration of the event.
        /// </summary>
        public readonly TimeSpan Duration;

        /// <summary>
        /// When the event started.
        /// </summary>
        public readonly DateTime Start;

        public TimingEvent(string name, TimeSpan duration)
        {
            this.Name = name;
            this.Duration = duration;
            this.Start = DateTime.UtcNow;
        }

        public TimingEvent(string name, DateTime when, TimeSpan duration)
        {
            this.Name = name;
            this.Start = when;
            this.Duration = duration;
        }

        public TimeSpan CalculateDuration(TimingEvent tEvent)
        {
            if (this.Start < tEvent.Start)
                return tEvent.Start - this.Start;

            return this.Start - tEvent.Start;
        }

        public bool Equals(TimingEvent other)
        {
            return this.Name == other.Name &&
                   this.Duration == other.Duration &&
                   this.Start == other.Start;
        }

        public override bool Equals(object obj)
        {
            if (obj is TimingEvent)
                return this.Equals((TimingEvent)obj);

            return false;
        }

        public override int GetHashCode() => (this.Name != null ? this.Name.GetHashCode() : 0) ^ this.Duration.GetHashCode() ^ this.Start.GetHashCode();

        public static bool operator ==(TimingEvent lhs, TimingEvent rhs) => lhs.Equals(rhs);

        public static bool operator !=(TimingEvent lhs, TimingEvent rhs) => !lhs.Equals(rhs);

        public override string ToString() => $"['{this.Name}': {this.Duration} ({this.Start.ToString("hh:mm:ss.fffffff")})]";

    }
}
