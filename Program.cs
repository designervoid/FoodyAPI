using Microsoft.Extensions.FileProviders;

using DataSourceFactory;
using Models;
using Entity.AppDbContext;
using Microsoft.EntityFrameworkCore;
using DTO;

var builder = WebApplication.CreateBuilder(args);

var Configuration = builder.Configuration;

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(Configuration["pgConnection"]));

var app = builder.Build();

var cacheMaxAgeOneWeek = (60 * 60 * 24 * 7).ToString();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "StaticFiles")
    ),
    RequestPath = "/StaticFiles",
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append(
             "Cache-Control", $"public, max-age={cacheMaxAgeOneWeek}");
    }
});

var connectionString = builder.Configuration["pgConnection"];
await using var dataSource = DatabaseConnectionRepository.GetDataSource(builder);

app.MapPost("/add-food", async (FoodItem foodItem, AppDbContext context) =>
{
    context.FoodItems.Add(foodItem);
    await context.SaveChangesAsync();
    return Results.Ok("Food added");
});

app.MapGet("/get-food-items", async (AppDbContext context) =>
{
    var foodItems = await context.FoodItems.ToListAsync();
    return Results.Ok(foodItems);
});

app.MapPost("/add-meal-item", async (CreateMealItem dto, AppDbContext context) =>
{
    DateTime? parsedReminder = DateTimeOffset.FromUnixTimeMilliseconds(dto.Reminder).UtcDateTime;

    var mealItem = new MealItem
    {
        FoodItemIds = dto.FoodItemIds ?? Array.Empty<int>(),
        Reminder = parsedReminder,
        FoodTypeId = dto.FoodTypeId
    };

    context.MealItems.Add(mealItem);
    await context.SaveChangesAsync();

    return Results.Ok("Meal item created");
});

app.MapGet("/get-meal-items", async (AppDbContext context) =>
{
    var mealItems = await context.MealItems.Include(m => m.FoodItems).Include(m => m.FoodType).ToListAsync();

    foreach (var mealItem in mealItems)
    {
        mealItem.FoodItems = await context.FoodItems.Where(f => mealItem.FoodItemIds.Contains(f.Id)).ToListAsync();
    }

    return Results.Ok(mealItems);
});

app.MapGet("/get-meal-item/{id}", async (int id, AppDbContext context) =>
{
    var mealItem = await context.MealItems.Include(m => m.FoodItems).Include(m => m.FoodType).FirstOrDefaultAsync(m => m.Id == id);
    if (mealItem == null) return Results.NotFound($"Meal item with id {id} not found");

    if (mealItem.FoodItems == null || !mealItem.FoodItems.Any())
    {
        mealItem.FoodItems = await context.FoodItems.Where(f => mealItem.FoodItemIds.Contains(f.Id)).ToListAsync();
    }

    return Results.Ok(mealItem);
});


app.MapPut("/edit-meal-item/{id}", async (int id, UpdateMealItem dto, AppDbContext context) =>
{
    var mealItem = await context.MealItems.FindAsync(id);
    if (mealItem == null)
        return Results.NotFound($"Meal item with id {id} not found");

    mealItem.FoodItemIds = dto.FoodItemIds;
    mealItem.FoodTypeId = dto.FoodTypeId;

    if (dto.Reminder != null)
    {
        mealItem.Reminder = DateTimeOffset.FromUnixTimeMilliseconds(dto.Reminder).UtcDateTime;
    }
    else
    {
        mealItem.Reminder = null;
    }

    await context.SaveChangesAsync();
    return Results.Ok("Meal item updated");
});



app.MapDelete("/delete-meal-item/{id}", async (int id, AppDbContext context) =>
{
    var mealItem = await context.MealItems.FindAsync(id);
    if (mealItem == null)
        return Results.NotFound($"Meal item with id {id} not found");

    context.MealItems.Remove(mealItem);
    await context.SaveChangesAsync();
    return Results.Ok($"Meal item with id {id} has been deleted");
});

app.MapGet("/get-food-items/{id}", async (int id, AppDbContext context) =>
{
    var foodItem = await context.FoodItems.FindAsync(id);
    if (foodItem == null)
        return Results.NotFound($"Food item with id {id} not found");

    return Results.Ok(foodItem);
});

app.MapGet("/get-food-types", async (AppDbContext context) =>
{
    var foodTypes = await context.FoodTypes.ToListAsync();
    return Results.Ok(foodTypes);
});

app.MapGet("/get-food-types/{id}", async (int id, AppDbContext context) =>
{
    var foodType = await context.FoodTypes.FindAsync(id);
    if (foodType == null)
        return Results.NotFound($"Food type with id {id} not found");

    return Results.Ok(foodType);
});

app.MapPut("/update-food/{id}", async (int id, FoodItem updatedFoodItem, AppDbContext context) =>
{
    var foodItem = await context.FoodItems.FindAsync(id);
    if (foodItem == null)
        return Results.NotFound($"Food item with id {id} not found");

    foodItem.Name = updatedFoodItem.Name;
    foodItem.ImageUrl = updatedFoodItem.ImageUrl;
    foodItem.FoodType = updatedFoodItem.FoodType;
    foodItem.Fat = updatedFoodItem.Fat;
    foodItem.Carbohydrates = updatedFoodItem.Carbohydrates;
    foodItem.Sugar = updatedFoodItem.Sugar;
    foodItem.Cholesterol = updatedFoodItem.Cholesterol;

    await context.SaveChangesAsync();
    return Results.Ok(updatedFoodItem);
});

app.MapDelete("/delete-food/{id}", async (int id, AppDbContext context) =>
{
    var foodItem = await context.FoodItems.FindAsync(id);
    if (foodItem == null)
        return Results.NotFound($"Food item with id {id} not found");

    context.FoodItems.Remove(foodItem);
    await context.SaveChangesAsync();
    return Results.Ok($"Food item with id {id} has been deleted");
});

app.MapGet("/calculate-rating/{id}", async (int id, AppDbContext context) =>
{
    var foodItem = await context.FoodItems.FindAsync(id);
    if (foodItem == null)
        return Results.NotFound($"Food item with id {id} not found");

    var rating = (foodItem.Fat + foodItem.Carbohydrates + foodItem.Sugar + foodItem.Cholesterol) / 4;
    rating = Math.Max(0, Math.Min(100, rating));

    return Results.Ok(new { Rating = rating });
});

app.Run();
