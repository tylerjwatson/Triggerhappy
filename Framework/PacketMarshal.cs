using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;

namespace Wolfje.Plugins.SEconomy.Packets {
    static class PacketMarshal {

        /// <summary>
        /// Generically will marshal a structure from a supplied byte buffer.
        /// </summary>
        /// <typeparam name="T">The type of structure to return</typeparam>
        /// <param name="buffer">The input byte buffer</param>
		/// <param name="__struct">A reference to the output structure where the value will be filled.</param>
		/// <returns>0 if the function succeeded, < 0 otherwise. </returns>
		public static int FromBuffer<T>(byte[] buffer, out T __struct) where T : struct {
            T packetStruct;
			IntPtr allocHandle = IntPtr.Zero;
            int size = Marshal.SizeOf(new T());

			__struct = default(T);

			if (buffer.Length != size) {
				return -2;
			}

			try {
				allocHandle = Marshal.AllocHGlobal(size);
			} catch (Exception) {
				return -1;
			}

            try {
				Marshal.Copy(buffer, 0, allocHandle, size);
				packetStruct = (T)Marshal.PtrToStructure(allocHandle, typeof(T));
			} catch(Exception) {
				goto failed;
			}

			Marshal.FreeHGlobal(allocHandle);
			__struct = packetStruct;

			return 0;
failed: 
			Marshal.FreeHGlobal(allocHandle);
			return -1;
        }

    }
}
