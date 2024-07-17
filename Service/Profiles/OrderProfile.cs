using AutoMapper;
using Core.Entities.Order.Aggregate;
using Service.Dtos.OrderDtos;

namespace Service.Profiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, OrderDto>().ReverseMap();
        }
    }
}
