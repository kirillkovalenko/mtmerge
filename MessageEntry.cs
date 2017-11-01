using System;

namespace MTMerge
{
    internal sealed class MessageEntry
    {
        public MessageEntry(UInt32 id, String message)
        {
            Id = id;
            Message = message;
        }

        public UInt32 Id { get; }

        public String Message { get; }
    }
}