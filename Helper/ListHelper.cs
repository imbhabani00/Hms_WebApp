using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hms.WebApp.Helper
{
    public class ListHelper
    {
        public static List<SelectListItem> PageItems = new()
    {
        new() { Text = "10", Value = "10", Selected = true },
        new() { Text = "20", Value = "20" },
        new() { Text = "30", Value = "30" },
        new() { Text = "40", Value = "40" }
    };

        public static List<SelectListItem> PageItems2 = new()
    {
        new() { Text = "20", Value = "20", Selected = true },
        new() { Text = "40", Value = "40" },
        new() { Text = "60", Value = "60" },
        new() { Text = "80", Value = "80" }
    };

        public static List<SelectListItem> GetPageSizes(int selectedSize = 10)
        {
            return PageItems.Select(item => new SelectListItem
            {
                Text = item.Text,
                Value = item.Value,
                Selected = item.Value == selectedSize.ToString()
            }).ToList();
        }
    }
}
