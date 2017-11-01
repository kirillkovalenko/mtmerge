using System;
using System.Runtime.InteropServices;

namespace MTMerge.Interop
{
    // https://msdn.microsoft.com/en-us/library/windows/desktop/ms648022(v=vs.85).aspx

    [StructLayout(LayoutKind.Sequential)]
    internal struct MESSAGE_RESOURCE_ENTRY
    {
        /// <summary>
        /// The length, in bytes, of the MESSAGE_RESOURCE_ENTRY structure including text data.
        /// </summary>
        public UInt16 Length;

        /// <summary>
        /// Indicates this entry string encoding
        /// </summary>
        public MESSAGE_RESOURCE_ENTRY_ENCODING Flags;
    }
}