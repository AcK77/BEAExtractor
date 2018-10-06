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

using System.Runtime.InteropServices;

namespace BEAExtractor
{
    enum CompressionType : short
    {
        Zstandard = 2
    }

    [StructLayout(LayoutKind.Sequential)]
    struct BEAHeader
    {
        public int   Magic;
        public int   Reserved0;
        public int   Unknown0;
        public short ByteOrderMark;
        public short SectionsNumber;
        public int   Reserved1;
        public short Unknown1;
        public short FirstASSTSectionOffset0;
        public int   _RLTSectionOffset;
        public int   SectionsSize;
        public long  FilesNumber;
        public long  ASSTInfoSectionOffset;
        public long  _DICSectionOffset;
        public long  Unknown2;
        public long  ArchiveNameOffset;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct ASSTSection
    {
        public int             Magic;
        public int             SectionSize0;
        public int             SectionSize1;
        public int             Reserved0;
        public CompressionType CompressionType;
        public short           Alignment;
        public int             CompressedSize;
        public int             DecompressedSize;
        public int             Reserved1;
        public long            FileOffset;
        public long            FilenameOffset;
    }
}
