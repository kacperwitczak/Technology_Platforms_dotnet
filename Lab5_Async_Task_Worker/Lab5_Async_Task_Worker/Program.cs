using System.ComponentModel;
using System.Threading;
using System;
using System.Numerics;
using System.Windows.Forms;
using System.IO.Compression;

namespace Lab5_BackgroundWorker_Task_Async
{


    public class Program
    {
        private static string? selectedFolderPath = null;

        public static int Numerator(int n, int k)
        {
            Console.WriteLine("Numerator waits 1s");
            Task.Delay(1000).Wait();
            int result = 1;
            for (int i = n; i >= n - k + 1; i--)
            {
                result *= i;
            }

            return result;
        }

        public static int Denominator(int k)
        {
            Console.WriteLine("Denominator waits 1s");
            Task.Delay(1000).Wait();
            int result = 1;
            for (int i = 1; i <= k; i++)
            {
                result *= i;
            }

            return result;
        }

        public static async Task<int> NumeratorAsync(int n, int k)
        {
            Console.WriteLine("NumeratorAsync waits 1s");
            await Task.Delay(1000);
            int result = 1;
            for (int i = n; i >= n - k + 1; i--)
            {
                result *= i;
            }

            return result;
        }

        public static async Task<int> DenominatorAsync(int k)
        {
            Console.WriteLine("DenominatorAsync waits 1s");
            await Task.Delay(1000);
            int result = 1;
            for (int i = 1; i <= k; i++)
            {
                result *= i;
            }

            return result;
        }

        [STAThread]
        static async Task Main(string[] args)
        {
            int n = 10;
            int k = 8;

            Task<int> numerator_task = Task.Run(() => Numerator(n, k));
            Task<int> denominator_task = Task.Run(() => Denominator(k));

            await Task.WhenAll(numerator_task, denominator_task);

            int result_task = numerator_task.Result / denominator_task.Result;

            Console.WriteLine("Result Task: " + result_task);

            Func<int, int, Task<int>> numerator_delegate = delegate (int n, int k)
            {
                return Task.Run(() => Numerator(n, k));
            };

            Func<int, Task<int>> denominator_delegate = delegate (int k)
            {
                return Task.Run(() => Denominator(k));
            };

            Task<int> numerator_del = numerator_delegate(n, k);
            Task<int> denominator_del = denominator_delegate(k);

            int result_delegate = await numerator_del / await denominator_del;

            Console.WriteLine("Result Delegate: " + result_delegate);

            int numerator_async = await NumeratorAsync(n, k);
            int denominator_async = await DenominatorAsync(k);


            int result = numerator_async / denominator_async;

            Console.WriteLine("Result Async: " + result);


            var worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += (sender, e) =>
            {
                int n = (int)e.Argument;

                if (n == 0)
                {
                    e.Result = 0;
                    return;
                }

                if (n == 1)
                {
                    e.Result = 1;
                    return;
                }

                BigInteger a = 0;
                BigInteger b = 1;

                for (int i = 2; i <= n; i++)
                {
                    BigInteger temp = a;
                    a = b;
                    b = temp + b;

                    Thread.Sleep(5);

                    worker.ReportProgress((int)((double)i / n * 100));
                }

                Thread.Sleep(1000);
                e.Result = b;
            };

            worker.ProgressChanged += (sender, e) =>
            {
                int progressBarLength = 50;
                int progress = e.ProgressPercentage;
                int completedBlocks = progressBarLength * progress / 100;

                //\r to carriage return z ako :)
                Console.Write("\r[");

                for (int i = 0; i < completedBlocks; i++)
                {
                    Console.Write("#");
                }

                for (int i = completedBlocks; i < progressBarLength; i++)
                {
                    Console.Write(" ");
                }

                Console.Write($"] {progress}%");

                Console.SetCursorPosition(0, Console.CursorTop);
            };

            worker.RunWorkerCompleted += (sender, e) =>
            {
                Console.SetCursorPosition(0, Console.CursorTop + 1);
                Console.WriteLine("Result: " + e.Result);
            };


            worker.RunWorkerAsync(100);

            Console.ReadLine();

            Thread dialogThread = new Thread(() =>
            {
                using (var dialog = new FolderBrowserDialog())
                {
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        selectedFolderPath = Path.GetFullPath(dialog.SelectedPath);
                    }
                }
            });
            dialogThread.SetApartmentState(ApartmentState.STA);
            dialogThread.Start();

            dialogThread.Join();

            if (selectedFolderPath != null)
            {
                Compress(selectedFolderPath);
                Console.ReadLine();
                Decompress(selectedFolderPath);
            }

        }

        private static void Compress(string directoryPath)
        {
            string[] files = Directory.GetFiles(directoryPath);

            var barrierCompression = new CountdownEvent(files.Length);

            files.AsParallel().ForAll(file =>
            {
                try
                {
                    using (FileStream originalFileStream = new FileStream(file, FileMode.Open))
                    {
                        using (FileStream compressedFileStream = File.Create(file + ".gz"))
                        {
                            using (GZipStream compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress))
                            {
                                originalFileStream.CopyTo(compressionStream);
                            }
                        }
                    }
                    File.Delete(file);
                }
                finally
                {
                    barrierCompression.Signal();
                }
            });

            barrierCompression.Wait();

            Console.WriteLine("Files compressed");
        }

        private static void Decompress(string directoryPath)
        {
            string[] files = Directory.GetFiles(directoryPath);

            var barrierDecompression = new CountdownEvent(files.Length);

            files.AsParallel().ForAll(file =>
            {
                try
                {
                    using (FileStream originalFileStream = new FileStream(file, FileMode.Open))
                    {
                        using (FileStream decompressedFileStream = File.Create(file.Replace(".gz", "")))
                        {
                            using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                            {
                                decompressionStream.CopyTo(decompressedFileStream);
                            }
                        }
                    }
                    File.Delete(file);
                }
                finally
                {
                    barrierDecompression.Signal();
                }
            });

            barrierDecompression.Wait();

            Console.WriteLine("Files decompressed");
        }
    }
}
