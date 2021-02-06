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

            CreateMap<OrderMasterViewModel_Datatable, OrderMasterViewModel_Datatable>()
                .ReverseMap();

        }
    }
}
