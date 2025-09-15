using Microsoft.EntityFrameworkCore;
using mini_blog.Data;
using NanoidDotNet;
using Slugify;

namespace mini_blog.Helpers;

public class SlugService(BlogDbContext context)
{
    public async Task<string> GenerateUniqueSlugFromTitle(string title)
    {
        var helper = new SlugHelper();
        var baseSlug = helper.GenerateSlug(title);
      
        var uniqueSuffix = GenerateNanoid(Nanoid.Alphabets.LowercaseLettersAndDigits, 12);
        var slug = $"{baseSlug}-{uniqueSuffix}";
        
        //check if slug already exists
        while (await context.Posts.AnyAsync(p => p.Slug == slug))
        {
            slug = $"{baseSlug}-{uniqueSuffix}";
          
        }
        return slug;

    }

    private static string GenerateNanoid(string alphabet, int length)
    {
        var uniqueSuffix = Nanoid.Generate(alphabet, length);
        
        return uniqueSuffix;
    }
}