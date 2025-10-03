using ChefServe.Core.Models;
using ChefServe.Core.DTOs;
using ChefServe.Core.Interfaces;
using ChefServe.Core.Helper;
using ChefServe.Infrastructure.Data;
using ChefServe.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;



[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    public AuthController()
    {

    }

    [HttpPost("CreateFolder")]
    public async Task<ActionResult> CreateFolder([FromBody] string Token, [FromBody] string FolderName, [FromBody] string ParentPath)
    {
        var user = 
    }
}