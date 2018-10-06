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

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Zstandard.Net;

namespace BEAExtractor
{
    class Program
    {
        static void Main(string[] Args)
        {
            Console.WriteLine("\n BEA Extractor v0.1 - Ac_K:\n" +
                              "----------------------------\n");

            if (Args.Length == 0 || Args.Length > 2)
            {
                Console.WriteLine("BEAExtractor.exe archive_file.bea out_directory (optionnal - list files if not present)");
            }
            else
            {
                using (FileStream Stream = new FileStream(Args[0], FileMode.Open, FileAccess.Read))
                {
                    if (Stream.ReadMagic(0x4) == "SCNE")
                    {
                        BinaryReader Reader      = new BinaryReader(Stream);
                        BEAHeader    Header      = Stream.ReadStruct<BEAHeader>();
                        string       ArchiveName = Stream.ReadName(Header.ArchiveNameOffset);

                        Console.WriteLine("Header Info:\n" +
                                          $"\tReserved0: 0x{Header.Reserved0.ToString("X2")}\n" +
                                          $"\tUnknown0: 0x{Header.Unknown0.ToString("X2")}\n" +
                                          $"\tByteOrderMark: 0x{Header.ByteOrderMark.ToString("X2")}\n" +
                                          $"\tSectionsNumber: {Header.SectionsNumber}\n" +
                                          $"\tReserved1: 0x{Header.Reserved1.ToString("X2")}\n" +
                                          $"\tUnknown1: 0x{Header.Unknown1.ToString("X2")}\n" +
                                          $"\tFirstASSTSectionOffset0: 0x{Header.FirstASSTSectionOffset0.ToString("X2")}\n" +
                                          $"\t_RLTSectionOffset: 0x{Header._RLTSectionOffset.ToString("X2")}\n" +
                                          $"\tSectionsSize: 0x{Header.SectionsSize.ToString("X2")}\n" +
                                          $"\tFilesNumber: {Header.FilesNumber}\n" +
                                          $"\tASSTInfoSectionOffset: 0x{Header.ASSTInfoSectionOffset.ToString("X2")}\n" +
                                          $"\t_DICSectionOffset: 0x{Header._DICSectionOffset.ToString("X2")}\n" +
                                          $"\tUnknown2: 0x{Header.Unknown2.ToString("X2")}\n" +
                                          $"\tArchiveNameOffset: 0x{Header.ArchiveNameOffset.ToString("X2")}\n" +
                                          $"\tArchiveName: {ArchiveName}\n\n");

                        List<ASSTSection> FilesInfo = new List<ASSTSection>();

                        for (int i = 0; i < Header.FilesNumber; i++)
                        {
                            Stream.Seek(Header.ASSTInfoSectionOffset + sizeof(long) * i, SeekOrigin.Begin);
                            Stream.Seek(Reader.ReadInt64(), SeekOrigin.Begin);

                            if (Stream.ReadMagic(0x4) == "ASST")
                            {
                                ASSTSection FileInfo = Stream.ReadStruct<ASSTSection>();

                                FilesInfo.Add(FileInfo);
                            }
                            else throw new Exception("ASST section not found!");
                        }

                        Console.WriteLine("Files Informations:");

                        foreach (ASSTSection FileInfo in FilesInfo)
                        {
                            string Filename = Stream.ReadName(FileInfo.FilenameOffset).Replace("/", "\\");

                            Console.WriteLine($"\t{Filename}:\n\n" +
                                              $"\t\tFileOffset: 0x{FileInfo.FileOffset.ToString("X2")}\n" +
                                              $"\t\tFilenameOffset: 0x{FileInfo.FilenameOffset.ToString("X2")}\n" +
                                              $"\t\tCompressionType: 0x{FileInfo.CompressionType}\n" +
                                              $"\t\tCompressedSize: 0x{FileInfo.CompressedSize.ToString("X2")}\n" +
                                              $"\t\tDecompressedSize: 0x{FileInfo.DecompressedSize.ToString("X2")}\n");

                            if (Args.Length > 1 && Args[1] != "")
                            {
                                Filename             = Path.Combine(Args[1], Filename);
                                string DirectoryName = Path.GetDirectoryName(Filename);

                                Stream.Seek(FileInfo.FileOffset, SeekOrigin.Begin);

                                byte[] FileData = Reader.ReadBytes(FileInfo.CompressedSize);

                                if (FileInfo.CompressionType == CompressionType.Zstandard)
                                {
                                    using (MemoryStream    MemStream  = new MemoryStream(FileData))
                                    using (ZstandardStream CompStream = new ZstandardStream(MemStream, CompressionMode.Decompress))
                                    using (MemoryStream    TempStream = new MemoryStream())
                                    {
                                        CompStream.CopyTo(TempStream);
                                        FileData = TempStream.ToArray();

                                        Console.Write("\t\tDecompressed! ");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"\tUnknown compressed type: 0x{FileInfo.CompressionType.ToString("X2")}! File save as Raw.");
                                }

                                try
                                {
                                    Directory.CreateDirectory(DirectoryName);
                                }
                                catch (IOException Ex)
                                {
                                    if (Ex.HResult == -2147024713) //Rename when folder name is same as file name.
                                    {
                                        DirectoryName = Path.Combine(Path.GetDirectoryName(DirectoryName), Path.GetFileName(DirectoryName) + "_dir");
                                        Filename      = Path.Combine(DirectoryName, Path.GetFileName(Filename));

                                        Directory.CreateDirectory(DirectoryName);
                                    }
                                }

                                File.WriteAllBytes(Filename, FileData);

                                Console.WriteLine("Extracted!\n");
                            }
                        }

                        Console.WriteLine("Done.");
                    }
                    else throw new Exception("BEA Archive file not found!");
                }
            }
        }
    }
}
