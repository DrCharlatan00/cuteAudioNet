using cuteAudioNet.APIModels.DTO.Albums;
using cuteAudioNet.APIModels.RDTOModel.Albums;
using cuteAudioNet.Postgresql.Models;
using cuteAudioNet.Postgresql.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace cuteAudioNet.IntegrationTests.AlbumIntTest
{
    public class TestIntAlbum : IClassFixture<TestWebApplicationFactory>
    {
        private readonly HttpClient client;
        private readonly ITestOutputHelper output;
        private readonly IAlbumsRepository repo;
        private readonly IArtistsRepository artist;

        public TestIntAlbum(TestWebApplicationFactory factory, ITestOutputHelper output)
        {
            client = factory.CreateClient();
            this.output = output;

            var scope = factory.Services.CreateScope();
            repo = scope.ServiceProvider.GetRequiredService<IAlbumsRepository>();
            artist = scope.ServiceProvider.GetRequiredService<IArtistsRepository>();
        }

        [Fact]
        public async Task GetCardAlbumTest() {
            try
            {
                var data = await client.GetAsync("api/album/");
                if (!data.IsSuccessStatusCode) output.WriteLine(data.ReasonPhrase);
                Assert.True(data.IsSuccessStatusCode);

                var dataRes = await data.Content.ReadFromJsonAsync<IEnumerable<RDTOAlbumCard>>();

                Assert.NotNull(dataRes);

            }
            catch(Exception ex) {
                output.WriteLine(ex.Message);
            }
        }

        [Fact]
        public async Task GetFullInfomation()
        {
            try
            {
                var data = await client.GetAsync("api/album/full");
                if (!data.IsSuccessStatusCode) output.WriteLine(data.ReasonPhrase);
                Assert.True(data.IsSuccessStatusCode);

                var dataRes = await data.Content.ReadFromJsonAsync<IEnumerable<RDTOAlbum>>();

                Assert.NotNull(dataRes);
            }
            catch (Exception ex) {
                output.WriteLine(ex.Message);
            }
        }

        [Fact]
        public async Task GetWhisPagination() {
            try
            {
                var data = await client.GetAsync("api/album/pag&page=1&pageSize=1");
                if (!data.IsSuccessStatusCode) output.WriteLine(data.ReasonPhrase);
                Assert.True(data.IsSuccessStatusCode);
                var dataRes = await data.Content.ReadFromJsonAsync<IEnumerable<RDTOAlbumCard>>();

                Assert.NotNull(dataRes);
                Assert.Single(dataRes);
            }
            catch (Exception ex) {
                output.WriteLine(ex.Message);
            }
        }

        [Fact]
        public async Task GetByIDTest() {
            Guid idAlbum;
            Guid idArtist;

            try
            {
                var data = new ModelArtistDB
                {
                    ArtistName = "TTest",
                    NickName = "TTestsd"
                };
                var ans = await artist.CreateAsyncDb(data);
                if (ans.ID is null) Assert.Fail("Can't create artist");
                idArtist = (Guid)ans.ID;
            }
            catch (Exception ex) {
                Assert.Fail("Can't create artist:" + ex?.InnerException?.Message);
                return;
            }

            try
            {
                ModelAlbumDB newModel = new ModelAlbumDB
                {

                    ID = Guid.NewGuid(),
                    AlbumName = "Test",
                    ArtistID = idArtist

                };

                var (ID, Message) = await repo.CreateAsyncDb(newModel);
                if (ID is null) throw new Exception($"Item not create\nMessage: {Message}");
                idAlbum = (Guid)ID;
            }
            catch (Exception ex)
            {
                Assert.Fail(ex?.InnerException?.Message);
                return;
            }

            try
            {
                var result = await client.GetAsync($"api/album/{idAlbum}");
                if (!result.IsSuccessStatusCode) output.WriteLine(result.ReasonPhrase);
                Assert.True(result.IsSuccessStatusCode);
                var dataRes = await result.Content.ReadFromJsonAsync<RDTOAlbumCard>();

                Assert.NotNull(dataRes);
                Assert.Equal("Test",dataRes.Name);
            }
            finally {
                try
                {
                    await repo.RemoveAsyncDb(idAlbum);
                    await artist.RemoveAsyncDb(idArtist);
                }
                catch (Exception ex) {
                    output.WriteLine(ex.Message);
                }
            }
        }

        [Fact]
        public async Task CreateTest()
        {
            Guid idArtist;
            try
            {
                var data = new ModelArtistDB
                {
                    ArtistName = "TTest",
                    NickName = "TTestsd"
                };
                var ans = await artist.CreateAsyncDb(data);
                if (ans.ID is null) Assert.Fail("Can't create artist");
                idArtist = (Guid)ans.ID;
            }
            catch (Exception ex)
            {
                Assert.Fail("Can't create artist:" + ex?.InnerException?.Message);
                return;
            }

            DTOCreateAlbum newModel = new DTOCreateAlbum
            (
                Name: "TTTTEST",
                null,
                IdArtist: idArtist
            );

            try
            {
                var result = await client.PostAsJsonAsync("api/album/", newModel);

                if (!result.IsSuccessStatusCode) output.WriteLine(result.ReasonPhrase);


                Assert.True(result.IsSuccessStatusCode);
                bool isCreate = false;

                await foreach (var item in repo.GetAsyncEnumerableAllAlbumDb())
                {
                    if (item.AlbumName == "TTTTEST")
                    {
                        isCreate = true;
                    }
                }
                if (!isCreate) Assert.Fail("Not found element in db");
                

            }
            finally {
                try
                {
                    Guid idAlbum = Guid.NewGuid();
                    await foreach (var item in  repo.GetAsyncEnumerableAllAlbumDb())
                    {
                        if (item.AlbumName == "TTTTEST") {
                            idAlbum = item.ID;
                            break;
                        }
                    }
                    await repo.RemoveAsyncDb(idAlbum);
                    await artist.RemoveAsyncDb(idArtist);
                }
                catch (Exception ex)
                {
                    output.WriteLine($"Failed remove {ex.Message}");
                }
            }
        }

        [Fact]
        public async Task RemoveTest() {
            Guid idAlbum;
            Guid idArtist;

            try
            {
                var data = new ModelArtistDB
                {
                    ArtistName = "TTest",
                    NickName = "TTestsd"
                };
                var ans = await artist.CreateAsyncDb(data);
                if (ans.ID is null) Assert.Fail("Can't create artist");
                idArtist = (Guid)ans.ID;
            }
            catch (Exception ex)
            {
                Assert.Fail("Can't create artist:" + ex?.InnerException?.Message);
                return;
            }

            try
            {
                ModelAlbumDB newModel = new ModelAlbumDB
                {

                    ID = Guid.NewGuid(),
                    AlbumName = "Test",
                    ArtistID = idArtist

                };

                var (ID, Message) = await repo.CreateAsyncDb(newModel);
                if (ID is null) throw new Exception($"Item not create\nMessage: {Message}");
                idAlbum = (Guid)ID;
            }
            catch (Exception ex)
            {
                Assert.Fail(ex?.InnerException?.Message);
                return;
            }

            try
            {
                var result = await client.DeleteAsync($"api/album/{idAlbum}");
                if (!result.IsSuccessStatusCode) output.WriteLine(result.ReasonPhrase);

                Assert.True(result.IsSuccessStatusCode);
            }
            finally {
                try
                {
                    await repo.RemoveAsyncDb(idAlbum);
                    await artist.RemoveAsyncDb(idArtist);
                }
                catch (Exception ex) {
                    output.WriteLine($"Failed remove {ex.Message}");
                }
            }

           

        }

        [Fact]
        public async Task UpdateTest() 
        {
            Guid idAlbum;
            Guid idArtist;

            try
            {
                var data = new ModelArtistDB
                {
                    ArtistName = "TTest",
                    NickName = "TTestsd"
                };
                var ans = await artist.CreateAsyncDb(data);
                if (ans.ID is null) Assert.Fail("Can't create artist");
                idArtist = (Guid)ans.ID;
            }
            catch (Exception ex)
            {
                Assert.Fail("Can't create artist:" + ex?.InnerException?.Message);
                return;
            }

            try
            {
                ModelAlbumDB newModel = new ModelAlbumDB
                {

                    ID = Guid.NewGuid(),
                    AlbumName = "Test",
                    ArtistID = idArtist

                };

                var (ID, Message) = await repo.CreateAsyncDb(newModel);
                if (ID is null) throw new Exception($"Item not create\nMessage: {Message}");
                idAlbum = (Guid)ID;
            }
            catch (Exception ex)
            {
                Assert.Fail(ex?.InnerException?.Message);
                return;
            }

            try
            {
                DTOUpdateAlbum upd = new DTOUpdateAlbum(
                     idAlbum,
                     "TREST",
                     null
                    );
                var result = await client.PutAsJsonAsync("api/album/", upd);

                if (!result.IsSuccessStatusCode) output.WriteLine(result.ReasonPhrase);

                Assert.True(result.IsSuccessStatusCode);

                bool IsUpdated = false;
                await foreach (var item in repo.GetAsyncEnumerableAllAlbumDb())
                {

                    if (item.AlbumName == "TREST")
                    {
                        IsUpdated = true;
                    }
                }
                if (!IsUpdated) Assert.Fail("Item not update");
            }
            finally {
                try
                {
                    await foreach (var item in repo.GetAsyncEnumerableAllAlbumDb())
                    {
                        if (item.AlbumName == "TREST")
                        {
                            idAlbum = item.ID;
                            break;
                        }
                    }
                    await repo.RemoveAsyncDb(idAlbum);
                    await artist.RemoveAsyncDb(idArtist);
                }
                catch (Exception ex)
                {
                    output.WriteLine($"Failed remove {ex.Message}");
                }
            }

        }

        [Fact]
        public async Task TestSearchByName() {
            var data = await client.GetAsync("api/album/by-name?name=test");
            if (!data.IsSuccessStatusCode) output.WriteLine(data.ReasonPhrase);

            Assert.True(data.IsSuccessStatusCode);
            Assert.NotNull(data); 
        }

        [Fact]
        public async Task TestSearchByPagName() {
            var data = await client.GetAsync("api/album/by-name-pag?name=test&page=1&pageSize=5");
            if (!data.IsSuccessStatusCode) output.WriteLine(data.ReasonPhrase);

            Assert.True(data.IsSuccessStatusCode);
            Assert.NotNull(data);
            
        }
    }
}
