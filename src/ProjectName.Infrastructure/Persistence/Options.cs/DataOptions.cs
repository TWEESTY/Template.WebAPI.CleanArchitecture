namespace ProjectName.Infrastructure.Persistence.Options;

public class DataOptions
{
    public static string Key => "Data";

    public string DatabaseRelativePathFromRepositoryRoot { get; set; } = "database";

    public string SqliteFileName { get; set; } = "projectname.db";
}
