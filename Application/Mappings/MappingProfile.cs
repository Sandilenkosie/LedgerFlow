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
        CreateMap<PersonViewModel, User>();

        CreateMap<User, PersonViewModel>()
            .ForMember(dest => dest.Accounts, opt => opt.MapFrom(src => src.Accounts));

        // Map Account to the lightweight RelatedAccountsViewModel used on person listings
        CreateMap<Account, RelatedAccountsViewModel>();

        CreateMap<Account, AccountViewModel>();

        // account creation mapping handled in handler; keep Account mapping configured

        CreateMap<Transaction, RelatedTransactionsViewModel>();
        CreateMap<Transaction, TransactionViewModel>();
    }
}
