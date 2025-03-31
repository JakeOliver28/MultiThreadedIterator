// See https://aka.ms/new-console-template for more information
using System.Threading;
using System.Collections.Concurrent;
using System.Text;


class Program
{
	private static async Task<string> ReadFile(string filePath)
	{
		var sb = new StringBuilder();

		using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous))
		{
			using (StreamReader reader = new StreamReader(fs))
			{
				string? line;
				while ((line = await reader.ReadLineAsync()) != null)
				{
					sb.AppendLine(line);
				}
			}
		}

		return sb.ToString();
	}

	private static async Task ProcessDirectory(string directoryPath, ConcurrentDictionary<string, string> output)
	{
		foreach (string file in Directory.EnumerateFiles(directoryPath))
		{
			output[file] = await ReadFile(file);
		}
	}

	private static void PrintOutput(ConcurrentDictionary<string, string> output, CancellationToken token)
	{
		var printedFiles = new ConcurrentBag<string>();

		while (!token.IsCancellationRequested || output.Count > printedFiles.Count)
		{
			foreach (var kvp in output)
			{
				if (!printedFiles.Contains(kvp.Key))
				{
					Console.WriteLine($"File: {kvp.Key}");
					Console.WriteLine(kvp.Value);
					printedFiles.Add(kvp.Key);
				} 
			}
			Thread.Sleep(500); // Reduce CPU usage by checking periodically
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

	private static async Task Main()
	{
		string[] directories = ValidateDirectoryInput();

		var output = new ConcurrentDictionary<string, string>();
		var tasks = new List<Task>();

		using (var cts = new CancellationTokenSource())
		{
			var printThread = new Thread(() => PrintOutput(output, cts.Token));
			printThread.Start();

			foreach (var directory in directories)
			{
				tasks.Add(ProcessDirectory(directory, output));
			}

			await Task.WhenAll(tasks);

			cts.Cancel();
			printThread.Join();
		}
	}

}