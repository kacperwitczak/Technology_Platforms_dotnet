using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace file_manager.Extensions
{
    public static class FileSystemInfoExtensions
    {
        public static string GetAttributesDOS(this FileSystemInfo fileSystemInfo)
        {
            string DOS = "";

            FileAttributes fileAttributes = fileSystemInfo.Attributes;

            DOS += ((fileAttributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) ? 'r' : '-';
            DOS += ((fileAttributes & FileAttributes.Archive) == FileAttributes.Archive) ? 'a' : '-';
            DOS += ((fileAttributes & FileAttributes.Hidden) == FileAttributes.Hidden) ? 'h' : '-';
            DOS += ((fileAttributes & FileAttributes.System) == FileAttributes.System) ? 's' : '-';

            return DOS;
        }
    }
}
