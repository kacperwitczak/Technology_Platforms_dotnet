using System;
using System.IO;

namespace file_manager.Extensions
{
    public static class DirectoryInfoExtensions
    {
        public static DateTime GetOldestElementDate(this DirectoryInfo directory)
        {
            DateTime oldest = DateTime.Now;

            if (directory.CreationTime < oldest)
            {
                oldest = directory.CreationTime;
            }

            foreach (var file in directory.GetFiles()) 
            {
                if (file.CreationTime < oldest)
                {
                    oldest = file.CreationTime;
                }
            }

            foreach (var dir in directory.GetDirectories())
            {
                DateTime d = dir.GetOldestElementDate();
                if (d < oldest)
                {
                    oldest = d;
                }
            }

            return oldest;
        }

    }
}
