using InnoAppointmentsApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDatabaseInfrastructure(builder.Configuration);

builder.Services.AddApplicationLogic(builder.Configuration);

builder.Services.AddApiSecurity(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddSwaggerDocs(builder.Environment.ApplicationName);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();