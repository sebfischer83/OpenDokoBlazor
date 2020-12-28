using Excubo.Generators.Blazor;

namespace OpenDokoBlazor.Client.Pages.Auth
{
    public class UserData
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}