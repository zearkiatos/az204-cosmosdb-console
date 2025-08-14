function build {
	dotnet build az204-cosmosdb-console.sln
}

function run {
	dotnet run
}

function docker-up {
	docker-compose up --build
}

function docker-down {
	docker-compose down
}

function podman-up {
	podman-compose up --build
}

function podman-down {
	podman-compose down
}