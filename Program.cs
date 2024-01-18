using DataSourceFactory;
using Types;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var connectionString = builder.Configuration["pgConnection"];
await using var dataSource = DatabaseConnectionRepository.GetDataSource(builder);

app.MapPost("/add-food", async (FoodItem foodItem) => {
    string insertQuery = "INSERT INTO FoodItems (FoodType, Fat, Carbohydrates, Sugar, Cholesterol) VALUES (@FoodType, @Fat, @Carbohydrates, @Sugar, @Cholesterol)";

    await using (var cmd = dataSource.CreateCommand(insertQuery))
    {
        cmd.Parameters.AddWithValue("@FoodType", (int)foodItem.FoodType);
        cmd.Parameters.AddWithValue("@Fat", foodItem.Fat);
        cmd.Parameters.AddWithValue("@Carbohydrates", foodItem.Carbohydrates);
        cmd.Parameters.AddWithValue("@Sugar", foodItem.Sugar);
        cmd.Parameters.AddWithValue("@Cholesterol", foodItem.Cholesterol);
        await cmd.ExecuteNonQueryAsync();
    }

    return Results.Ok("Food added");
});

app.MapGet("/get-food-items", async () => {
    List<FoodItem> foodItems = [];
    await using (var cmd = dataSource.CreateCommand("SELECT * FROM FoodItems"))
    await using (var reader = await cmd.ExecuteReaderAsync())
    {
        while (await reader.ReadAsync())
        {
            foodItems.Add(new FoodItem
            {
                Id = reader.GetInt32(0),
                Name = reader.IsDBNull(1) ? null : reader.GetString(1),
                ImageUrl = reader.IsDBNull(2) ? null : reader.GetString(2),
                FoodType = reader.GetInt32(3),
                Fat = reader.GetDouble(4),
                Carbohydrates = reader.GetDouble(5),
                Sugar = reader.GetDouble(6),
                Cholesterol = reader.GetDouble(7)
            });
        }
    }
    return Results.Ok(foodItems);
});

app.MapGet("/get-food-items/{id}", async (int id) => {
    await using (var cmd = dataSource.CreateCommand("SELECT * FROM FoodItems WHERE Id = @Id"))
    {
        cmd.Parameters.AddWithValue("@Id", id);

        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            if (await reader.ReadAsync())
            {
                var foodItem = new FoodItem
                {
                    Id = reader.GetInt32(0),
                    Name = reader.IsDBNull(1) ? null : reader.GetString(1),
                    ImageUrl = reader.IsDBNull(2) ? null : reader.GetString(2),
                    FoodType = reader.GetInt32(3),
                    Fat = reader.GetDouble(4),
                    Carbohydrates = reader.GetDouble(5),
                    Sugar = reader.GetDouble(6),
                    Cholesterol = reader.GetDouble(7)
                };
                return Results.Ok(foodItem);
            }
        }
    }
    return Results.NotFound($"Food item with id {id} not found");
});

app.MapGet("/get-food-types", async () => {
    List<FoodTypes> foodTypes = [];
    await using (var cmd = dataSource.CreateCommand("SELECT * FROM FoodTypes"))
    await using (var reader = await cmd.ExecuteReaderAsync())
    {
        while (await reader.ReadAsync())
        {
            foodTypes.Add(new FoodTypes
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.IsDBNull(2) ? null : reader.GetString(2)
            });
        }
    }
    return Results.Ok(foodTypes);
});

app.MapGet("/get-food-types/{id}", async (int id) => {
    await using (var cmd = dataSource.CreateCommand("SELECT * FROM FoodTypes WHERE Id = @Id"))
    {
        cmd.Parameters.AddWithValue("@Id", id);

        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            if (await reader.ReadAsync())
            {
                var foodItem = new FoodTypes
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2)
                };
                return Results.Ok(foodItem);
            }
        }
    }
    return Results.NotFound($"Food item with id {id} not found");
});

app.MapPut("/update-food/{id}", async (int id, FoodItem updatedFoodItem) => {
    var cmd = dataSource.CreateCommand($"SELECT COUNT(*) FROM FoodItems WHERE Id = {id}");
    var count = (long)await cmd.ExecuteScalarAsync();
    if (count == 0)
    {
        return Results.NotFound($"Food item with id {id} not found");
    }

    cmd = dataSource.CreateCommand($@"
        UPDATE FoodItems SET
        Name = '{updatedFoodItem.Name}',
        ImageUrl = '{updatedFoodItem.ImageUrl}',
        FoodType = {(int)updatedFoodItem.FoodType},
        Fat = {updatedFoodItem.Fat},
        Carbohydrates = {updatedFoodItem.Carbohydrates},
        Sugar = {updatedFoodItem.Sugar},
        Cholesterol = {updatedFoodItem.Cholesterol}
        WHERE Id = {id}
    ");
    await cmd.ExecuteNonQueryAsync();

    return Results.Ok(updatedFoodItem);
});

app.MapDelete("/delete-food/{id}", async (int id) => {
    var cmd = dataSource.CreateCommand($"SELECT COUNT(*) FROM FoodItems WHERE Id = {id}");
    var count = (long)await cmd.ExecuteScalarAsync();
    if (count == 0)
    {
        return Results.NotFound($"Food item with id {id} not found");
    }

    cmd = dataSource.CreateCommand($"DELETE FROM FoodItems WHERE Id = {id}");
    await cmd.ExecuteNonQueryAsync();

    return Results.Ok($"Food item with id {id} has been deleted");
});

app.MapGet("/calculate-rating/{id}", async (int id) => {
    var cmd = dataSource.CreateCommand($"SELECT * FROM FoodItems WHERE Id = {id}");
    await using (var reader = await cmd.ExecuteReaderAsync())
    {
        if (await reader.ReadAsync())
        {
            var fat = reader.GetDouble(4);
            var carbohydrates = reader.GetDouble(5);
            var sugar = reader.GetDouble(6);
            var cholesterol = reader.GetDouble(7);

            var rating = (fat + carbohydrates + sugar + cholesterol) / 4;

            rating = Math.Max(0, Math.Min(100, rating));

            return Results.Ok(new { Rating = rating });
        }
    }

    return Results.NotFound($"Food item with id {id} not found");
});

app.Run();
