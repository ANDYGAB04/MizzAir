using System;
using System.Linq;
using API.DTOs;
using API.Interface;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(UserManager<User> userManager, ITokenService tokenService, IMapper mapper) : BaseApiController
{
    [HttpPost("register")]//acount/register
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await EmailExist(registerDto.Email))
        {
            return BadRequest("Username is taken");
        }

        var user = mapper.Map<User>(registerDto);

        user.Email = registerDto.Email.ToLower();

        var result = await userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return new UserDto
        {
            Lastname = user.LastName,
            Token = await tokenService.CreateToken(user),
            FirstName = user.FirstName,
            Email = user.Email
        };
    }

    [HttpPost("login")]//acount/login
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await userManager.Users
            .FirstOrDefaultAsync(x => x.Email == loginDto.Email.ToLower());

        if (user == null || user.Email == null) return Unauthorized("Invalid Email");
        return new UserDto
        {
            Lastname = user.LastName,
            Token = await tokenService.CreateToken(user),
            FirstName = user.FirstName,
            Email = user.Email
        };
    }

    private async Task<bool> EmailExist(string email)
    {
        return await userManager.Users.AnyAsync(x => x.Email == email.ToLower());
    }
}
