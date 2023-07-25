using Common.Application;
using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;
using Shop.Application.Orders._EvenHandlers;
using Shop.Domain.OrderAgg.Repository;
using Shop.Domain.SellerAgg.Repository;
using System.Data;

namespace Shop.Application.Orders.Finally;

public class OrderFinallyCommandHandler : IBaseCommandHandler<OrderFinallyCommand>
{
    private readonly IOrderRepository _orderRepository;
    public OrderFinallyCommandHandler(IOrderRepository repository)
    {
        _orderRepository = repository;
    }

    public async Task<OperationResult> Handle(OrderFinallyCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetTracking(request.OrderId);
        if (order == null)
            return OperationResult.NotFound();

        order.Finally();

        await _orderRepository.Save();
        return OperationResult.Success();
    }
}