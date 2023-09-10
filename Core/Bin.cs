using Helper;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;


namespace Bayonetta3_bin_tool.Core
{

    public class StringBlock
    {
        public const int Size = 0x10;
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class StringInfo
        {
            public uint hash { get; set; }
            public int String_pos { get; set; }
            public int char_length { get; set; }
            public int byte_length { get; set; }
        };
        public StringInfo Info { get; set; }
        public string HashStr { get; set; }
        public string Str { get; set; }

        public StringBlock(IStream stream)
        {
            Info = stream.Get<StringInfo>();
        }

        public void ReadString(IStream stream)
        {
            Str = stream.GetEncodeString(Info.byte_length);
        }

        public void WriteString(IStream stream, long StartPosition)
        {
            Info.String_pos = (int)(stream.Position - StartPosition);
            var length = stream.SetEncodeString(Str);
            Info.byte_length = length;
            Info.char_length = length / 2;
        }

        public void WriteInfo(IStream stream)
        {
            stream.SetStructureValus(Info);
        }
    }

    public class Bin : List<StringBlock>
    {
        IStream Stream;
        public long TextStartBlock;
        public int TextBlockSize;
        public int Text_Count;

        public Bin(string FilePath)
        {
            Stream = new MStream(FilePath);
            load();

        }


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Header
        {
            public uint magic;
            public int u1;
            public int u2;
            public int hash_count;
            public int hash_data_pos;
            public int u3;
            public int string_data_pos;
        }

        public Dictionary<uint, StringBlock> NamesBlock = new Dictionary<uint, StringBlock>();


        void load()
        {
            var header = Stream.Get<Header>();
            if (header.magic != 0x445450)
                throw new Exception("Invalid file:!= PTD\0");

            Stream.Seek(header.hash_data_pos);
            for (int i = 0; i < header.hash_count; i++)
            {
                var block = new StringBlock(Stream);
                NamesBlock.Add(block.Info.hash, block);
            }

            foreach (var value in NamesBlock.Values)
                value.ReadString(Stream);

            if (NamesBlock[Stream.GetUIntValue()].Str != "archive_MES")
                throw new Exception("Invalid file:!= \"archive_MES\"");


            bool has_groupid = Stream.GetIntValue() == 1;
            Stream.Skip(12);//unko

            if (has_groupid)
            {
                if (NamesBlock[Stream.GetUIntValue()].Str != "groupid")
                    throw new Exception("Invalid file:!= \"groupid\"");

                int groupid_count = Stream.GetIntValue();

                Stream.Skip(4);//unko
                Stream.Skip(4 * groupid_count);//groupids
            }


            if (NamesBlock[Stream.GetUIntValue()].Str != "Text")
                throw new Exception("Invalid file:!= \"Text\"");
            Text_Count = Stream.GetIntValue();
            Stream.Skip(4);//unko

            if (NamesBlock[Stream.GetUIntValue()].Str != "CharName")
                throw new Exception("Invalid file:!= \"CharName\"");
            Stream.Skip(4);//Text_Count?
            TextStartBlock = Stream.Position;

            TextBlockSize = Stream.GetIntValue();//-12 

            for (int i = 0; i < Text_Count; i++)
            {
                var block = new StringBlock(Stream);
                block.HashStr = NamesBlock[block.Info.hash].Str;
                Add(block);
            }

            foreach (var value in this)
                value.ReadString(Stream);
        }

        public string[] GetStrings()
        {
            var strings = new string[Count];
            for (int i = 0; i < Count; i++)
            {
                strings[i] = this[i].HashStr + "=" + this[i].Str;
            }
            return strings;
        }

        public void UpdateStrings(string[] strings)
        {
            int i = 0;
            foreach (var line in strings)
            {
                var cells = line.Split(new[] { '=' }, 2);
                this[i++].Str = cells[1];
            }
        }

        public void Save(string filePath)
        {

            var InfoBlock = new MStream();
            InfoBlock.Skip(4);//TextBlockSize + 8
            InfoBlock.Skip(Text_Count * StringBlock.Size);
            int Start = 4;
            foreach (var item in this)
            {
                item.WriteString(InfoBlock, Start);
                Start += StringBlock.Size;
            }
            InfoBlock.Seek(0);

            InfoBlock.SetIntValue((int)(InfoBlock.Length + 8));
            foreach (var item in this)
            {
                item.WriteInfo(InfoBlock);
            }

            Stream.Seek(TextStartBlock);
            Stream.DeleteBytes(TextBlockSize - 8);
            TextBlockSize = (int)(InfoBlock.Length + 8);
            Stream.InsertBytes(InfoBlock.ToArray());
            InfoBlock.Close();

            Stream.WriteFile(filePath);
        }

    }
}
