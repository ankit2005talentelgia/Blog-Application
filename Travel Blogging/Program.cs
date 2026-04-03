using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using Travel_Blogging.Data;
using Travel_Blogging.Models;
using Travel_Blogging.Repositories.Implementations;
using Travel_Blogging.Repositories.Interfaces;
using Travel_Blogging.Services.Implementations;
using Travel_Blogging.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// add the toastify for notification
builder.Services.AddMvc().AddNToastNotifyToastr(new ToastrOptions()
{
    ProgressBar = false,
    PositionClass = ToastPositions.TopCenter
});

// connect with db
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


// this is for mail configuration so that mail confirmation is startes
builder.Services.AddScoped<IEmailService, EmailService>();

// add dependency injection
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();

// add dependency injection for services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IReviewService, ReviewService>();


// Add the cookie authentication middleware for authenticating the user
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/user/login";  // redire to login page if user is not authenticated
        options.LogoutPath = "/user/logout"; // redirect after user is loggedout
        options.ExpireTimeSpan = TimeSpan.FromHours(24);  // this is the expiry time for cookie
        options.SlidingExpiration = true;   // extend expiry time if user do some activity on the website (auto reset the time)
    });

// add the http context accessor for using httpcontext for creating the cookie by signinasync method
builder.Services.AddHttpContextAccessor();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    
}

app.Use(async (context, next) =>
{
    context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
    context.Response.Headers["Pragma"] = "no-cache";
    context.Response.Headers["Expires"] = "0";

    await next();
});

// add the ntoast notify
app.UseNToastNotify();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// add authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();



//app.Use(async (context, next) =>
//{
//    context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
//    context.Response.Headers["Pragma"] = "no-cache";
//    context.Response.Headers["Expires"] = "0";

//    await next();
//});