public class User
{
    public string Id {get; set;} = string.Empty;
    public string Email {get; set;} = string.Empty;

    public User(string id, string email)
    {
        Id = id;
        Email = email;
    }
}