using Xunit.Abstractions;

namespace cuteAudioNet.IntegrationTests;

public class TestsIntArtists : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient httpClient;
    private readonly ITestOutputHelper output;

    public TestsIntArtists(TestWebApplicationFactory factory, ITestOutputHelper output)
    {
        httpClient = factory.CreateClient();
        this.output = output;
    }

    [Fact]
    public async Task TestGetCard() {
        var result = await httpClient.GetAsync("api/artist");
        if (!result.IsSuccessStatusCode) output.WriteLine(result.ReasonPhrase);

        Assert.True(result.IsSuccessStatusCode);
        Assert.NotNull(result);

    }

    [Fact]
    public async Task TestGetFull()
    {
        var result = await httpClient.GetAsync("api/artist/full");
        if (!result.IsSuccessStatusCode) output.WriteLine(result.ReasonPhrase);

        Assert.True(result.IsSuccessStatusCode);
        Assert.NotNull(result);

    }


    [Fact]
    public async Task TestGetInfo()
    {
        var result = await httpClient.GetAsync("api/artist/info");
        if (!result.IsSuccessStatusCode) output.WriteLine(result.ReasonPhrase);

        Assert.True(result.IsSuccessStatusCode);
        Assert.NotNull(result);

    }

    [Fact]
    public async Task TestGetPag()
    {
        var result = await httpClient.GetAsync("api/artist/pag?page=1&pageSize=1");
        if (!result.IsSuccessStatusCode) output.WriteLine(result.ReasonPhrase);

        Assert.True(result.IsSuccessStatusCode);
        Assert.NotNull(result);
    }

    

}
