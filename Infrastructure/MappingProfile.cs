using FieldServiceApp.Models;
using AutoMapper;

namespace FieldServiceApp.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CustomerMasterViewModel_datatable, CustomerMasterViewModel_datatable>()
                .ReverseMap();
         
        }
    }
}
