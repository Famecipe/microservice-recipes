using Microsoft.AspNetCore.Mvc;
using Famecipe.Services;
using Famecipe.Models;

namespace Famecipe.Microservice.Recipes.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces(System.Net.Mime.MediaTypeNames.Application.Json)]
public class RecipesController : ControllerBase
{
    private readonly RecipeService _recipeService;

    public RecipesController(RecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok(await _recipeService.GetRecipes());
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetList([FromQuery] string[] identifiers)
    {
        var recipes = new List<Recipe>();
        foreach (var identifier in identifiers)
        {
            recipes.Add(await _recipeService.GetRecipe(identifier));
        }
        return Ok(recipes);
    }

    [HttpGet("{identifier}")]
    public async Task<IActionResult> GetByIdentifier(string identifier)
    {
        var recipe = await _recipeService.GetRecipe(identifier);
        if (recipe == null)
        {
            return NotFound();
        }
        else
        {
            return Ok(recipe);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Post(Recipe recipe)
    {
        var obj = await _recipeService.CreateRecipe(recipe);
        return Created($"/recipes/{obj.Identifier}", obj);
    }

    [HttpPut]
    public async Task<IActionResult> Put(string identifier, Recipe recipe)
    {
        var foundItem = await _recipeService.GetRecipe(identifier);
        if (foundItem == null)
        {
            return NotFound();
        }
        else
        {
            await _recipeService.UpdateRecipe(identifier, recipe);
            return NoContent();
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(string identifier)
    {
        var foundItem = await _recipeService.GetRecipe(identifier);
        if (foundItem == null)
        {
            return NotFound();
        }
        else
        {
            await this._recipeService.DeleteRecipe(identifier);
            return NoContent();
        }
    }

    [HttpDelete("all")]
    public async Task<IActionResult> DeleteAll()
    {
        var recipes = await _recipeService.GetRecipes();
        foreach (var recipe in recipes)
        {
            await this._recipeService.DeleteRecipe(recipe.Identifier!);
        }
        return NoContent();
    }
}
