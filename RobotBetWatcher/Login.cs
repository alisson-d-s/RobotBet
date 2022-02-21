using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotBetWatcher
{
    public class Login
    {
        private string User { get; set; } = "vrourex";
        private string Pass { get; set; } = "56ho3etq";

        public async Task LoginUserAsync()
        {
            //hm-MainHeaderRHSLoggedOutMed_Login
            //hm-MainHeaderRHSLoggedOutWide_Login 
            var divLogin = await GetPage.PageResults.WaitForSelectorAsync(".hm-MainHeaderRHSLoggedOutWide_Login ");
            await divLogin.ClickAsync();

            var inputUsername = await GetPage.PageResults.WaitForSelectorAsync(".lms-StandardLogin_Username");
            await inputUsername.EvaluateFunctionAsync($"el => el.value = '{User}'");

            var inputPassword = await GetPage.PageResults.WaitForSelectorAsync(".lms-StandardLogin_Password ");
            await inputPassword.EvaluateFunctionAsync($"el => el.value = '{Pass}'");

            var divInputLogin = await GetPage.PageResults.WaitForSelectorAsync(".lms-LoginButton");
            await divInputLogin.ClickAsync();

            await GetPage.PageResults.WaitForNavigationAsync();
        }
    }
}
