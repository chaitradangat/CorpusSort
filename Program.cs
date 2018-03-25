using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace CorpusSort
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("input folder:");
            var inputPath = Console.ReadLine();

            Console.WriteLine("output folder:");
            var outputPath = Console.ReadLine();


            //var textfiles = Directory.GetFiles(inputPath, "*.*", SearchOption.AllDirectories).Where(x=>x.ToLower().EndsWith(".txt"));

            //foreach (var textfile in textfiles)
            //{
            //    Console.Write(".");

            //    var combifile = outputPath + "\\" + "hugecsvdata.txt";

            //    File.AppendAllLines(combifile, File.ReadAllLines(textfile), Encoding.UTF8);
            //}



            //foreach (var textfile in textfiles)
            //{
            //    Console.Write(">");

            //    var combifile = outputPath + "\\" + "hugecsvdata.txt";

            //    File.AppendAllLines(combifile, File.ReadAllLines(textfile), Encoding.UTF8);
            //}

            SortCorpus(inputPath, outputPath);

            //SortLevel2(outputPath, ",");

            Console.WriteLine("..");

            Console.ReadLine();
        }

        static void SortCorpus(string inputPath,string outputPath)
        {
            var textFiles = 
            Directory.GetFiles(inputPath, "*.*", SearchOption.AllDirectories).Where(x => x.ToLower().EndsWith(".txt"));

            var sort1Path = outputPath + "\\" + "sort1";

            if (!Directory.Exists(sort1Path))
            {
                Directory.CreateDirectory(sort1Path);
            }

            OutPutWriter ow = new OutPutWriter(sort1Path, 1000);

            foreach (var textFile in textFiles)
            {
                int i = 0;

                using (FileStream fs = new FileStream(textFile,FileMode.Open))
                using (BufferedStream bfs = new BufferedStream(fs))
                using (StreamReader sr = new StreamReader(bfs))
                {
                    while (!sr.EndOfStream)
                    {
                        ++i;

                        ow.Add(sr.ReadLine());

                        if (i%100 == 0)
                        {
                            //Console.Clear();
                            Console.WriteLine(i.ToString());
                        }
                        
                    }
                }
            }

            ow.Flush(0);
        }

        static void SortLevel2(string outputPath,string separator)
        {
            var sort1Path = outputPath + "\\" + "sort1";

            var sort2Path = outputPath + "\\" + "sort2";

            var textFiles = Directory.GetFiles(sort1Path, "*.txt", SearchOption.TopDirectoryOnly);

            foreach (var textFile in textFiles)
            {
                var subdirname = Path.GetFileNameWithoutExtension(textFile);

                if (!Directory.Exists(sort2Path + "\\" + subdirname))
                {
                    Directory.CreateDirectory(sort2Path + "\\" + subdirname);
                }
            }

            int filecount = 0;

            foreach (var textFile in textFiles)
            {
                ++filecount;

                var subdirname = Path.GetFileNameWithoutExtension(textFile);

                var sort2SubPath = sort2Path + "\\" + subdirname;

                OutPutWriter ow = new OutPutWriter(sort2SubPath, 0);

                int cnt = 0;

                using (FileStream fs = new FileStream(textFile,FileMode.Open))
                using (BufferedStream bfs = new BufferedStream(fs))
                using (StreamReader sr = new StreamReader(bfs))
                {
                    while (!sr.EndOfStream)
                    {
                        ++cnt;
                        Console.WriteLine(cnt.ToString() + ":" + filecount.ToString() + "\\" + textFiles.Length);

                        var line = sr.ReadLine();

                        ow.AddLevel1(line, separator);
                    }
                }

                ow.FlushLevel1(0);
            }


            



        }

    }

    class OutPutWriter
    {
        List<List<string>> data {get;set;} 

        Dictionary<string, List<string>> level1data { get; set; }

        string outputPath { get; set; }

        public OutPutWriter(string outputPath,int maxSize)
        {
            this.outputPath = outputPath;

            data = new List<List<string>>();

            level1data = new Dictionary<string, List<string>>();

            int i = 0;

            while (i<maxSize)
            {
                data.Add(new List<string>());
                ++i;
            }

            data.ToString();
        }

        internal void Add(string line)
        {
            if (line.Length > data.Count || line.Length == 0)
            {
                return;
            }

            data[line.Length - 1].Add(line);

            if (data[line.Length-1].Count == 500000)
            {
                File.AppendAllLines(outputPath + "\\" + line.Length.ToString() + ".txt", data[line.Length - 1]);
                data[line.Length - 1].Clear();
                Flush(10000);
            }
        }

        internal void Flush(int Threshold)
        {
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].Count > Threshold)
                {
                    File.AppendAllLines(outputPath + "\\" + (i + 1).ToString() + ".txt", data[i]);
                    data[i].Clear();
                }
            }
        }

        internal void AddLevel1(string line,string separator)
        {
            var words = line.Split(new[] { separator }, StringSplitOptions.None);

            if (words.Length < 2)
            {
                return;
            }

            var word1 = words[0];

            if (!level1data.ContainsKey(word1))
            {
                level1data.Add(word1, new List<string>());
            }

            level1data[word1].Add(line);

            if (level1data[word1].Count == 10000)
            {
                File.AppendAllLines(outputPath + "\\" + word1 + ".txt", level1data[word1]);
                level1data[word1].Clear();
                FlushLevel1(10000);
            }


        }

        internal void FlushLevel1(int Threshold)
        {
            for (int i = 0; i < level1data.Count; i++)
            {
                var l1element = level1data.ElementAt(i);

                if (l1element.Value.Count > Threshold)
                {
                    File.AppendAllLines(outputPath + "\\" + l1element.Key + ".txt", l1element.Value);
                    l1element.Value.Clear();
                }
            }
        }



    }











}
