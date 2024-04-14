using file_manager.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace file_manager.Managers
{
    public static class FileManager
    {
        public static List<string> GetDirectoryContent(string path)
        {
            string[] directories = Directory.GetDirectories(path);
            string[] files = Directory.GetFiles(path);

            var result = new List<string>();

            foreach (var d in directories)
            {
                result.Add(Path.GetFileName(d));
            }

            foreach (var f in files)
            {
                result.Add(Path.GetFileName(f));
            }

            return result;
        }

        public static void DisplayDirectoryContentRecursively(string path, int depth = 0)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append('-', depth);

            string indentation = sb.ToString();

            string[] files = Directory.GetFiles(path);

            foreach (var f in files)
            {
                FileInfo fileInfo = new FileInfo(f);

                long size = fileInfo.Length;

                string fileDOS = fileInfo.GetAttributesDOS();

                Console.WriteLine(indentation + fileInfo.Name + " " + size.ToString() + " " + fileDOS);
            }

            string[] directories = Directory.GetDirectories(path);

            foreach (var d in directories)
            {
                int fileCount = Directory.GetFiles(d).Length;
                int directoryCount = Directory.GetDirectories(path).Length;

                long size = fileCount + directoryCount;

                FileInfo dirInfo = new FileInfo(d);

                string DOSAttr = dirInfo.GetAttributesDOS();

                Console.WriteLine(indentation + dirInfo.Name + " " + size.ToString() + " " + DOSAttr);
                DisplayDirectoryContentRecursively(d, depth + 1);
            }
        }

        public static SortedDictionary<string, long> GetDirectoryContentWithSize(string path, IComparer<string> comparer)
        {
            SortedDictionary<string, long> result = new SortedDictionary<string, long>(comparer);

            string[] files = Directory.GetFiles(path);
            foreach (var f in files)
            {
                FileInfo fileInfo = new FileInfo(f);
                long size = fileInfo.Length;

                result.Add(f, size);
            }

            string[] directories = Directory.GetDirectories(path);
            foreach (var d in directories)
            {
                int fileCount = Directory.GetFiles(d).Length;
                int directoryCount = Directory.GetDirectories(path).Length;

                long size = fileCount + directoryCount;

                result.Add(d, size);
            }

            return result;
        }

        public static void SerializeDirectory(IDictionary<string, long> dict, string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                var bf = new BinaryFormatter();

                bf.Serialize(fs, dict);
            }
        }


        public static SortedDictionary<string, long> DeserializeDirectory(string path)
        {
            SortedDictionary<string, long> result;

            using (FileStream fs = new FileStream(path, FileMode.Open)) {
                var bf = new BinaryFormatter();

                result = (SortedDictionary<string, long>)bf.Deserialize(fs);
            };

            return result;
        }

    }
}
