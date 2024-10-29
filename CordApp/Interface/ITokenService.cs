using CordApp.Models;

namespace CordApp.Interface
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}
