// See https://aka.ms/new-console-template for more information
using System.Threading;
using System.Collections.Concurrent;
using System.Text;


class Program
{
	private static void Main()
	{
		string[] directories = ValidateDirectoryInput();

		var output = new ConcurrentDictionary<string, string>();
		var fileQueue = new ConcurrentQueue<string>();
		var threads = new List<Thread>();
		var countdown = new CountdownEvent(directories.Length);

		using (var cts = new CancellationTokenSource())
		{
			var printThread = new Thread(() => PrintOutput(output, fileQueue, cts.Token, countdown));
			printThread.Start();

			foreach (var directory in directories)
			{
				var thread = new Thread(() =>
				{
					ProcessDirectory(directory, output, fileQueue);
					countdown.Signal();
				});
				thread.Start();
				threads.Add(thread);
			}

			foreach (var thread in threads)
			{
				thread.Join();
			}

			countdown.Wait();
			cts.Cancel();
			printThread.Join();
		}
	}

	private static string[] ValidateDirectoryInput()
	{
		string[] directories = Array.Empty<string>();
		bool isValidInput = false;
		Console.WriteLine("Enter directory paths to be iterated (separated by space or comma):");
		
		while (!isValidInput)
		{
			string input = Console.ReadLine() ?? string.Empty;
			directories = input.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
			isValidInput = directories.All(Directory.Exists);
			if (!isValidInput)
			{
				Console.WriteLine("One or more directories do not exist. Please try again.");
			}
		}

		return directories;
	}

	private static void ProcessDirectory(string directoryPath, ConcurrentDictionary<string, string> output, ConcurrentQueue<string> fileQueue)
	{
		foreach (string file in Directory.EnumerateFiles(directoryPath))
		{
			var content = File.ReadAllText(file);
			output[file] = content;
			fileQueue.Enqueue(file);
		}
	}

	private static void PrintOutput(ConcurrentDictionary<string, string> output, ConcurrentQueue<string> fileQueue, CancellationToken token, CountdownEvent countdown)
	{
		while (!token.IsCancellationRequested || !fileQueue.IsEmpty || countdown.CurrentCount > 0)
		{
			while (fileQueue.TryDequeue(out string? filePath))
			{
				Console.WriteLine($"File: {filePath}");
				Console.WriteLine(output[filePath]);
			}
			Thread.Sleep(200); // Reduce CPU usage by checking periodically
		}
	}
}