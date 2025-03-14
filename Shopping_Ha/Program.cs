using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shopping_Ha.Areas.Admin.Repository;
using Shopping_Ha.Models;
using Shopping_Ha.Repository;

namespace Shopping_Ha
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
			builder.Services.AddDbContext<DataContext>(options =>
	            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
			builder.Services.AddTransient<IEmailSender, EmailSender>();
			builder.Services.AddDistributedMemoryCache();

            builder.Services.AddSession(option =>
            {
                option.IdleTimeout = TimeSpan.FromMinutes(30);
                option.Cookie.IsEssential = true;
            });


			builder.Services.AddIdentity<AppUserModel,IdentityRole>()
	            .AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders();
			

			builder.Services.Configure<IdentityOptions>(options =>
			{
				// Password settings.
				options.Password.RequireDigit = true;
				options.Password.RequireLowercase = true;
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequireUppercase = false;
				options.Password.RequiredLength = 4;
				
				options.User.RequireUniqueEmail = true;
			});

			var app = builder.Build();


            app.UseStatusCodePagesWithRedirects("/Home/Error?statuscode={0}");
			app.UseSession();
			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

			app.UseAuthentication();//??ng nh?p trc
			app.UseAuthorization();//Ki?m tra quy?n

			app.MapControllerRoute(
			   name: "Areas",
			   pattern: "{area:exists}/{controller=Product}/{action=Index}/{id?}");


            app.MapControllerRoute(
               name: "category",
               pattern: "/category/{Slug?}",
               defaults : new {controller="Category",action= "Index"});

            app.MapControllerRoute(
              name: "brand",
              pattern: "/brand/{Slug?}",
              defaults: new { controller = "Brand", action = "Index" });

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

			
			var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<DataContext>();
            SeedData.SeedingData(context);

            app.Run();
        }
    }
}