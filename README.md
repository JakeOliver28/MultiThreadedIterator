# Multi-Threaded Directory Reader

## Overview
This program takes multiple directory paths as input, iterates through the directories to read file contents, and prints the contents using a multi-threaded approach. Each directory is processed on a dedicated thread, and a separate thread is responsible for printing the output.

## Installation & Usage
### Prerequisites
- .NET SDK installed on your machine

### Steps to Run
1. Clone the repository:
   ```sh
   git clone https://github.com/JakeOliver28/MultiThreadedIterator.git
   cd MultiThreadedIterator
   ```
2. Build and run the application:
   ```sh
   dotnet run
   ```
3. Enter directory paths separated by spaces or commas when prompted.
4. The program will read and display file contents from each directory.

## Code Structure
- **`Program.cs`**: Contains the main logic for reading directories, processing files, and printing output.
- **Methods:**
  - `Main()`: Initializes threads, manages their execution, and coordinates processing.
  - `ValidateDirectoryInput()`: Prompts the user for directory input and validates existence.
  - `ProcessDirectory()`: Reads files from a directory and stores their contents in `ConcurrentDictionary`.
  - `PrintOutput()`: Prints file contents using a dedicated thread.

## Example Output
```
Enter directory paths to be iterated (separated by space or comma):
C:\Users\Documents, C:\Users\Desktop
File: C:\Users\Documents\file1.txt
Hello, this is file1.
File: C:\Users\Documents\file2.txt
This is another file.
```


