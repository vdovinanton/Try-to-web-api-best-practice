using AutoMapper;
using WebApplicationExercise.Models;
using WebApplicationExercise.ViewModels;

namespace WebApplicationExercise.Utils.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<OrderViewModel, Order>()
                .ForMember(m => m.CustomerName, e => e.MapFrom(o => o.Customer))
                .ForMember(q => q.CreatedDate, w => w.MapFrom(r => r.CreatedDate.ConvertFromStringToUnix()));

            CreateMap<Order, OrderViewModel>()
                .ForMember(q => q.CreatedDate, w => w.MapFrom(r => r.CreatedDate.ConvertFromUnixToStringUtc()));

            CreateMap<ProductViewModel, Product>()
                .ForMember(x => x.OrderId, opt => opt.Ignore())
                .ForMember(x => x.Order, opt => opt.Ignore());
        }
    }
}