using Domain.Entities;
using Domain.Interfaces;
using MediatR;
using AutoMapper;
using Application.Services;
using Application.ViewModels;

namespace Application.Handlers;

public class RegisterHandler : IRequestHandler<RegisterCommand, AuthenticationResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IJwtService _jwtService;

    public RegisterHandler(IUserRepository userRepository, IMapper mapper, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _jwtService = jwtService;
    }

    public async Task<AuthenticationResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // 1. Check if user already exists
        var existingUser = await _userRepository.GetByUsernameAsync(request.Person.Username);

        if (existingUser != null)
            throw new Exception("Username already exists");

        // 2. Validate password
        if (request.Person.PasswordHash != request.Person.ConfirmPassword)
            throw new Exception("Passwords do not match");

        // 3. Hash password
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Person.PasswordHash);

        // 4. Create domain entity via AutoMapper then ensure hashed password is set
        var user = _mapper.Map<User>(request.Person);
        user.PasswordHash = passwordHash;

        // 5. Save user
        await _userRepository.AddAsync(user);

        // 6. Generate JWT
        return _jwtService.GenerateToken(user);
    }
}
// Move GenerateJwtToken helper to be shared with LoginHandler via internal static helper
public class LoginHandler : IRequestHandler<LoginCommand, AuthenticationResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    public LoginHandler(IUserRepository userRepository, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<AuthenticationResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Person.Username);
        if (user == null)
            throw new Exception("Invalid credentials");

        var valid = BCrypt.Net.BCrypt.Verify(request.Person.PasswordHash, user.PasswordHash);
        if (!valid) throw new Exception("Invalid credentials");

        // Generate JWT
        return _jwtService.GenerateToken(user);
    }
}
