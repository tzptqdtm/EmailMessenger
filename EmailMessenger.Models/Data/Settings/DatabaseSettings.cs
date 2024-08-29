namespace EmailMessenger.Models.Data.Settings;

public class DatabaseSettings
{
    public string ConnectionString
    {
        get; set;
    } = null!;

    public string DatabaseSchema
    {
        get; set;
    } = null!;
}