using Common.Domain;
using Common.Domain.Exceptions;

namespace Shop.Domain.SellerAgg
{
    public class SellerInventory : BaseEntity
    {
        public SellerInventory(long productId, int count, int price, int? discountPercentage = null)
        {
            if (price < 1 || count < 0)
                throw new InvalidDomainDataException();

            ProductId = productId;
            Count = count;
            Price = price;
            DiscountPercentage = discountPercentage;
            ReservedCount = 0;
        }

        public long SellerId { get; internal set; }
        public long ProductId { get; private set; }
        public int Count { get; private set; }
        public int ReservedCount { get; private set; }   // مقدار رزرو شده
        public int AvailableCount => Count - ReservedCount;  // موجودی قابل فروش
        public int Price { get; private set; }
        public int? DiscountPercentage { get; private set; }


        public void Edit(int count, int price, int? discountPercentage = null)
        {
            if (price < 1 || count < 0)
                throw new InvalidDomainDataException();

            Count = count;
            Price = price;
            DiscountPercentage = discountPercentage;
        }

        // متد رزرو کردن کالا
        public void Reserve(int count)
        {
            if (count < 1 || AvailableCount < count)
                throw new InvalidDomainDataException("موجودی کافی برای رزرو وجود ندارد.");

            ReservedCount += count;
        }

        // آزاد کردن رزرو (وقتی پرداخت انجام نشه یا لغو بشه)
        public void ReleaseReserved(int count)
        {
            if (count < 1 || ReservedCount < count)
                throw new InvalidDomainDataException("رزرو برای آزادسازی کافی نیست.");

            ReservedCount -= count;
        }

        // کاهش موجودی واقعی (بعد از پرداخت موفق)
        public void DecreaseCount(int count)
        {
            if (count < 1 || ReservedCount < count)
                throw new InvalidDomainDataException("موجودی رزرو شده کافی نیست.");

            ReservedCount -= count;
            Count -= count;
        }
    }
}