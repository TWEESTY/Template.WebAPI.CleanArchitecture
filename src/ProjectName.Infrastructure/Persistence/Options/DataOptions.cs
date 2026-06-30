namespace ProjectName.Infrastructure.Persistence.Options;

/// <summary>
/// Represents the options for configuring data persistence, including database relative path and SQLite file name.
/// </summary>
public class DataOptions
{
    public static string Key => "Data";

    public string DatabaseRelativePathFromRepositoryRoot { get; set; } = "database";

    public string SqliteFileName { get; set; } = "projectname.db";
}
