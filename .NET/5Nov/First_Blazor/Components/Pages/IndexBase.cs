using Microsoft.AspNetCore.Components;
namespace First_Blazor.Components.Pages
{
    public class IndexBase : ComponentBase
    {
        public string Text { get; set; } = "Click me";

        protected void ChangeText()
        {
            Text = "You clicked me!";
        }
    }
}
