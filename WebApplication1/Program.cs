using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

namespace WebApplication1
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                string[] roles = { "Lecturer", "Coordinator" };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }

                // OPTIONAL: Pre-create test users (only runs first time)
                var lecturerEmail = "lecturer@example.com";
                var coordinatorEmail = "coordinator@example.com";
                var password = "Test123!";

                if (await userManager.FindByEmailAsync(lecturerEmail) == null)
                {
                    var lecturer = new IdentityUser { UserName = lecturerEmail, Email = lecturerEmail };
                    var result = await userManager.CreateAsync(lecturer, password);
                    if (result.Succeeded)
                        await userManager.AddToRoleAsync(lecturer, "Lecturer");
                }

                if (await userManager.FindByEmailAsync(coordinatorEmail) == null)
                {
                    var coordinator = new IdentityUser { UserName = coordinatorEmail, Email = coordinatorEmail };
                    var result = await userManager.CreateAsync(coordinator, password);
                    if (result.Succeeded)
                        await userManager.AddToRoleAsync(coordinator, "Coordinator");
                }
            }

            app.Run();
        }
    }
}
