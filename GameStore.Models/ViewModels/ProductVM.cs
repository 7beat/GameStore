using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GameStore.Models.ViewModels
{
    public class ProductVM
    {
        public Product Product { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> GenreList { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> PlatformList { get; set; }
    }
}
