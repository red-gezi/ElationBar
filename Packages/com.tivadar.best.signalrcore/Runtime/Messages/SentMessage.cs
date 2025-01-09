using Best.HTTP.Shared.PlatformSupport.Memory;

namespace Best.SignalR.Messages
{
    internal readonly struct SentMessage
    {
        public readonly long SequenceId;
        public readonly Message Message;

        public SentMessage(long sequenceId, Message message)
        {
            SequenceId = sequenceId;
            Message = message;
        }

        public override string ToString()
            => $"[SentMessage {SequenceId}, {Message}]";
    }
}
