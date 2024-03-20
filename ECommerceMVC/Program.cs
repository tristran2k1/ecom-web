using ECommerceMVC.Data;
using ECommerceMVC.Helpers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddTransient<MiddlewareLog>();
// app.UseMiddleware<MiddlewareLog>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<Hshop2023Context>(options => {
	options.UseSqlServer(builder.Configuration.GetConnectionString("HShop"));
});

builder.Services.AddDistributedMemoryCache();  // Thêm dịch vụ dùng bộ nhớ lưu cache (session sử dụng dịch vụ này)

builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(10);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});

// https://docs.automapper.org/en/stable/Dependency-injection.html
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();


// Thêm StaticFileMiddleware - nếu Request là yêu cầu truy cập file tĩnh,
// Nó trả ngay về Response nội dung file và là điểm cuối pipeline, nếu  khác
// nó gọi  Middleware phía sau trong Pipeline
app.UseStaticFiles(); // allow use of static files : wwwroot

// Thêm EndpointRoutingMiddleware: ánh xạ Request gọi đến Endpoint (Middleware cuối) phù hợp định nghĩa bởi EndpointMiddleware
app.UseRouting();


// Thêm SessionMiddleware:  khôi phục, thiết lập - tạo ra session
// gán context.Session, sau đó chuyển gọi ngay middleware tiếp trong pipeline
app.UseSession(); // Thêm  dịch vụ Session, dịch vụ này cunng cấp Middleware: 

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

// ======= app.UseEndpoint dùng để xây dựng các endpoint - điểm cuối  của pipeline theo Url truy cập
// app.UseEndpoints(endpoints =>
// {

//     // EndPoint(2) khi truy vấn đến /Testpost với phương thức post hoặc put
//     endpoints.MapMethods("/Testpost" , new string[] {"post", "put"}, async context => {
//         await context.Response.WriteAsync("post/pust");
//     });

//     //  EndPoint(2) -  Middleware khi truy cập /Home với phương thức GET - nó làm Middleware cuối Pipeline
//     endpoints.MapGet("/Home", async context => {

//         int? count  = context.Session.GetInt32("count");
//         count = (count != null) ? count + 1 : 1;
//         context.Session.SetInt32("count", count.Value);
//         await context.Response.WriteAsync($"Home page! {count}");

//     });
// });
// ======>>>>>> truy cập địa chỉ /Home (http://localhost:5000/Home) 
// ======>>>>>> thì HttpContext đi qua các Middleware: StaticFileMiddleware, SessionMiddleware,EndpointMiddleware, EndPoint(2)


// app.run -> 1 endpoint 
app.Run();



// use custopm middleware
// app.UseMiddleware<CheckAcessMiddleware>();
