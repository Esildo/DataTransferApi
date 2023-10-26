```
// Initial setup
docker-compose build
docker-compose up

// Migrations
docker-compose exec app dotnet ef database update
```