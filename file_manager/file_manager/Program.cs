using file_manager.Extensions;
using file_manager.Helpers;
using file_manager.Managers;

namespace file_manager
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //BinaryFormatter serialization is obsolete and should not be used. See https://aka.ms/binaryformatter for more information.
            AppContext.SetSwitch("System.Runtime.Serialization.EnableUnsafeBinaryFormatterSerialization", true);
            string path = args[0];

            var directoryContent = FileManager.GetDirectoryContent(path);
            foreach (var item in directoryContent)
            {
                Console.WriteLine(item);
            }

            FileManager.DisplayDirectoryContentRecursively(path);

            DirectoryInfo dirInfo = new DirectoryInfo(path);
            DateTime oldestDate = dirInfo.GetOldestElementDate();

            Console.WriteLine(oldestDate.ToString());

            foreach (var item in directoryContent)
            {
                Console.WriteLine(path + item);
                FileSystemInfo fileSystemInfo = new FileInfo(path + item);
                Console.WriteLine(fileSystemInfo.GetAttributesDOS());
            }

            SortedDictionary<string, long> directoryWithSize = FileManager.GetDirectoryContentWithSize(path,new StringLengthComparer());

            foreach (var item in directoryWithSize)
            {
                Console.WriteLine(item.Key + ": " + item.Value);
            }


            string serializationFileName = "dict.bin";

            string currentDirectory = Directory.GetCurrentDirectory();
            string serializationFilePath = Path.Combine(currentDirectory, serializationFileName);

            FileManager.SerializeDirectory(directoryWithSize, serializationFilePath);

            SortedDictionary<string, long> deserializedDirectory = FileManager.DeserializeDirectory(serializationFilePath);

            Console.WriteLine("Deserialized:");
            foreach (var item in deserializedDirectory)
            {
                Console.WriteLine(item.Key + ": " + item.Value);

            }

        }
    }
}
