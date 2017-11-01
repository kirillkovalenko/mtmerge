using MTMerge.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.InteropServices;

namespace MTMerge
{
    internal sealed class MessageResource
    {
        public MessageResource(String filePath)
        {
            FileName = filePath;

            using (MemoryMappedFile mmfile = MemoryMappedFile.CreateFromFile(filePath, FileMode.Open, null, 0L, MemoryMappedFileAccess.Read))
            using (MemoryMappedViewAccessor memory = mmfile.CreateViewAccessor(0L, 0L, MemoryMappedFileAccess.Read))
            {
                UInt32 numberOfBlocks;
                memory.Read(0, out numberOfBlocks);

                MESSAGE_RESOURCE_BLOCK[] blocks = new MESSAGE_RESOURCE_BLOCK[numberOfBlocks];
                memory.ReadArray(sizeof(UInt32), blocks, 0, blocks.Length);

                MessageBlocks = blocks.Select(block => new MessageBlock(ref block, memory)).ToList().AsReadOnly();
            }
        }

        public MessageResource(IEnumerable<MessageEntry> entries)
        {
            List<MessageBlock> result = new List<MessageBlock>();

            UInt32 firstMessageId = 0;
            List<String> messages = new List<String>();

            foreach (var entry in entries.OrderBy(e => e.Id))
            {
                if (entry.Id != firstMessageId + messages.Count)
                {
                    if (messages.Count > 0) // special case on start
                    {
                        result.Add(new MessageBlock(firstMessageId, messages));
                    }

                    firstMessageId = entry.Id;
                    messages.Clear();
                }

                messages.Add(entry.Message);
            }

            // commit the last block
            if (messages.Count > 0)
            {
                result.Add(new MessageBlock(firstMessageId, messages));
            }

            MessageBlocks = result.AsReadOnly();
        }

        public MessageResource(IEnumerable<MessageBlock> blocks)
            : this(blocks.SelectMany(b => b))
        {
        }

        public MessageResource(IEnumerable<MessageResource> resources)
            : this(resources.SelectMany(r => r.MessageBlocks))
        {
        }

        public MessageResource(params MessageResource[] resources)
            : this(resources.SelectMany(r => r.MessageBlocks))
        {
        }

        public String FileName { get; private set; }

        public IReadOnlyList<MessageBlock> MessageBlocks { get; }

        public void Save(String filePath, Boolean overrideExisting)
        {
            FileMode fileMode = overrideExisting ? FileMode.Create : FileMode.CreateNew;

            Int64 finalLength = 0L;

            using (MemoryMappedFile mmfile = MemoryMappedFile.CreateFromFile(filePath, fileMode, null, 1024 * 1024, MemoryMappedFileAccess.ReadWrite))
            using (MemoryMappedViewAccessor memory = mmfile.CreateViewAccessor(0L, 0L, MemoryMappedFileAccess.Write))
            {
                Int64 offset = 0L;

                memory.Write(offset, (UInt32)MessageBlocks.Count);
                offset += sizeof(UInt32);

                MESSAGE_RESOURCE_BLOCK[] blocks = new MESSAGE_RESOURCE_BLOCK[MessageBlocks.Count];

                memory.WriteArray(sizeof(UInt32), blocks, 0, blocks.Length);
                offset += blocks.Length * Marshal.SizeOf<MESSAGE_RESOURCE_BLOCK>();

                for (Int32 i = 0; i < MessageBlocks.Count; ++i)
                {
                    blocks[i] = MessageBlocks[i].Write(ref offset, memory);
                }

                finalLength = offset;

                memory.WriteArray(sizeof(UInt32), blocks, 0, blocks.Length);
            }

            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite))
            {
                stream.SetLength(finalLength);
                stream.Flush();
            }

            FileName = filePath;
        }
    }
}