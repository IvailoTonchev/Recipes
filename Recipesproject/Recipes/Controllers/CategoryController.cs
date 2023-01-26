using Microsoft.AspNetCore.Mvc;
using Recipes.Data;
using Recipes.Data.Models;
using Recipes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recipes.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext db;

        public CategoryController(ApplicationDbContext db)
        {
            this.db = db;
        }
        public IActionResult Index()
        {
            var categories = db.Categories.Select( x => new InputCategoryModel
            {
             Id = x.Id,
             Name = x.Name
            }).ToList();

            return View(categories);
        }
        [HttpGet]
        public IActionResult Add()
        {
            return this.View();
        }
        [HttpPost]
        public IActionResult Add(InputCategoryModel model)
        {
            var category = new Category { Name = model.Name };
            db.Categories.Add(category);
            db.SaveChanges();

            return this.Redirect("Index");
        }
    }
}
