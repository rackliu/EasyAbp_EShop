using System;

namespace EasyAbp.EShop.Orders.Orders.Dtos;

public class OrderDiscountDto : IOrderDiscount
{
    public Guid OrderId { get; set; }

    public Guid OrderLineId { get; set; }

    public string EffectGroup { get; set; }

    public string Name { get; set; }

    public string Key { get; set; }

    public string DisplayName { get; set; }

    public decimal DiscountedAmount { get; set; }
}