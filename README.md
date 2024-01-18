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