namespace AuthService;

public class User
{
    public Guid Id { get; set; }
    public int FirmId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Password { get; set; }
    
    
    public static User CreateUser(Guid id, NewUser newUser)
    {
        return new User
        {
            Id = id,
            FirmId = newUser.FirmId,
            FirstName = newUser.FirstName,
            LastName = newUser.LastName,
            Email = newUser.Email,
            Phone = newUser.Phone,
            Password = newUser.Password
        };
    }
}