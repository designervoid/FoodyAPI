using DataSourceFactory;
using Npgsql;
using Types;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var connectionString = builder.Configuration["pgConnection"];
await using var dataSource = DatabaseConnectionRepository.GetDataSource(builder);

app.MapPost("/add-food", async (FoodItem foodItem) => {
    string insertQuery = "INSERT INTO FoodItems (Name, ImageUrl, FoodType, Fat, Carbohydrates, Sugar, Cholesterol) VALUES (@Name, @ImageUrl, @FoodType, @Fat, @Carbohydrates, @Sugar, @Cholesterol)";

    await using (var cmd = dataSource.CreateCommand(insertQuery))
    {
        cmd.Parameters.AddWithValue("@Name", foodItem.Name ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@ImageUrl", foodItem.ImageUrl ?? (object)DBNull.Value);
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

app.MapPost("/add-meal-item", async (CreateMealItemDto dto) => {
    string insertQuery = "INSERT INTO MealItems (FoodItemIds, Reminder, FoodTypeId) VALUES (@FoodItemIds, @Reminder, @FoodTypeId)";

    await using (var cmd = dataSource.CreateCommand(insertQuery))
    {
        cmd.Parameters.Add(new NpgsqlParameter("FoodItemIds", NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Integer)
        {
            Value = dto.FoodItemIds ?? Array.Empty<int>()
        });

        cmd.Parameters.AddWithValue("@FoodTypeId", dto.FoodTypeId);

        DateTime? parsedReminder = null;
        if (!string.IsNullOrEmpty(dto.Reminder) && DateTime.TryParse(dto.Reminder, out DateTime reminder))
        {
            parsedReminder = reminder;
        }

        cmd.Parameters.AddWithValue("@Reminder", parsedReminder.HasValue ? (object)parsedReminder.Value : DBNull.Value);
        await cmd.ExecuteNonQueryAsync();
    }

    return Results.Ok("Meal item created");
});

app.MapGet("/get-meal-items", async () => {
    List<MealItem> mealItems = [];
    await using (var cmd = dataSource.CreateCommand("SELECT * FROM MealItems"))
    await using (var reader = await cmd.ExecuteReaderAsync())
    {
        while (await reader.ReadAsync())
        {
            var mealItem = new MealItem
            {
                Id = reader.GetInt32(0),
                FoodItemIds = reader.IsDBNull(1) ? Array.Empty<int>() : (int[])reader.GetValue(1),
                Reminder = reader.IsDBNull(2) ? (DateTime?)null : reader.GetDateTime(2),
                FoodTypeId = reader.GetInt32(3),
                FoodItems = []
            };

            var foodTypeCmd = dataSource.CreateCommand("SELECT Name, Description FROM FoodTypes WHERE Id = @FoodTypeId");
            foodTypeCmd.Parameters.AddWithValue("@FoodTypeId", mealItem.FoodTypeId);
            await using (var foodTypeReader = await foodTypeCmd.ExecuteReaderAsync())
            {
                if (await foodTypeReader.ReadAsync())
                {
                    mealItem.FoodType = new FoodType
                    {
                        Id = mealItem.FoodTypeId,
                        Name = foodTypeReader.GetString(0),
                        Description = foodTypeReader.GetString(1)
                    };
                }
            }

            foreach (var foodItemId in mealItem.FoodItemIds)
            {
                var foodItemCmd = dataSource.CreateCommand("SELECT * FROM FoodItems WHERE Id = @FoodItemId");
                foodItemCmd.Parameters.AddWithValue("@FoodItemId", foodItemId);
                await using var foodItemReader = await foodItemCmd.ExecuteReaderAsync();
                while (await foodItemReader.ReadAsync())
                {
                    mealItem.FoodItems.Add(new FoodItem
                    {
                        Id = foodItemReader.GetInt32(0),
                        Name = foodItemReader.IsDBNull(1) ? null : foodItemReader.GetString(1),
                        ImageUrl = foodItemReader.IsDBNull(2) ? null : foodItemReader.GetString(2),
                        FoodType = foodItemReader.GetInt32(3),
                        Fat = foodItemReader.GetDouble(4),
                        Carbohydrates = foodItemReader.GetDouble(5),
                        Sugar = foodItemReader.GetDouble(6),
                        Cholesterol = foodItemReader.GetDouble(7)
                    });
                }
            }

            mealItems.Add(mealItem);
        }
    }
    return Results.Ok(mealItems);
});

app.MapGet("/get-meal-item/{id}", async (int id) => {
    MealItem? mealItem = null;
    await using (var cmd = dataSource.CreateCommand("SELECT * FROM MealItems WHERE Id = @Id"))
    {
        cmd.Parameters.AddWithValue("@Id", id);
        await using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            mealItem = new MealItem
            {
                Id = reader.GetInt32(0),
                FoodItemIds = reader.IsDBNull(1) ? Array.Empty<int>() : (int[])reader.GetValue(1),
                Reminder = reader.IsDBNull(2) ? (DateTime?)null : reader.GetDateTime(2),
                FoodTypeId = reader.GetInt32(3),
                FoodItems = []
            };

            var foodTypeCmd = dataSource.CreateCommand("SELECT Name, Description FROM FoodTypes WHERE Id = @FoodTypeId");
            foodTypeCmd.Parameters.AddWithValue("@FoodTypeId", mealItem.FoodTypeId);
            await using (var foodTypeReader = await foodTypeCmd.ExecuteReaderAsync())
            {
                if (await foodTypeReader.ReadAsync())
                {
                    mealItem.FoodType = new FoodType
                    {
                        Id = mealItem.FoodTypeId,
                        Name = foodTypeReader.GetString(0),
                        Description = foodTypeReader.GetString(1)
                    };
                }
            }

            foreach (var foodItemId in mealItem.FoodItemIds)
            {
                var foodItemCmd = dataSource.CreateCommand("SELECT * FROM FoodItems WHERE Id = @FoodItemId");
                foodItemCmd.Parameters.AddWithValue("@FoodItemId", foodItemId);
                await using var foodItemReader = await foodItemCmd.ExecuteReaderAsync();
                while (await foodItemReader.ReadAsync())
                {
                    mealItem.FoodItems.Add(new FoodItem
                    {
                        Id = foodItemReader.GetInt32(0),
                        Name = foodItemReader.IsDBNull(1) ? null : foodItemReader.GetString(1),
                        ImageUrl = foodItemReader.IsDBNull(2) ? null : foodItemReader.GetString(2),
                        FoodType = foodItemReader.GetInt32(3),
                        Fat = foodItemReader.GetDouble(4),
                        Carbohydrates = foodItemReader.GetDouble(5),
                        Sugar = foodItemReader.GetDouble(6),
                        Cholesterol = foodItemReader.GetDouble(7)
                    });
                }
            }
        }
    }

    if (mealItem != null)
    {
        return Results.Ok(mealItem);
    }
    else
    {
        return Results.NotFound($"Meal item with id {id} not found");
    }
});

app.MapPut("/edit-meal-item/{id}", async (int id, UpdateMealItemDto dto) => {
    string updateQuery = "UPDATE MealItems SET FoodItemIds = @FoodItemIds, Reminder = @Reminder, FoodTypeId = @FoodTypeId WHERE Id = @Id";

    await using var cmd = dataSource.CreateCommand(updateQuery);
    cmd.Parameters.Add(new NpgsqlParameter("FoodItemIds", NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Integer)
    {
        Value = dto.FoodItemIds ?? Array.Empty<int>()
    });

    cmd.Parameters.AddWithValue("@FoodTypeId", dto.FoodTypeId); // Handle the new FoodTypeId

    DateTime? parsedReminder = null;
    if (!string.IsNullOrEmpty(dto.Reminder) && DateTime.TryParse(dto.Reminder, out DateTime reminder))
    {
        parsedReminder = reminder;
    }

    cmd.Parameters.AddWithValue("@Reminder", parsedReminder.HasValue ? (object)parsedReminder.Value : DBNull.Value);
    cmd.Parameters.AddWithValue("@Id", id);
    var result = await cmd.ExecuteNonQueryAsync();

    if (result == 0)
        return Results.NotFound($"Meal item with id {id} not found");

    return Results.Ok("Meal item updated");
});

app.MapDelete("/delete-meal-item/{id}", async (int id) => {
    string deleteQuery = "DELETE FROM MealItems WHERE Id = @Id";

    await using var cmd = dataSource.CreateCommand(deleteQuery);
    cmd.Parameters.AddWithValue("@Id", id);
    var result = await cmd.ExecuteNonQueryAsync();

    if (result == 0)
        return Results.NotFound($"Meal item with id {id} not found");

    return Results.Ok($"Meal item with id {id} has been deleted");
});

app.MapGet("/get-food-items/{id}", async (int id) => {
    await using (var cmd = dataSource.CreateCommand("SELECT * FROM FoodItems WHERE Id = @Id"))
    {
        cmd.Parameters.AddWithValue("@Id", id);

        await using var reader = await cmd.ExecuteReaderAsync();
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

        await using var reader = await cmd.ExecuteReaderAsync();
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
