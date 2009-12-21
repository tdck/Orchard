using System.Collections.Generic;
using Orchard.Blogs.Models;
using Orchard.ContentManagement.ViewModels;
using Orchard.Mvc.ViewModels;

namespace Orchard.Blogs.ViewModels {
    public class BlogsViewModel : BaseViewModel {
        public IEnumerable<ItemDisplayModel<Blog>> Blogs { get; set; }
    }
}