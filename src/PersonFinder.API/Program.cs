using PersonFinder.Application.Features.Persons.Commands;
using PersonFinder.Application.Features.Persons.Queries;
using PersonFinder.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddInfrastructure(builder.Configuration);

// Application layer — command handlers
builder.Services.AddScoped<CreatePersonCommandHandler>();
builder.Services.AddScoped<UpdatePersonLocationCommandHandler>();
builder.Services.AddScoped<GetPersonByIdQueryHandler>();
builder.Services.AddScoped<GetNearbyPersonsQueryHandler>();

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

app.Run();
