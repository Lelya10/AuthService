using System.Text;
using AuthService.BD;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AuthService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = Consts.Issuer,
                    ValidateAudience = true,
                    ValidAudience = Consts.Audience,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Consts.Key)),
                    ValidateIssuerSigningKey = true,
                };
            });

        var connection = builder.Configuration.GetConnectionString("ConnectionStringBeg") + 
                         Environment.GetEnvironmentVariable("HOSTNAME") + 
                         builder.Configuration.GetConnectionString("DefaultConnection") + 
                         "Username=" + Environment.GetEnvironmentVariable("USERNAME") + 
                         ";Password="+
                         Environment.GetEnvironmentVariable("PASSWORD");
        
// добавляем контекст ApplicationContext в качестве сервиса в приложение
        Console.WriteLine(connection);
        builder.Services.AddDbContext<BDManage>(options => options.UseNpgsql(connection));

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();
        
        app.MapControllers();

        app.Run();
    }
}