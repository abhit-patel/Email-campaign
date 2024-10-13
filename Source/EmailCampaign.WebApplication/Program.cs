using EmailCampaign.Application.DataMapper;
using EmailCampaign.Core.SharedKernel;
using EmailCampaign.Infrastructure.Data.Context;
using EmailCampaign.Infrastructure.Data.Services;
using EmailCampaign.Query.QueryService;
using EmailCampaign.WebApplication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Environment.IsDevelopment();


builder.Services.AddAppDI(builder.Configuration);

builder.Services.AddSingleton<EmailService>();


// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();
//builder.Services.AddDatabaseDeveloperPageExceptionFilter();


builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options => {
    options.Cookie.Name = "EmailCampaignCookies";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.LoginPath = "/Account/Index";
    options.LogoutPath = "/Account/UserLogout";

});


builder.Services.AddAuthorization(options => {
    options.AddPolicy("RequireAuthenticatedUser", policy => policy.RequireAuthenticatedUser());

    options.AddPolicy("ViewPermission", policy =>
        policy.Requirements.Add(new PermissionRequirement("View")));

    options.AddPolicy("AddEditPermission", policy =>
        policy.Requirements.Add(new PermissionRequirement("AddEdit")));

    options.AddPolicy("DeletePermission", policy =>
        policy.Requirements.Add(new PermissionRequirement("Delete")));


});

builder.Services.AddSession(options => {
    //options.IdleTimeout = TimeSpan.FromHours(24);
    options.Cookie.HttpOnly = true;
});



builder.Services.AddControllersWithViews();
builder.Services.AddControllers();


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
app.UseStaticFiles();

app.UseSession();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<NotificationMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Index}/{id?}");
//app.MapRazorPages();

app.Run();
