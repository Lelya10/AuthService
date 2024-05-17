using AuthService.BD;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

public class RegistrationController : ControllerBase
{
    private readonly ILogger<RegistrationController> _logger;
    private readonly BDManage _bd;

    public RegistrationController(ILogger<RegistrationController> logger, BDManage bd)
    {
        _logger = logger;
        _bd = bd;
    }
    
    [HttpPost("/user/login")]
    public async Task<IResult> Login(LoginData data)
    {
        try
        {
            var res = await _bd.Login(data);
            return  res;
        }
        catch (Exception e)
        {
            _logger.LogError("Ошибка при выполнении запроса: " + e);
            return Results.StatusCode(500);
        }
    }
    
    [HttpPost("/user/registration")]
    public async Task<IResult> UserRegistration(NewUser user)
    {
        try
        {
            var res =  await _bd.AddUser(user);
            return  res;
        }
        catch (Exception e)
        {
            _logger.LogError("Ошибка при выполнении запроса: " + e);
            return Results.StatusCode(500);
        }
    }
}