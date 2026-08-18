using cuteAudioNet.APIModels.DTO.Artists;
using cuteAudioNet.APIModels.RDTOModel.Artists;
using cuteAudioNet.Postgresql.Models;
using cuteAudioNet.Postgresql.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace cuteAudioNet.IntegrationTests;

public class TestsIntArtists : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient httpClient;
    private readonly ITestOutputHelper output;
    IArtistsRepository artistsRepository;

    public TestsIntArtists(TestWebApplicationFactory factory, ITestOutputHelper output)
    {
        var scope = factory.Services.CreateScope();

        IArtistsRepository artistsRepository = scope.ServiceProvider.GetRequiredService<IArtistsRepository>();
        this.artistsRepository = artistsRepository;
        httpClient = factory.CreateClient();

        this.output = output;
    }

    [Fact]
    public async Task TestGetCard() {
        var result = await httpClient.GetAsync("api/artist");
        if (!result.IsSuccessStatusCode) output.WriteLine(result.ReasonPhrase);

        Assert.True(result.IsSuccessStatusCode);
        var data = await result.Content.ReadFromJsonAsync<IEnumerable<RDTOArtistCard>>();

        Assert.NotNull(data);

    }

    [Fact]
    public async Task TestGetFull()
    {
        var result = await httpClient.GetAsync("api/artist/full");
        if (!result.IsSuccessStatusCode) output.WriteLine(result.ReasonPhrase);

        Assert.True(result.IsSuccessStatusCode);
        var data = await result.Content.ReadFromJsonAsync<IEnumerable<RDTOArtist>>();

        Assert.NotNull(data);

    }


    [Fact]
    public async Task TestGetInfo()
    {
        var result = await httpClient.GetAsync("api/artist/info");
        if (!result.IsSuccessStatusCode) output.WriteLine(result.ReasonPhrase);

        Assert.True(result.IsSuccessStatusCode);
        var data = await result.Content.ReadFromJsonAsync<IEnumerable<RDTOOnlyArtistInfo>>();

        Assert.NotNull(data);

    }

    [Fact]
    public async Task TestGetPag()
    {
        var result = await httpClient.GetAsync("api/artist/pag?page=1&pageSize=1");
        if (!result.IsSuccessStatusCode) output.WriteLine(result.ReasonPhrase);

        Assert.True(result.IsSuccessStatusCode);
        var resultData = await result.Content.ReadFromJsonAsync<IEnumerable<RDTOArtistCard>>();

        Assert.NotNull(resultData);
        Assert.Single(resultData);

    }

    [Fact]
    public async Task TestSearchByName() {
        var result = await httpClient.GetAsync("api/artist/by-name?name=Zanfords");
        if (!result.IsSuccessStatusCode) output.WriteLine(result.ReasonPhrase);

        Assert.True(result.IsSuccessStatusCode);

        var resultData = await result.Content.ReadFromJsonAsync<IEnumerable<RDTOArtist>>();

        Assert.NotNull(resultData);
        Assert.Equal("Zanfords", resultData.First().NickName);
    }
    [Fact]
    public async Task TestSearchByNameWithPag()
    {
        var result = await httpClient.GetAsync("api/artist/by-name?name=Zanfords&page=1pageSize=1");
        if (!result.IsSuccessStatusCode) output.WriteLine(result.ReasonPhrase);

        Assert.True(result.IsSuccessStatusCode);

        var resultData = await result.Content.ReadFromJsonAsync<IEnumerable<RDTOArtistCard>>();

        Assert.NotNull(resultData);
        Assert.Single(resultData);
        Assert.Equal("Zanfords", resultData.First().NickName);
    }


    [Fact]
    public async Task TestCreateItem() {
        DTOArtist testData = new DTOArtist(
            Name: "Test",
            NickName: "Test",
            null,
            null,
            null
            );

        try
        {
            var result = await httpClient.PostAsJsonAsync("api/artist/", testData);
            if (!result.IsSuccessStatusCode) output.WriteLine(result.ReasonPhrase);

            Assert.True(result.IsSuccessStatusCode);

        }
        catch (Exception ex)
        {
            output.WriteLine(ex.Message);
            Assert.Fail();
        }
     
        
        
    }

    [Fact]
    public async Task TestUpdate() {
        ModelArtistDB model = new ModelArtistDB {
            ArtistName = "Test",
            NickName = "Test",
            ID = Guid.NewGuid()
        };

        try
        {
            var create = await artistsRepository.CreateAsyncDb(model);
            model.ID = (Guid)create.ID;
        }
        catch {
            output.WriteLine("Test abort, Can't create item for check");
            Assert.Fail();
        }
        var updateModel = new DTOArtist
        (
                Name: "Test",
                NickName: "Update test",
                null,
                null,
                null
        );
        var result = await httpClient.PutAsJsonAsync($"api/artist/{model.ID}",updateModel);
        if (!result.IsSuccessStatusCode) output.WriteLine(result.ReasonPhrase);
        Assert.True(result.IsSuccessStatusCode);

        var dataResult =  await result.Content.ReadFromJsonAsync<RDTOArtist>();
        Assert.NotNull(dataResult);
        Assert.Equal("Update test", dataResult.NickName);
        try
        {
            await artistsRepository.RemoveAsyncDb(model.ID);
        }
        catch
        {
            output.WriteLine("Test data not removed");
        }
    }
    
}
