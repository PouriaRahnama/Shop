using Common.Query;
using Dapper;
using Shop.Domain.SellerAgg.Repository;
using Shop.Infrastructure.Persistent.Dapper;
using Shop.Infrastructure.Persistent.Ef;
using Shop.Infrastructure.Persistent.Ef.SellerAgg;
using Shop.Query.Sellers.DTOs;

namespace Shop.Query.Sellers.Inventories.GetList;

public class GetInventoriesQueryHandler : IQueryHandler<GetInventoriesQuery, List<InventoryDto>>
{
    private readonly ShopContext _Shopcontext;
    private readonly DapperContext _context;
     ISellerRepository _sellerRepository;

    public GetInventoriesQueryHandler(DapperContext context, ISellerRepository sellerRepository)
    {
        _context = context;
        _sellerRepository = sellerRepository;
    }
    public async Task<List<InventoryDto>> Handle(GetInventoriesQuery request, CancellationToken cancellationToken)
    {
        //One or more errors occurred. (Cannot create a DbSet for 'SellerInventory' because it is configured as an owned entity type and must be accessed through its owning entity type 'Seller'. See https://aka.ms/efcore-docs-owned for more information.)
        //var result =  _sellerRepository.GetInventoryByIds(1);

        using var connection = _context.CreateConnection();

        var sql = @$"SELECT i.Id, i.SellerId , i.ProductId ,i.Count , i.Price,i.CreationDate , i.DiscountPercentage , s.ShopName ,
                        p.Title as ProductTitle,p.ImageName as ProductImage
            FROM {_context.Inventories} i inner join {_context.Sellers} s on i.SellerId=s.Id  
            inner join {_context.Products} p on i.ProductId=p.Id WHERE i.SellerId=@sellerId";

        var inventories = await connection.QueryAsync<InventoryDto>(sql, new { sellerId = request.SellerId });
        return inventories.ToList();
    }
}