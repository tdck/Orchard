using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using Orchard.Blogs.Services;
using Orchard.Core.Common.Models;
using Orchard.Data;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.ContentManagement.ViewModels;

namespace Orchard.Blogs.Models {
    public class BlogPostHandler : ContentHandler {
        public override IEnumerable<ContentType> GetContentTypes() {
            return new[] { BlogPost.ContentType };
        }

        public BlogPostHandler(
            IRepository<BlogPostRecord> repository,
            IContentManager contentManager,
            IBlogPostService blogPostService) {

            Filters.Add(new ActivatingFilter<BlogPost>("blogpost"));
            Filters.Add(new ActivatingFilter<CommonAspect>("blogpost"));
            Filters.Add(new ActivatingFilter<RoutableAspect>("blogpost"));
            Filters.Add(new ActivatingFilter<BodyAspect>("blogpost"));
            Filters.Add(new StorageFilter<BlogPostRecord>(repository));
            Filters.Add(new ContentItemTemplates<BlogPost>("Items/Blogs.BlogPost", "Summary SummaryAdmin"));

            OnCreated<BlogPost>((context, bp) => bp.Blog.PostCount++);

            OnGetItemMetadata<BlogPost>((context, bp) => {
                context.Metadata.DisplayText = bp.Title;
                context.Metadata.DisplayRouteValues =
                    new RouteValueDictionary(
                        new {
                            area = "Orchard.Blogs",
                            controller = "BlogPost",
                            action = "Item",
                            blogSlug = bp.Blog.Slug,
                            postSlug = bp.Slug
                        });
                context.Metadata.EditorRouteValues =
                    new RouteValueDictionary(
                        new {
                            area = "Orchard.Blogs",
                            controller = "BlogPost",
                            action = "Edit",
                            blogSlug = bp.Blog.Slug,
                            postSlug = bp.Slug
                        });
            });

            OnGetDisplayViewModel<Blog>((context, blog) => {
                if (!context.DisplayType.StartsWith("Detail"))
                    return;

                var posts = blogPostService.Get(blog);

                switch(context.DisplayType) {
                    case "Detail":
                        context.AddDisplay(
                            new TemplateViewModel(posts.Select(bp => contentManager.BuildDisplayModel(bp, "Summary"))) {
                                TemplateName = "Parts/Blogs.BlogPost.List",
                                ZoneName = "body"
                            });
                        break;
                    case "DetailAdmin":
                        context.AddDisplay(
                            new TemplateViewModel(posts.Select(bp => contentManager.BuildDisplayModel(bp, "SummaryAdmin"))) {
                                TemplateName = "Parts/Blogs.BlogPost.List",
                                ZoneName = "body"
                            });
                        break;
                }
            });

            OnGetEditorViewModel<BlogPost>((context, blogPost) => {
                context.AddEditor(new TemplateViewModel(blogPost) { TemplateName = "Parts/Blogs.BlogPost.Fields", ZoneName = "primary", Position = "1" });
                context.AddEditor(new TemplateViewModel(blogPost) { TemplateName = "Parts/Blogs.BlogPost.Publish", ZoneName = "secondary", Position = "1" });
            });

            OnUpdateEditorViewModel<BlogPost>((context, blogPost) => {
                context.AddEditor(new TemplateViewModel(blogPost) { TemplateName = "Parts/Blogs.BlogPost.Fields", ZoneName = "primary", Position = "1" });
                context.AddEditor(new TemplateViewModel(blogPost) { TemplateName = "Parts/Blogs.BlogPost.Publish", ZoneName = "secondary", Position = "1" });
                context.Updater.TryUpdateModel(blogPost, "", null, null);
            });
        }

    }
}