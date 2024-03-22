using AuthServer.Core.Dtos;
using Shared.Dtos;

namespace AuthServer.Core.Services;

public interface IAuthenticationService
{
    Task<Response<TokenDto>> CreateToken(LoginDto loginDto);
    Task<Response<TokenDto>> CreateTokenByREfreshToken(string refreshToken);
    Task<Response<NoDataDto>> RevokeRefreshToken(string refreshToken);
    Task<Response<ClientTokenDto>> CreateTokenByClient(ClientLoginDto clientLoginDto);
}
