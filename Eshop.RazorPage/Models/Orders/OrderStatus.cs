namespace Eshop.RazorPage.Models.Orders;

public enum OrderStatus
{
    Pending,        // هنوز نهایی نشده و پرداخت هم نشده
    CheckedOut,     // کاربر سفارش رو نهایی کرده، ولی هنوز پرداخت نکرده
    Finally,        // پرداخت انجام شده
    Shipping,
    Rejected
}