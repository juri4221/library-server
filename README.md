This project uses .Net Core7 Entity Framework 5.

1. Edit ```appsettings.Development.json``` database credentials.
2. Run ```dotnet ef database update``` to run migrations.
3. Migrations will add an admin with username:```admin``` and password:```Admin123+```
4. Create a directory called ```wwwroot``` inside the ```BookManagement``` directory.
5. Run ```cd BookManagement/ && dotnet run```
