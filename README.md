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

Go to terminal and run migrations:

1. Run migrations
`dotnet ef database update`

2. Set data

```sql
INSERT INTO public."FoodTypes"(
	"Name", "Description")
	VALUES ('Breakfast', 'The first meal of the day, usually eaten in the morning. Typically includes a variety of foods such as eggs, bread, cereal, and sometimes fruits.'),
    ('Lunch', 'A meal eaten in the middle of the day, typically one that is lighter or less formal than an evening meal.'),
    ('Dinner', 'The main meal of the day, eaten either in the evening or at midday.')

INSERT INTO public."FoodItems"(
	"Name", "ImageUrl", "FoodType", "Fat", "Carbohydrates", "Sugar", "Cholesterol")
	VALUES 'Random Food2', 'http://example.com/image.jpg', 1, 20.5, 50.2, 10.5, 0.5

UPDATE public."FoodItems"
SET "ImageUrl" = 'https://2dxz44zd-5069.euw.devtunnels.ms/StaticFiles/images/melon.png'
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
4. ~~migrations~~
5. openapi, swagger
6. unit, int testing

# FAQ

## DotNet User Secrets List

`dotnet user-secrets list`

## DotNet Clear Secrets List

`dotnet user-secrets clear`

## DotNet Add Package Command

`dotnet add package`

## DotNet Create PostgreSQL Migration

`dotnet ef migrations add SomeMigration`

## DotNet Apply PostgreSQL Migrations

`dotnet ef database update`

## Postman Docs

[Link](https://documenter.getpostman.com/view/14601124/2s9YynjPNy)