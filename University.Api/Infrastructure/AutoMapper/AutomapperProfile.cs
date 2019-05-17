using AutoMapper;
using University.Api.ViewModels.Accounts;
using University.Data.Models;

namespace University.Api.Infrastructures.AutoMapper
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<User, UserRegistrationViewModel>().ReverseMap();
            CreateMap<User, LoginViewModel>().ReverseMap();
        }
    }
}
