using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using HarrysPizza.Data;
using Microsoft.AspNetCore.Identity;
using HarrysPizza.Models;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<HarrysPizzaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("HarrysPizzaContext") ?? throw new InvalidOperationException("Connection string 'HarrysPizzaContext' not found.")));

builder.Services.AddDefaultIdentity<HarrysPizzaUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<HarrysPizzaContext>();

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    //.AddEntityFrameworkStores<HarrysPizzaContext>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<HarrysPizzaUser, IdentityRole>(

    options =>
    {
        options.Stores.MaxLengthForKeys = 128;
    })
        
.AddEntityFrameworkStores<HarrysPizzaContext>()
.AddRoles<IdentityRole>()
.AddDefaultUI()
.AddDefaultTokenProviders();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdmins", policy => policy.RequireRole("Admin"));
});
builder.Services.AddRazorPages()
    .AddRazorPagesOptions(options =>
    {
        options.Conventions.AuthorizeFolder("/Admin", "RequireAdmins");
    });

var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<HarrysPizzaContext>();
    context.Database.EnsureCreated();
    //DBInitializer.Initialize(context);
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();;

app.UseAuthorization();

app.MapRazorPages();

using(var scope = app.Services.CreateScope()) 
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<HarrysPizzaContext>();
    context.Database.Migrate();
    var userMgr = services.GetRequiredService<UserManager<HarrysPizzaUser>>();
    var roleMgr = services.GetRequiredService<RoleManager<IdentityRole>>();
    IdentitySeedData.Initialize(context,userMgr, roleMgr).Wait();  
}


app.Run();
