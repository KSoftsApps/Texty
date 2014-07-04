﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Texty
{
    class History
    {
        [Serializable()]
        class SerializedContents : ISerializable
        {
            internal const int BUFFER_SIZE = 10 ;
            internal string[] names { get; set; }

            public SerializedContents(List<string> from)
            {
                for (int i = 0; i < BUFFER_SIZE || i < from.Count; ++i)
                    names[i] = from[i];
            }

            public SerializedContents(SerializationInfo info, StreamingContext ctx)
            {
                for (int i = 0; i < BUFFER_SIZE; ++i)
                {
                    names[i] = (string)info.GetValue("name" + i, typeof(string));
                }
            }

            public SerializedContents()
            {
                names = new string[BUFFER_SIZE];
            }

            public void GetObjectData(SerializationInfo info, StreamingContext ctx)
            {
                for (int i = 0; i < BUFFER_SIZE; ++i)
                {
                    info.AddValue("name" + i, names[i]);
                }
            }

        }

        static SerializedContents contents = new SerializedContents();
        static int added = 0;

        public static List<string> FetchFileList()
        {
            List<string> ret = new List<string>();
            try
            {
                Stream file = File.Open(Program.HISTORY_FILE, FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();
                contents = (SerializedContents)bf.Deserialize(file);
                for (int i = 0; i < SerializedContents.BUFFER_SIZE; ++i)
                    ret.Add(contents.names[i]);
            }
            catch (Exception exp)
            {
            }
            return ret;
        }

        public static void SaveHistory()
        {
            if (File.Exists(Program.HISTORY_FILE))
                File.WriteAllText(Program.HISTORY_FILE, "");
            try
            {
                Stream file = File.Open(Program.HISTORY_FILE, FileMode.OpenOrCreate);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(file, contents);
                file.Close();
            }
            catch (Exception exp)
            {
            }
        }

        public static void Add(string item)
        {
            if (added < SerializedContents.BUFFER_SIZE)
            {
                contents.names[added] = item;
                added += 1;
            }
        }

    }
}
