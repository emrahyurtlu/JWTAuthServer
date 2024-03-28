using AuthServer.Core.Configuration;
using AuthServer.Core.Dtos;
using AuthServer.Core.Models;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared.Dtos;

namespace AuthServer.Service.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly List<Client> _clients;
    private readonly ITokenService _tokenService;
    private readonly UserManager<UserApp> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGenericRepository<UserRefreshToken> _userRefreshTokenService;

    public AuthenticationService(IOptions<List<Client>> optionsClient,UserManager<UserApp> userManager, ITokenService tokenService, IUnitOfWork unitOfWork, IGenericRepository<UserRefreshToken> userRefreshTokenService)
    {
        _clients = optionsClient.Value;
        _userManager = userManager;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
        _userRefreshTokenService = userRefreshTokenService;
    }

    public async Task<Response<TokenDto>> CreateToken(LoginDto loginDto)
    {
        if (loginDto is null)
        {
            throw new ArgumentNullException(nameof(loginDto));
        }

        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        
        if (user == null)
        {
            return Response<TokenDto>.Fail("Email or Password is wrong.",404);
        }

        var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!isPasswordCorrect)
        {
            return Response<TokenDto>.Fail("Email or Password is wrong.", 404);
        }

        var token = _tokenService.CreateToken(user);
        var userRefreshToken = await _userRefreshTokenService.Where(x => x.UserId == user.Id).SingleOrDefaultAsync();
        if (userRefreshToken == null)
        {
            await _userRefreshTokenService.AddAsync(new UserRefreshToken { UserId = user.Id, Code = token.RefreshToken, Expiration = token.RefreshTokenExpiration });
        } else
        {
            userRefreshToken.Code = token.RefreshToken;
            userRefreshToken.Expiration = token.RefreshTokenExpiration;
        }

        await _unitOfWork.SaveChangesAsync();
        return Response<TokenDto>.Success(token, 200);
    }

    public Response<ClientTokenDto> CreateTokenByClient(ClientLoginDto clientLoginDto)
    {
        var client = _clients.SingleOrDefault(t => t.Id == clientLoginDto.ClientId && t.Secret == clientLoginDto.ClientSecret);
        if (client == null)
        {
            return Response<ClientTokenDto>.Fail("ClientId or ClientSecret not found",404);
        }

        var token = _tokenService.CreateTokenByClient(client);

        return Response<ClientTokenDto>.Success(token,200);
    }

    public async Task<Response<TokenDto>> CreateTokenByRefreshToken(string refreshToken)
    {
        var refToken = await _userRefreshTokenService.Where(t => t.Code == refreshToken).SingleOrDefaultAsync();
        if (refToken == null)
        {
            return Response<TokenDto>.Fail("Refresh token not found.", 404);
        }

        var user = await _userManager.FindByIdAsync(refToken.UserId);

        if (user == null)
        {
            return Response<TokenDto>.Fail("UserId not found.", 404);
        }

        var tokenDto = _tokenService.CreateToken(user);
        refToken.Code = tokenDto.RefreshToken;
        refToken.Expiration = tokenDto.RefreshTokenExpiration;

        await _unitOfWork.SaveChangesAsync();

        return Response<TokenDto>.Success(tokenDto, 200);
    }

    public async Task<Response<NoDataDto>> RevokeRefreshToken(string refreshToken)
    {
        var existRefreshToken = await _userRefreshTokenService.Where(t => t.Code == refreshToken).SingleOrDefaultAsync();
        if (existRefreshToken == null)
        {
            return Response<NoDataDto>.Fail("Refresh token not found.", 404);
        }

        _userRefreshTokenService.Remove(existRefreshToken);
        await _unitOfWork.SaveChangesAsync();

        return Response<NoDataDto>.Success(200);
    }
}
