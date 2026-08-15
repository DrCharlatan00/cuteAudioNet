using cuteAudioNet.APIModels.DTO.Albums;
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
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using cuteAudioNet.Services.Caching;

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

builder.Services.AddStackExchangeRedisCache(options => {
    options.InstanceName = "cuteAudioCache";
    options.Configuration = builder.Configuration.GetConnectionString("RedisMain");
});

builder.Services.AddScoped<ICacheService, RedisCacheService>();

#endregion
builder.Services.AddAutoMapper(cnf => {
    cnf.AddProfile<MappingTracksProfile>();
    cnf.AddProfile<MappingAlbumProfile>();
});

builder.Services.AddScoped<IAlbumsRepository, AlbumsRepository>();
builder.Services.AddScoped<ITracksRepository, TracksRepository>();
builder.Services.AddScoped<IArtistsRepository, ArtistsRepository>();


builder.Services.AddScoped<ITrackService, TrackService>();
builder.Services.AddScoped<IAlbumService, AlbumService>();


builder.Services.AddTransient<IValidator<DTOTrack>, ValidatorTrack>();
builder.Services.AddTransient<IValidator<DTOCreateAlbum>, ValidatorCreateAlbum> ();
builder.Services.AddTransient<IValidator<DTOUpdateAlbum>, ValidatorUpdateAlbum>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

#if DEBUG
foreach (var endpoint in app.Services
    .GetRequiredService<EndpointDataSource>()
    .Endpoints)
{
    Console.WriteLine(endpoint.DisplayName);
}
#endif

app.Run();
