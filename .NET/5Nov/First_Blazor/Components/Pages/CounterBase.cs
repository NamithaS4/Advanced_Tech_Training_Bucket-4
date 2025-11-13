using Microsoft.AspNetCore.Components;

namespace First_Blazor.Components.Pages
{
    public class CounterBase : ComponentBase
    {
        public int currentCount = 0;

        public void IncrementCount()
        {
            currentCount++;
        }
    }
}
