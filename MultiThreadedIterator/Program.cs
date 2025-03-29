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

	private static async Task Main()
	{

		Console.WriteLine("Enter directory paths to be iterated:");
		string input = Console.ReadLine() ?? string.Empty;

		string[] directories = input.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

		// Create output data structure to print contents
		var output = new ConcurrentDictionary<string, string>();
		var tasks = new List<Task>();

		// Iterate through the directories and add content to the data structure
		foreach (var directory in directories)
		{
			tasks.Add(ProcessDirectory(directory, output));
		}

		await Task.WhenAll(tasks);

		// Print content of directories
		foreach (var kvp in output)
		{
			Console.WriteLine($"File: {kvp.Key}");
			Console.WriteLine(kvp.Value);
		}
	}

}