using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Recipes.Data;
using Recipes.Data.Models;
using Recipes.Models;
using Recipes.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Recipes.Controllers
{
    public class RecipeController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IShortStringService shortStringService;
        private string[] allowedExtention = new[] {"png", "jpg","jpeg" }; 

        public RecipeController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment,
            UserManager<ApplicationUser> userManager,IShortStringService shortStringService,IRecipeService recipeService)
        {
            this.db = db;
            this.webHostEnvironment = webHostEnvironment;
            this.userManager = userManager;
            this.shortStringService = shortStringService;
        }
        // List with recipes
        public IActionResult Index()
        {
            var model = db.Recipes.Select(x=>new InputRecipeModel {
                Name = x.Name,
                Id = x.Id,
                ImgURL = $"/img/{x.Images.FirstOrDefault().Id}.{x.Images.FirstOrDefault().Extention}"
            }).ToList();
            return View(model);
        }
        [HttpGet]
        public IActionResult Add()
        {
            // 
            var categories = db.Categories.Select(x =>
               new SelectListItem { 
                   Text = x.Name,
                   Value = x.Id.ToString()
               }).ToList();
            InputRecipeModel model = new InputRecipeModel { 
            Categories = categories};
            return View(model); 
        }
        [HttpPost]
        public IActionResult Add(InputRecipeModel model)
        {
            //Добавяне на 1 рецепта в базата данни
            var recipe = new Recipe
            {
                Name = model.Name,
                PreparationTime = TimeSpan.FromMinutes(model.PreparationTime),
                CookingTime = TimeSpan.FromMinutes(model.CookingTime),
                PortionCount = model.PortionCount,
                Description = model.Description,
                CategoryId = model.CategoryId
            };
            // от името на прикачения файл получаваме неговото разширение .png
            var extention = Path.GetExtension(model.Image.FileName).TrimStart('.');
            //създаваме обект, който ще се запише в БД
            var image = new Image {
                Extention = extention
                };
            // задаваме името на файла заедно с директориите
            string path = $"{webHostEnvironment.WebRootPath}/img/{image.Id}.{extention}";

            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                model.Image.CopyTo(fs);
             }
            recipe.Images.Add(image);
            foreach (var item in model.Ingredients)
            {
                var ingredient = db.Ingredients.FirstOrDefault(x=>item.Name == x.Name);
                if (ingredient == null)
                {
                    ingredient = new Ingredient { Name = item.Name };
                }

                recipe.Ingredients.Add(new RecipeIngredient { 
                Ingredient = ingredient,
                Quantity = item.Quantity
                });
            }

            db.Recipes.Add(recipe);
            db.SaveChanges();

            return this.Redirect("/");
        }
        public IActionResult ById(int id)
        {
            var model = db.Recipes.Where(x => x.Id == id).Select(x => new InputRecipeModel {
                Id = x.Id,
             Name =x.Name,
             CategoryName = x.Category.Name,
             PreparationTime = TimeSpan.FromMinutes(x.PreparationTime.TotalMinutes).Minutes,
             CookingTime =TimeSpan.FromMinutes(x.CookingTime.TotalMinutes).Minutes,
             PortionCount = x.PortionCount,
             ImgURL = $"/img/{x.Images.FirstOrDefault().Id}.{x.Images.FirstOrDefault().Extention}",
             Ingredients = x.Ingredients.Select(x => new InputRecipeIngredientModel { 
                Name = x.Ingredient.Name,
                Quantity = x.Quantity
                }).ToList(),
             Description = shortStringService.GetShort(x.Description,20)

            }).FirstOrDefault();
            return this.View(model);
        }
        public IActionResult Edit(int id)
        {
            var model = recipeService.GetRecipeById(id);
            return this.View();
        }

    }
}
