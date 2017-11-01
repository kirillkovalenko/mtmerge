using System;
using System.Runtime.InteropServices;

namespace MTMerge.Interop
{
    // https://msdn.microsoft.com/en-us/library/windows/desktop/ms648020(v=vs.85).aspx

    [StructLayout(LayoutKind.Sequential)]
    internal struct MESSAGE_RESOURCE_BLOCK
    {
        /// <summary>
        /// The lowest message identifier contained within this structure.
        /// </summary>
        public UInt32 LowId;

        /// <summary>
        /// The highest message identifier contained within this structure.
        /// </summary>
        public UInt32 HighId;

        /// <summary>
        /// The offset, in bytes, from the beginning of the MESSAGE_RESOURCE_DATA structure
        /// to the MESSAGE_RESOURCE_ENTRY structures in this MESSAGE_RESOURCE_BLOCK.
        /// The MESSAGE_RESOURCE_ENTRY structures contain the message strings.
        /// </summary>
        public UInt32 OffsetToEntries;
    }
}