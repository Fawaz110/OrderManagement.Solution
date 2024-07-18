using AutoMapper.Configuration.Annotations;
using Core.Entities.OrderAggregate;
using System.ComponentModel.DataAnnotations;

namespace Service.Dtos.OrderDtos
{
    public class OrderItemDto
    {
        public ProductOrderItem Product { get; set; }
        [Range(1, maximum:1000)]
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
