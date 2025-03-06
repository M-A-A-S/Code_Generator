// See https://aka.ms/new-console-template for more information
using DataBusiness;
using Microsoft.Data.SqlClient;

string databaseName = GetDatabaseSelection();
string outputPath = GetOutputPath();

CodeGenerator.Generate(databaseName, outputPath);

static string GetDatabaseSelection()
{
    string connectionString = "Server=.;Database=DB1;User Id=sa;Password=sa123456;Encrypt=False;TrustServerCertificate=True;Connection Timeout=30;";

    List<string> databases = DatabaseHelper.GetAllDatabasesNames(connectionString);

    if (databases == null || databases.Count == 0)
    {
        Console.WriteLine("No databases found or unable to fetch databases.");
        return string.Empty;
    }

    Console.Clear();
    Console.WriteLine("Available Databases:");

    for (int i = 0; i < databases.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {databases[i]}");
    }

    while (true)
    {
        Console.Write("\nEnter the number of the database to select: ");
        string input = Console.ReadLine();

        if (int.TryParse(input, out int choice) && choice > 0 && choice <= databases.Count)
        {
            return databases[choice - 1];
        }

        Console.WriteLine("❌ Invalid selection. Please enter a number from the list.");
    }
}

static string GetOutputPath()
{
    //string defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "GeneratedFiles");
    string defaultPath = @"D:\GeneratedClasses";

    while (true)
    {
        Console.Write($"Enter output path (or press Enter to use default: {defaultPath}): ");
        string outputPath = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(outputPath))
        {
            outputPath = defaultPath; // Use default path
        }

        if (!Path.IsPathRooted(outputPath))
        {
            Console.WriteLine("❌ Invalid path. Please enter a full absolute path.");
            continue;
        }

        try
        {
            Directory.CreateDirectory(outputPath); // Ensure the directory exists
            Console.WriteLine($"✅ Output path set to: {outputPath}");
            return outputPath;
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine("❌ Access denied. Try choosing a different location.");
        }
        catch (PathTooLongException)
        {
            Console.WriteLine("❌ The specified path is too long. Try a shorter path.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
    }
}


