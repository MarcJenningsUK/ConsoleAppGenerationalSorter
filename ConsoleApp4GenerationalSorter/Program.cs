using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleApp4GenerationalSorter
{
    class Program
    {
        static void Main(string[] args)
        {
            Worker.SortFolder(@"L:\nodered", "*.json");
            Worker.SortFolder(@"L:\HomeAssistant", "*.tgz");
            Console.WriteLine("Done");
        }
    }

    internal static class Worker
    {
        static int dailyFileCount = 14;
        static int weeklyFileCount = 12;
        static int monthlyFileCount = 12;

        internal static void SortFolder(string path, string searchPattern)
        {
            var weeklyFolder = Path.Combine(path, "Weekly");
            var monthlyFolder = Path.Combine(path, "Monthly");

            var allFiles = Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories);

            var fileInfos = new List<FileInfo>();
            
            foreach(var file in allFiles)
            {
                var fi = new FileInfo(file);
                fileInfos.Add(fi);
                if (fi.CreationTimeUtc.DayOfWeek == DayOfWeek.Sunday)
                {
                    File.Copy(file, Path.Combine(weeklyFolder, fi.Name), true);
                }
                if (fi.CreationTimeUtc.Day == 1)
                {
                    File.Copy(file, Path.Combine(monthlyFolder, fi.Name), true);
                }

                Console.WriteLine(fi.Name + "\t\t" + fi.CreationTimeUtc.ToString());
            }

            foreach (var delDaily in fileInfos.Where(f=> f.Directory.FullName == path).OrderByDescending(f => f.CreationTimeUtc).Skip(dailyFileCount))
            {
                Console.WriteLine("delete " + delDaily.Name);
                File.Delete(delDaily.FullName);
            }

            allFiles = Directory.GetFiles(weeklyFolder, searchPattern, SearchOption.TopDirectoryOnly);
            foreach (var delWeekly in fileInfos.Where(f => f.Directory.FullName == weeklyFolder).OrderByDescending(f => f.CreationTimeUtc).Skip(weeklyFileCount))
            {
                Console.WriteLine("delete " + delWeekly.Name);
                File.Delete(delWeekly.FullName);
            }

            allFiles = Directory.GetFiles(weeklyFolder, searchPattern, SearchOption.TopDirectoryOnly);
            foreach (var delMonthly in fileInfos.Where(f => f.Directory.FullName == monthlyFolder).OrderByDescending(f => f.CreationTimeUtc).Skip(monthlyFileCount))
            {
                Console.WriteLine("delete " + delMonthly.Name);
                File.Delete(delMonthly.FullName);
            }
        }
    }
}
