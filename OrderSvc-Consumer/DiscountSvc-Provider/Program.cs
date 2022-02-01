using DiscountSvc_Provider.Models;
using DiscountSvc_Provider.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddTransient<DiscountService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    var svc = endpoints.ServiceProvider.GetService<DiscountService>();

    endpoints.MapPost("/discount", async context =>
    {
        var model = await context.Request.ReadFromJsonAsync<DiscountModel>();

        var amount = svc.GetDiscountAmount(model.CustomerRating);

        await context.Response.WriteAsJsonAsync(new DiscountModel
        {
            CustomerRating = model.CustomerRating,
            AmountToDiscount = amount
        });
    });
});

app.Run();
