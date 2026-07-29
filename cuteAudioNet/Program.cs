using cuteAudioNet.APIModels.DTO.Tracks;
using cuteAudioNet.APIModels.Mapping;
using cuteAudioNet.APIModels.Validators;
using cuteAudioNet.Middlewares;
using cuteAudioNet.Postgresql;
using cuteAudioNet.Postgresql.Repositories;
using cuteAudioNet.Postgresql.Repositories.Interfaces;
using cuteAudioNet.Services;
using cuteAudioNet.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Hosting.Builder;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

#region DB
builder.Services.AddDbContext<PgContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultDB") ?? throw new ArgumentNullException("Connection string is null ??"));
});


#endregion
builder.Services.AddAutoMapper(cnf => {
    cnf.AddProfile<MappingTracksProfile>();
});

builder.Services.AddScoped<IAlbumsRepository, AlbumsRepository>();
builder.Services.AddScoped<ITracksRepository, TracksRepository>();
builder.Services.AddScoped<IArtistsRepository, ArtistsRepository>();


builder.Services.AddScoped<ITrackService, TrackService>();


builder.Services.AddTransient<IValidator<DTOTrack>, ValidatorTrack>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

app.Run();
