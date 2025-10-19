Photography Portfolio - ASP.NET Core MVC (net8.0)
------------------------------------------------

What's included:
- ASP.NET Core MVC project (Program.cs, Controllers, Models, Views)
- EF Core (SQL Server) code-first with ApplicationDbContext
- Simple session-based admin login (username: admin, password: password123)
- Uploads stored in wwwroot/uploads
- Demo seed data and placeholder images

Setup:
1. Install .NET 8 SDK.
2. Open the folder in Visual Studio or VS Code.
3. (Optional) Edit appsettings.json connection string to your SQL Server.
   Default uses LocalDB: "Server=(localdb)\\mssqllocaldb;Database=PhotographyPortfolioDb;Trusted_Connection=True;"
4. Run:
   dotnet restore
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   dotnet run

Admin:
- Go to /Admin/Login and use username: admin, password: password123
- After first run, change the admin password in AdminController or implement Identity.

Enjoy!
