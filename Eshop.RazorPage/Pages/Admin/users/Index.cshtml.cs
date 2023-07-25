using Eshop.RazorPage.Infrastructure.RazorUtils;
using Eshop.RazorPage.Models.Users;
using Eshop.RazorPage.Services.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Eshop.RazorPage.Pages.Admin.users
{
    public class IndexModel : BaseRazorFilter<UserFilterParams>
    {
        private IUserService _userService;

        public IndexModel(IUserService userService)
        {
            _userService = userService;
        }

        public UserFilterResult FilterResult { get; set; }
        public async Task OnGet()
        {
            FilterParams.Take = 1;
            FilterResult = await _userService.GetUsersByFilter(FilterParams);
        }



    }
}
