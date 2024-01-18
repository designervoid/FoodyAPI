build_dotnet:
	cat ./secrets.json | dotnet user-secrets set
	dotnet build

run_dotnet:
	dotnet run

build_and_run_dotnet: build_dotnet run_dotnet