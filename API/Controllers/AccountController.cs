using System;
using System.Linq;
using API.DTOs;
using API.Extensions;
using API.Interface;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(UserManager<User> userManager, ITokenService tokenService, IMapper mapper, IAccountService accountService) : BaseApiController
{
    [HttpPost("register")]//acount/register
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await EmailExist(registerDto.Email))
        {
            return BadRequest("Email is taken");
        }

        var user = mapper.Map<User>(registerDto);

        user.Email = registerDto.Email.ToLower();

        if (registerDto.FirstName is null)
        {
            return BadRequest("FirstName is not set");
        }
        user.UserName = registerDto.FirstName.ToLower();

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
            Email = user.Email,
            City = user.City,
            Country = user.Country,
            Address = user.Address
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
            Email = user.Email,
            City = user.City,
            Country = user.Country,
            Address = user.Address
        };
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var user = await accountService.GetCurrentUserAsync(User.GetUserId());
        if (user == null)
        {
            return NotFound("User not found");
        }

        return Ok(user);
    }

    [Authorize]
    [HttpPatch]
    public async Task<ActionResult<UserDto>> UpdateAccount(UpdateAccountDto updateAccountDto)
    {
        var result = await accountService.UpdateAccountAsync(User.GetUserId(), updateAccountDto);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result.User);
    }

    [Authorize]
    [HttpDelete]
    public async Task<ActionResult<DeleteAccountResultDto>> DeleteCurrentUser()
    {
        var result = await accountService.DeleteCurrentUserAsync(User.GetUserId());
        if (result == null)
        {
            return NotFound("User not found");
        }

        return Ok(result);
    }

    private async Task<bool> EmailExist(string email)
    {
        return await userManager.Users.AnyAsync(x => x.Email == email.ToLower());
    }
}
