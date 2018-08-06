using AutoMapper;
using System;
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
                .ForMember(q => q.CreatedDate, w => w.MapFrom(r => r.CreatedDate.ConvertToDateTimeUtc()));

            CreateMap<Order, OrderViewModel>()
                .ForMember(m => m.Customer, e => e.MapFrom(q => q.CustomerName))
                .ForMember(q => q.CreatedDate, w => w.MapFrom(r => r.CreatedDate.ConvertToStringUtc()));

            CreateMap<ProductViewModel, Product>()
                .ForMember(x => x.OrderId, opt => opt.Ignore())
                .ForMember(x => x.Order, opt => opt.Ignore());
        }
    }
}