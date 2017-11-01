using MTMerge.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections;

namespace MTMerge
{
    internal sealed class MessageBlock : IEnumerable<MessageEntry>
    {
        private static readonly Int32 SizeOfEntry = Marshal.SizeOf<MESSAGE_RESOURCE_ENTRY>();

        public MessageBlock(UInt32 firstMessageId, IEnumerable<String> messages)
        {
            FirstMessageId = firstMessageId;
            Messages = new List<String>(messages).AsReadOnly();
        }

        public MessageBlock(ref MESSAGE_RESOURCE_BLOCK block, MemoryMappedViewAccessor memory)
        {
            List<String> messages = new List<String>();

            Int64 offset = block.OffsetToEntries;

            for (UInt32 messageid = block.LowId; messageid <= block.HighId; ++messageid)
            {
                MESSAGE_RESOURCE_ENTRY entry;
                memory.Read(offset, out entry);
                offset += SizeOfEntry;

                Byte[] data = new Byte[entry.Length - SizeOfEntry];
                memory.ReadArray(offset, data, 0, data.Length);
                offset += data.Length;

                String message;

                switch (entry.Flags)
                {
                    case MESSAGE_RESOURCE_ENTRY_ENCODING.Ansi:
                        message = Encoding.ASCII.GetString(data);
                        break;

                    case MESSAGE_RESOURCE_ENTRY_ENCODING.Unicode:
                        message = Encoding.Unicode.GetString(data);
                        break;

                    default:
                        throw new FileFormatException();
                }

                messages.Add(message);
            }

            FirstMessageId = block.LowId;
            Messages = messages.AsReadOnly();
        }

        public UInt32 FirstMessageId { get; }

        public UInt32 LastMessageId => FirstMessageId + (UInt32)Messages.Count - 1;

        public IReadOnlyList<String> Messages { get; }

        public IEnumerator<MessageEntry> GetEnumerator()
        {
            for (Int32 i = 0; i < Messages.Count; ++i)
            {
                yield return new MessageEntry(FirstMessageId + (UInt32)i, Messages[i]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal MESSAGE_RESOURCE_BLOCK Write(ref Int64 offset, MemoryMappedViewAccessor memory)
        {
            MESSAGE_RESOURCE_BLOCK result = new MESSAGE_RESOURCE_BLOCK
            {
                LowId = FirstMessageId,
                HighId = LastMessageId,
                OffsetToEntries = (UInt32)offset
            };

            foreach (String message in Messages)
            {
                Byte[] payload = Encoding.Unicode.GetBytes(message);

                MESSAGE_RESOURCE_ENTRY entry = new MESSAGE_RESOURCE_ENTRY
                {
                    Length = (UInt16)(payload.Length + SizeOfEntry),
                    Flags = MESSAGE_RESOURCE_ENTRY_ENCODING.Unicode
                };

                memory.Write(offset, ref entry);
                offset += SizeOfEntry;

                memory.WriteArray(offset, payload, 0, payload.Length);
                offset += payload.Length;
            }

            return result;
        }
    }
}