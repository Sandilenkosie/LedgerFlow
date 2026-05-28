using AutoMapper;
using Domain.Entities;
using Application.ViewModels;

namespace Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Map LoginViewModel to domain User for authentication
        CreateMap<LoginViewModel, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.PasswordHash));

        // Map RegisterViewModel to domain User for creating new users
        CreateMap<RegisterViewModel, User>();

        CreateMap<User, PersonViewModel>();

        CreateMap<CreateAccountCommand, Account>()
            .ConstructUsing(src => new Account(src.AccountNumber));

        CreateMap<CreateTransactionCommand, Transaction>()
            .ConstructUsing(src => new Transaction(src.Amount, src.Date));
    }
}
