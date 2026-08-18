using cuteAudioNet.APIModels.DTO.Tracks;
using cuteAudioNet.APIModels.RDTOModel.Tracks;
using cuteAudioNet.Postgresql.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace cuteAudioNet.IntegrationTests;

public class TestIntTrack : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient client;
    private readonly ITracksRepository _repository;
    private readonly IAlbumsRepository _albums;
    private readonly IArtistsRepository _artist;
    private readonly ITestOutputHelper output;

    public TestIntTrack(TestWebApplicationFactory factory, ITestOutputHelper output)
    {
        client = factory.CreateClient();

        var scope = factory.Services.CreateScope();
        _repository = scope.ServiceProvider.GetRequiredService<ITracksRepository>();
        _albums = scope.ServiceProvider.GetRequiredService<IAlbumsRepository>();
        _artist = scope.ServiceProvider.GetRequiredService<IArtistsRepository>();
        this.output = output;
    }

    [Fact]
    public async Task TestGetTrackCard()
    {
        var result = await client.GetAsync("api/tracks/");

        if (!result.IsSuccessStatusCode) output.WriteLine(result.ReasonPhrase);

        Assert.True(result.IsSuccessStatusCode);

        var data = await result.Content.ReadFromJsonAsync<IEnumerable<RDTOCardTrack>>();

        Assert.NotNull(data);
    }

    [Fact]
    public async Task TestGetTrackFull()
    {
        var result = await client.GetAsync("api/tracks/full");
        if (!result.IsSuccessStatusCode) output.WriteLine(result.ReasonPhrase);


        Assert.True(result.IsSuccessStatusCode);
        var data = await result.Content.ReadFromJsonAsync<IEnumerable<RDTOTrack>>();

        Assert.NotNull(data);
    }

   
    [Fact]
    public async Task CreateTrack()
    {
        var result = await client.PostAsJsonAsync("api/tracks/create", new DTOTrack("Test", Guid.Parse("fe2923bc-c740-4d00-9e67-383320f2ee99"), MusicGenre.ROCK, null, null));
        Assert.True(result.IsSuccessStatusCode, $"Track not create, Code return server: {result.ReasonPhrase}");
    }
    [Fact(Skip = "In prod work")]
    public async Task UpdateTrack()
    {
        var testArtist = await _artist.CreateAsyncDb(new Postgresql.Models.ModelArtistDB
        {
            ArtistName = "TestArtist",
            NickName = "TestNick"
        });
        if (_artist.GetByIdAsyncDb((Guid)testArtist.ID) is null) {
            output.WriteLine("Artist not create, Test stop");
            return;
        }

        var testAlbum = await _albums.CreateAsyncDb(new Postgresql.Models.ModelAlbumDB
        {
            ID = Guid.NewGuid(),
            ArtistID = testArtist.ID!.Value,
            AlbumName = "TestAlbum"
        });

        var testTrack = await _repository.CreateAsyncDb(new Postgresql.Models.ModelTrackDB
        {
            ID = Guid.NewGuid(),
            Name = "OriginalTrack",
            AlbumID = testAlbum.ID!.Value,
            Genre = Postgresql.Models.MusicGenre.ROCK,
            SubArtist = "OriginalFeat",
            TimeRelease = DateTime.Now
        });

        try
        {
            var updateData = new DTOTrack(
                Name: "Updated",
                testAlbum.ID.Value,
                MusicGenre.POP,
                null,
                null
                );

            var response = await client.PutAsJsonAsync($"/api/tracks/{testTrack.ID}", updateData);

            // Assert
            Assert.True(response.IsSuccessStatusCode,
                $"Expected success status code, but got {response.StatusCode}");


        }
        finally
        {
            try
            {

                var TRC = await _repository.GetAllTrackAsyncDb();
                var ArtID = TRC.FirstOrDefault(x => x.ID == testArtist.ID.Value);
                await _repository.RemoveAsyncDb(ArtID.ID);

                await _albums.RemoveAsyncDb(testAlbum.ID!.Value);
                await _artist.RemoveAsyncDb(testArtist.ID!.Value);
            }
            catch
            {
                output.WriteLine("Warn: Not remove test data");
            }
        }
    }

    [Fact]
    public async Task TestGetByName() {
        var data = await client.GetAsync("api/tracks/by-name?name=Track 3");

        if (!data.IsSuccessStatusCode) output.WriteLine(data.ReasonPhrase);

        Assert.True(data.IsSuccessStatusCode);
        var dataRes = await data.Content.ReadFromJsonAsync<IEnumerable<RDTOTrack>>();

        Assert.NotNull(data);
        Assert.Equal("Track 3",dataRes.First().Name);

    }

    [Fact]
    public async Task TestSearchByPagName()
    {
        var data = await client.GetAsync("api/tracks/by-name-pag?name=Track 3&page=1&pageSize=1");
        if (!data.IsSuccessStatusCode) output.WriteLine(data.ReasonPhrase);

        Assert.True(data.IsSuccessStatusCode);
        var dataRes = await data.Content.ReadFromJsonAsync<IEnumerable<RDTOCardTrack>>();

        Assert.NotNull(dataRes);
        Assert.Single(dataRes);
        Assert.Equal("Track 3", dataRes.First().Name);
        


    }
}
