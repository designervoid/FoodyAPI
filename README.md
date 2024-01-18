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

1. docker-compose pgsql
1. pgsql pool connection
2. migrations
4. openapi, swagger
5. unit, int testing

# FAQ

## DotNet User Secrets List

`dotnet user-secrets list`

## DotNet Clear Secrets List

`dotnet user-secrets clear`

## DotNet Add Package Command

`dotnet add package`