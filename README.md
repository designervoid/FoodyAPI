# Prepare

`.env.local.dev`

```
POSTGRES_USER=usr
POSTGRES_PASSWORD=pswd
POSTGRES_DB=db
```

`secrets.json`

```
{
    "pgConnection": "Host=localhost; Port=5432; Database=db; Username=usr; Password=pswd"
}
```

Go to terminal:

```
psql -h <hostname> -p <port> -U <usr> -d <db>
```

Create Tables (because currently migrations not integrated):
```sql
CREATE TABLE FoodItems (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(255),
    ImageUrl TEXT,
    FoodType INT CHECK (FoodType IN (1, 2, 3)),
    Fat DECIMAL(5,2) CHECK (Fat BETWEEN 0 AND 99),
    Carbohydrates DECIMAL(5,2) CHECK (Carbohydrates BETWEEN 0 AND 99),
    Sugar DECIMAL(5,2) CHECK (Sugar BETWEEN 0 AND 99),
    Cholesterol DECIMAL(3,2) CHECK (Cholesterol BETWEEN 0 AND 1)
);
```

```sql
CREATE TABLE FoodTypes (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Description TEXT
);
```

```sql
ALTER TABLE FoodItems
ADD CONSTRAINT fk_FoodItems_FoodType FOREIGN KEY (FoodType)
REFERENCES FoodTypes (Id);
```

```sql
INSERT INTO FoodTypes (Name, Description)
VALUES
    ('Energy-giving foods', 'These foods are high in carbohydrates and fats, which provide the body with energy. Examples of energy-giving foods include bread, rice, pasta, potatoes, fruits, vegetables, nuts, and seeds.'),
    ('Body-building foods', 'These foods are high in protein, which is essential for building and repairing muscle tissue. Examples of body-building foods include meat, poultry, fish, eggs, dairy products, legumes, nuts, and seeds.'),
    ('Protective foods', 'These foods are high in vitamins, minerals, and antioxidants, which help to protect the body from disease. Examples of protective foods include fruits, vegetables, and whole grains.');
```

```sql
UPDATE FoodTypes SET
Name = 'Breakfast',
Description = 'The first meal of the day, usually eaten in the morning. Typically includes a variety of foods such as eggs, bread, cereal, and sometimes fruits.'
WHERE id = 1;

UPDATE FoodTypes SET
Name = 'Lunch',
Description = 'A meal eaten in the middle of the day, typically one that is lighter or less formal than an evening meal.'
WHERE id = 2;

UPDATE FoodTypes SET
Name = 'Dinner',
Description = 'The main meal of the day, eaten either in the evening or at midday.'
WHERE id = 3;
```

```sql
CREATE TABLE MealItems (
    Id SERIAL PRIMARY KEY,
    FoodItemIds INT[],
    Reminder TIMESTAMP
);
```

```sql
ALTER TABLE MealItems
ADD COLUMN FoodTypeId INT,
ADD CONSTRAINT fk_mealitems_foodtypeid FOREIGN KEY (FoodTypeId)
REFERENCES FoodTypes (Id);
```

Then can build!

# Build

```sh
dotnet user-secrets init

cat ./secrets.json | dotnet user-secrets set

dotnet build
```

# Run

## PostgreSQL

```sh
docker-compose -f docker-compose.dev.yml up --build -d
```

## Dotnet

```sh
dotnet run
```

# TODO

1. ~~docker-compose pgsql~~
2. ~~pgsql pool connection~~ (by defaut thx dotnet framework)
3. ~~endpoints~~
4. migrations
5. openapi, swagger
6. unit, int testing

# FAQ

## DotNet User Secrets List

`dotnet user-secrets list`

## DotNet Clear Secrets List

`dotnet user-secrets clear`

## DotNet Add Package Command

`dotnet add package`