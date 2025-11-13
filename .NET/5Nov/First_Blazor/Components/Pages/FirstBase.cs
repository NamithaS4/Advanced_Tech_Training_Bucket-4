using DataAccess;
using Microsoft.AspNetCore.Components;
namespace First_Blazor.Components.Pages
{
    public class FirstBase : ComponentBase
    {
        public IEnumerable<Employee> Employees { get; set; }
        protected override Task OnInitializedAsync()
        {
            LoadEmployees();
                return base.OnInitializedAsync();
        }

        private void LoadEmployees()
        {
            Employees = new List<Employee>
            {
                new Employee { Id = 1, Name = "Nami", Position = "Software Engineer" },
                new Employee { Id = 2, Name = "Shubham", Position = "AI-ML" },
                new Employee { Id = 3, Name = "Gopal", Position = "AI-ML" },
                new Employee { Id = 4, Name = "Ganaesh", Position = "Backend" }
            };
        }
    }
}
