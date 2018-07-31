using AutoMapper;
using WebApplicationExercise.Models;
using WebApplicationExercise.ViewModels;

namespace WebApplicationExercise.Utils.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Order, OrderViewModel>()
                .ForMember(m => m.Customer, e => e.MapFrom(o => o.CustomerName));

            CreateMap<ProductViewModel, Product>()
                .ForMember(x => x.OrderId, opt => opt.Ignore())
                .ForMember(x => x.Order, opt => opt.Ignore());
            
        }
    }
}