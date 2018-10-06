/*
 * Copyright (c) 2018 Ac_K
 *
 * This program is free software; you can redistribute it and/or modify it
 * under the terms and conditions of the GNU General Public License,
 * version 2, as published by the Free Software Foundation.
 *
 * This program is distributed in the hope it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for
 * more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace BEAExtractor
{
    static class StreamExtended
    {
        public static T ReadStruct<T>(this Stream Stream) where T : struct
        {
            int    Size   = Marshal.SizeOf(typeof(T));
            byte[] Buffer = new byte[Size];

            Stream.Read(Buffer, 0, Size);

            GCHandle PinnedBuffer = GCHandle.Alloc(Buffer, GCHandleType.Pinned);
            T        Struct       = (T)Marshal.PtrToStructure(PinnedBuffer.AddrOfPinnedObject(), typeof(T));

            PinnedBuffer.Free();

            return Struct;
        }

        public static string ReadMagic(this Stream Stream, int Size)
        {
            byte[] Magic = new byte[Size];

            Stream.Read(Magic, 0, Size);

            Stream.Seek(-Size, SeekOrigin.Current);

            return Encoding.ASCII.GetString(Magic);
        }

        public static string ReadName(this Stream Stream, long Offset)
        {
            Stream.Seek(Offset, SeekOrigin.Begin);

            BinaryReader Reader = new BinaryReader(Stream);
            short        Size   = Reader.ReadInt16();
            byte[]       Magic  = new byte[Size];

            Stream.Read(Magic, 0, Size);

            return Encoding.ASCII.GetString(Magic);
        }
    }
}
