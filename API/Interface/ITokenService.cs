using System;
using API.Models;

namespace API.Interface;

public interface ITokenService
{
    Task<string> CreateToken(User user);
}
