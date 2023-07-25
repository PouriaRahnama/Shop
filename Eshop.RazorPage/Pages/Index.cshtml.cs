using Eshop.RazorPage.Models;
using Eshop.RazorPage.Services.Auth;
using Eshop.RazorPage.Services.MainPage;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Eshop.RazorPage.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IAuthService _authService;
        private readonly IMainPageService _mainPageService;
        public IndexModel(ILogger<IndexModel> logger, IAuthService authService, IMainPageService mainPageService)
        {
            _logger = logger;
            _authService = authService;
            _mainPageService = mainPageService;
        }
        public MainPageDto? MainPageData { get; set; }
        public async Task OnGet()
        {
            Thread.Sleep(1000);
            MainPageData = await _mainPageService.GetMainPageData();

        }
    }
}