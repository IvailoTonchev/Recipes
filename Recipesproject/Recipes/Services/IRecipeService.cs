using Microsoft.CodeAnalysis.CSharp.Syntax;
using Recipes.Models;
using Sitecore.FakeDb;

namespace Recipes.Services
{
    public interface IRecipeService
    {
        public InputRecipeModel GetRecipeById(int id);
        {
    var recipe = Db.Recipes.Where(x => x.Id == id).Select(x => new InputRecipeModel
    {
        Id = x.Id,
        Name = x.Name,
        CategoryName = x.Category.Name,
        PreparationTime = TimeSpan.FromMinutes(x.PreparationTime.TotalMinutes).Minutes,
        CookingTime = TimeSpan.FromMinutes(x.CookingTime.TotalMinutes).Minutes,
        PortionCount = x.PortionCount,
        ImgURL = $"/img/{x.Images.FirstOrDefault().Id}.{x.Images.FirstOrDefault().Extention}",
        Description = x.Description,
    }).FirstOrDefault();
        return Recipes;
      }
    }
}
