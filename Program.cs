using DataSourceFactory;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var connectionString = builder.Configuration["pgConnection"];
await using var dataSource = DatabaseConnectionRepository.GetDataSource(builder);

app.MapGet("/", () => "Hello World!");

// FoodItem foodItem
app.MapPost("/add-food", async () => {
    // Добавить проверку валидности входных данных

    // Сохранить foodItem в базу данных
    // Возвращать результат операции (успех или ошибка)

    // Insert some data
    await using (var cmd = dataSource.CreateCommand("INSERT INTO data (some_field) VALUES ($1)"))
    {
        cmd.Parameters.AddWithValue("Hello world");
        await cmd.ExecuteNonQueryAsync();
    }

    // Retrieve all rows
    await using (var cmd = dataSource.CreateCommand("SELECT some_field FROM data"))
    await using (var reader = await cmd.ExecuteReaderAsync())
    {
        while (await reader.ReadAsync())
        {
            Console.WriteLine(reader.GetString(0));
        }
    }
    Results.Ok();
});

app.MapGet("/get-food-items", () => {
    // Извлечь все продукты из базы данных
    // Вернуть список продуктов
});

// int id, FoodItem updatedFoodItem
app.MapPut("/update-food/{id}", () => {
    // Проверить, существует ли продукт с данным id
    // Обновить информацию о продукте
    // Вернуть обновленный продукт или сообщение об ошибке
});

app.MapDelete("/delete-food/{id}", (int id) => {
    // Проверить, существует ли продукт с данным id
    // Удалить продукт из базы данных
    // Вернуть результат операции (успех или ошибка)
});

app.MapGet("/calculate-rating/{id}", (int id) => {
    // Найти продукт с данным id
    // Вычислить рейтинг согласно формуле
    // Ограничить рейтинг значениями от 0 до 100
    // Вернуть рейтинг
});

app.Run();
