﻿using Eshop.RazorPage.Infrastructure;
using Eshop.RazorPage.Infrastructure.RazorUtils;
using Eshop.RazorPage.Models.Comments;
using Eshop.RazorPage.Models.Products;
using Eshop.RazorPage.Models.Sellers;
using Eshop.RazorPage.Services.Comments;
using Eshop.RazorPage.Services.Products;
using Eshop.RazorPage.Services.Sellers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Eshop.RazorPage.Pages;

public class ProductModel : BaseRazorPage
{
    private readonly IProductService _service;
    private readonly ISellerService _sellerService;
    private ICommentService _commentService;
    public ProductModel(IProductService service, ISellerService sellerService, ICommentService commentService)
    {
        _service = service;
        _sellerService = sellerService;
        _commentService = commentService;
    }

    public SingleProductDto ProductPageModel { get; set; }

    public async Task<IActionResult> OnGet(string slug)
    {
        var product = await _service.GetSingleProduct(slug);
        if (product.ProductDto.Id == 0)
            return NotFound();

        ProductPageModel = product;
        return Page();
    }

    public async Task<IActionResult> OnPost(string slug,string comment)
    {
        if (User.Identity.IsAuthenticated == false)
            return Page();

        var prod = await _service.GetProductBySlug(slug);

        var result = await _commentService.AddComment(new AddCommentCommand()
        {
            ProductId = prod.Id,
            Text = comment,
            UserId = User.GetUserId()
        });
        if (result.IsSuccess == false)
        {
            ErrorAlert(result.MetaData.Message);
            return RedirectToPage("Product", new { slug });
        }
        SuccessAlert("نظر شما ثبت شد ، بعد از تایید در سایت نمایش داده می شود");
        Thread.Sleep(3000);
        return RedirectToPage("Product", new { slug });
    }
    public async Task<IActionResult> OnGetProductComments(long productId, int pageId = 1)
    {
        var commentResult = await _commentService.GetProductComments(pageId, 14, productId);
        return Partial("Shared/Products/_Comments", commentResult);
    }
    public async Task<IActionResult> OnPostDeleteComment(long id)
    {
        return await AjaxTryCatch(() => _commentService.DeleteComment(id));
    }
}