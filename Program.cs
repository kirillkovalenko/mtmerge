using System;
using System.Linq;

namespace MTMerge
{
    internal class Program
    {
        /*
        private void Test1()
        {
            foreach (var name in new[] { @"W:\mtmerge\MSG00001.bin", @"W:\mtmerge\MSG00002.bin" })
            {
                Console.WriteLine($"{name} 1111111111111111111111111111111111111111111111111111111");
                MessageResource resource = new MessageResource(name);

                foreach (var block in resource.MessageBlocks)
                {
                    Console.WriteLine($"0x{block.FirstMessageId:X}-0x{block.LastMessageId:X} => {block.Messages.Count}");
                    foreach (var kvp in block)
                    {
                        Console.WriteLine($"    0x{kvp.Id:X} => {kvp.Message}");
                    }
                }

                Console.WriteLine($"{name} 222222222222222222222222222222222222222222222222222222");
                MessageResource resource2 = new MessageResource(resource.MessageBlocks);

                foreach (var block in resource2.MessageBlocks)
                {
                    Console.WriteLine($"0x{block.FirstMessageId:X}-0x{block.LastMessageId:X} => {block.Messages.Count}");
                    foreach (var kvp in block)
                    {
                        Console.WriteLine($"    0x{kvp.Id:X} => {kvp.Message}");
                    }
                }
            }
        }

        private void Test2()
        {
            MessageResource resource1 = new MessageResource(@"W:\mtmerge\MSG00001.bin");
            Console.WriteLine($"resource1: {resource1.MessageBlocks.Count} blocks");

            MessageResource resource2 = new MessageResource(@"W:\mtmerge\MSG00002.bin");
            Console.WriteLine($"resource2: {resource2.MessageBlocks.Count} blocks");

            MessageResource resource = new MessageResource(resource1, resource2);
            Console.WriteLine($"combined: {resource.MessageBlocks.Count} blocks");

            foreach (var block in resource.MessageBlocks)
            {
                Console.WriteLine($"0x{block.FirstMessageId:X}-0x{block.LastMessageId:X} => {block.Messages.Count}");
                foreach (var kvp in block)
                {
                    Console.WriteLine($"    0x{kvp.Id:X} => {kvp.Message}");
                }
            }


        }

        private static void Test3()
        {
            {
                MessageResource resource1 = new MessageResource(@"W:\mtmerge\MSG00001.bin");
                resource1.Save(@"W:\mtmerge\MSG00001_1.bin", true);

                MessageResource resource2 = new MessageResource(resource1);
                resource2.Save(@"W:\mtmerge\MSG00001_2.bin", true);
            }

            {
                MessageResource resource1 = new MessageResource(@"W:\mtmerge\MSG00002.bin");
                resource1.Save(@"W:\mtmerge\MSG00002_1.bin", true);

                MessageResource resource2 = new MessageResource(resource1);
                resource2.Save(@"W:\mtmerge\MSG00002_2.bin", true);
            }
        }
        */

        private static Int32 Main(String[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("USAGE: mtmerge.exe outfile infile1 infile2 [... infileN]");
                return 1;
            }

            try
            {
                var input = args.Skip(1).Select(file => new MessageResource(file));
                var result = new MessageResource(input);
                result.Save(args[0], overrideExisting: false);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return -1;
            }

            return 0;
        }
    }
}