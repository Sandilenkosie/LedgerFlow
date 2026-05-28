using Domain.Entities;
using Application.ViewModels;

namespace Application.Services;

public interface IJwtService
{
    AuthenticationResult GenerateToken(User user);
}
