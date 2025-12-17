namespace SmartHouse.Models;

public class User
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Member;
    public List<string> AllowedDomains { get; set; } = new();
}

public enum UserRole
{
    Owner,
    Admin,
    Member,
    Guest
}
