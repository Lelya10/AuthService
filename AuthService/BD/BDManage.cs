using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.BD;

public class BDManage : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    
    public BDManage(DbContextOptions<BDManage> options) : base(options)
    {
        Database.EnsureCreated(); 
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasData(
            new User { Id = Guid.NewGuid(), FirmId = 2910073, FirstName = "Kesha", LastName = "Ivanov", Email = "kesha2024@mail.ru", Phone = "+79865643425", Password = "testtest10"},
            new User { Id = Guid.NewGuid(), FirmId = 1154738, FirstName = "Vesta", LastName = "Sergeeva", Email = "vesta69@mail.ru", Phone = "+79765643679", Password = "qwerty123" },
            new User { Id = Guid.NewGuid(), FirmId = 8956373, FirstName = "Maksim", LastName = "Petrov", Email = "maksim89@mail.ru", Phone = "+79087654536", Password = "abcd3456"}
        );
    }

    public async Task<IResult> Login(LoginData data)
    {
        var success = false;
        foreach (var user in Users)
        {
            if (user.FirmId == data.FirmId && user.Password == data.Password)
            {
                success = true;
                break;
            }
        }

        if (!success)
        {
            return Results.Problem("Неправильный логин или пароль. Попробуйте снова.");
        }
        
        var claims = new List<Claim> {new Claim(ClaimTypes.Name, data.FirmId.ToString()) };
        var jwt = new JwtSecurityToken(
            issuer: Consts.Issuer,
            audience: Consts.Audience,
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)), // время действия 2 минуты
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Consts.Key)), SecurityAlgorithms.HmacSha256));
        
        var response = new
        {
            access_token = new JwtSecurityTokenHandler().WriteToken(jwt),
            username = data.FirmId
        };

        return Results.Json(response);
    }

    public async Task<IResult> AddUser(NewUser newUser)
    {
        var userCon = Users.FirstOrDefault(u => u.FirmId == newUser.FirmId);
        if (userCon != null)
        {
            return Results.Problem("Пользователь с таким номером фирмы уже существует");
        }
        var id = Guid.NewGuid();
        
        var user = User.CreateUser(id, newUser);
        try
        {
            await Users.AddAsync(user);
            await SaveChangesAsync();
            return Results.Json("Пользователь добавлен с id=" + id);
        }
        catch (Exception e)
        {
            return Results.Problem(e.ToString());
        }
    }
}