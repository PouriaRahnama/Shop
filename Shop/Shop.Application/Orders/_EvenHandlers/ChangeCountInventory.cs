using AngleSharp.Io;
using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;
using Shop.Domain.OrderAgg.Events;
using Shop.Domain.OrderAgg.Repository;
using Shop.Domain.SellerAgg.Repository;
using System.Data;

namespace Shop.Application.Orders._EvenHandlers;

public class ChangeCountInventory:INotificationHandler<OrderFinalized>
{
    public async Task Handle(OrderFinalized notification, CancellationToken cancellationToken)
    {
        Console.WriteLine("--------------");
    }
}
