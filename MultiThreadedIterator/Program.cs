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

	private static async Task<ConcurrentDictionary<string, string>> ProcessDirectory(string directoryPath)
	{
		var directoryContent = new ConcurrentDictionary<string, string>();

		foreach (string file in Directory.EnumerateFiles(directoryPath))
		{
			directoryContent[file] = await ReadFile(file);
		}

		return directoryContent;
	}

	private static void Main()
	{

		Console.WriteLine("Enter directory paths to be iterated:");

		// Allow input
		string input = Console.ReadLine() ?? string.Empty;

		// Process input
		string[] directories = input.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

		// Create a ConcurrentQueue to hold the directories
		var directoryQueue = new ConcurrentQueue<string>(directories);

		// Create output data structure to print contents
		var output = new ConcurrentDictionary<string, string>();

		// Iterate through the directories and add content to the data structure
		Parallel.ForEach(directoryQueue, (directory) =>
		{
			// Simulate processing of each directory
			Console.WriteLine($"Processing directory: {directory}");
			var directoryContent = ProcessDirectory(directory).Result;
			foreach (var kvp in directoryContent)
			{
				output[kvp.Key] = kvp.Value;
			}
		});

		// Print content of directories
		foreach (var kvp in output)
		{
			Console.WriteLine($"File: {kvp.Key}");
			Console.WriteLine(kvp.Value);
		}
	}

}