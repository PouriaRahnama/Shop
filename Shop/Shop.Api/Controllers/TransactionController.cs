﻿using Common.Application;
using Common.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shop.Api.Infrastructure.Gateways.Zibal;
using Shop.Api.Infrastructure.Gateways.Zibal.DTOs;
using Shop.Api.ViewModels.Transactions;
using Shop.Application.Orders.Finally;
using Shop.Domain.OrderAgg;
using Shop.Presentation.Facade.Orders;

namespace Shop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ApiController
    {
        private readonly IZibalService _zibalService;
        private readonly IOrderFacade _orderFacade;

        public TransactionController(IZibalService zibalService, IOrderFacade orderFacade)
        {
            _zibalService = zibalService;
            _orderFacade = orderFacade;
        }

        [HttpPost]
        [Authorize]
        public async Task<ApiResult<string>> CreateTransaction(CreateTransactionViewModel command)
        {
            var order = await _orderFacade.GetOrderById(command.OrderId);
            if (order == null || order.Address == null || order.ShippingMethod == null)
                return CommandResult(OperationResult<string>.NotFound());


            var url = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            var result = await _zibalService.StartPay(new ZibalPaymentRequest()
            {
                Amount = order.TotalPrice,
                CallBackUrl = $"{url}/api/transaction?orderId={order.Id}&errorRedirect={command.ErrorCallBackUrl}&successRedirect={command.SuccessCallBackUrl}",
                Description = $"پرداخت سفارش با شناسه {order.Id}",
                LinkToPay = false,
                Merchant = "zibal",
                SendSms = false,
                Mobile = User.GetPhoneNumber()
            });
            return CommandResult(OperationResult<string>.Success(result));
        }

        [HttpGet]
        public async Task<IActionResult> Verify(long orderId, long trackId, int success, string errorRedirect, string successRedirect)
        {
            if (success == 0)
            {
                await _orderFacade.CancelOrder(orderId); // 🔴 لغو سفارش چون پرداخت انجام نشده
                return Redirect(errorRedirect);
            }
               
            var order = await _orderFacade.GetOrderById(orderId);

            if (order == null)
                return Redirect(errorRedirect);

            // 🛑 جلوگیری از پردازش مجدد سفارش‌هایی که نهایی یا رد شده‌اند
            if (order.Status == OrderStatus.Finally)
            {
                return Redirect(successRedirect); // یا یک پیام مشخص، مثلا "پرداخت قبلاً انجام شده است"
            }

            // 🛑 جلوگیری از پردازش مجدد سفارش‌هایی که نهایی یا رد شده‌اند
            if (order.Status == OrderStatus.Rejected)
            {
                return Redirect(errorRedirect); // یا یک پیام مشخص، مثلا "پرداخت قبلاً انجام شده است"
            }

            var result = await _zibalService.Verify(new ZibalVeriyfyRequest(trackId, "zibal"));
            if (result.Status != 1)
            {
                await _orderFacade.CancelOrder(orderId); // 🔴 لغو سفارش چون پرداخت تایید نشده
                return Redirect(errorRedirect);
            }


            if (result.Amount != order.TotalPrice)
            {
                await _orderFacade.CancelOrder(orderId); // 🔴 لغو سفارش چون مبلغ تطابق نداره
                return Redirect(errorRedirect);
            }

            var commandResult = await _orderFacade.FinallyOrder(new OrderFinallyCommand(orderId));

            if (commandResult.Status == OperationResultStatus.Success)
                return Redirect(successRedirect);

            await _orderFacade.CancelOrder(orderId); // 🔴 لغو سفارش چون مبلغ تطابق نداره
            return Redirect(errorRedirect);
        }
    }
}
