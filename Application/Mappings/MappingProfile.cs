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
        CreateMap<Account, RelatedAccountsViewModel>()
            .ForMember(dest => dest.Created, opt => opt.MapFrom(src => src.Created))
            .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.Person != null ? src.Person.Name : string.Empty))
            .ForMember(dest => dest.AccountType, opt => opt.MapFrom(src => src.AccountType));

        CreateMap<Account, AccountViewModel>()
            .ForMember(dest => dest.AccountType, opt => opt.MapFrom(src => src.AccountType))
            .ForMember(dest => dest.Created, opt => opt.MapFrom(src => src.Created))
            .ForMember(dest => dest.PersonId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Person, opt => opt.MapFrom(src => src.Person))
            .ForMember(dest => dest.Transactions, opt => opt.MapFrom(src => src.Transactions));

        CreateMap<Transaction, RelatedTransactionsViewModel>()
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.AccountId))
            .ForMember(dest => dest.TransactionDate, opt => opt.MapFrom(src => src.TransactionDate));

        // account creation mapping handled in handler; keep Account mapping configured

        CreateMap<Transaction, TransactionViewModel>();
    }
}
