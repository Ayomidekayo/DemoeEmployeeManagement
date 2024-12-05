
namespace BaseLibrary.Responses
{
    public record LoginResponse(bool Flag, string message=null!, string Token=null!, string RefreshToken=null!);
    
}
