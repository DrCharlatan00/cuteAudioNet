using AutoMapper;
using cuteAudioNet.APIModels.DTO.Tracks;
using cuteAudioNet.APIModels.RDTOModel.Tracks;
using cuteAudioNet.Postgresql.Models;
using cuteAudioNet.Postgresql.Repositories;
using cuteAudioNet.Postgresql.Repositories.Interfaces;
using cuteAudioNet.Services;
using FluentValidation;
using Moq;

namespace cuteAudioNet.UnitTests;

public class TestTrack
{
    [Fact]
    public async Task TrackGetTest()
    {

        var PgMock = new Mock<ITracksRepository>();

        var TestData = new List<ModelTrackDB>();

        for (int i = 0; i < 5; i++)
        {
            TestData.Add(new ModelTrackDB
            {
                ID = Guid.NewGuid(),
                Album = new ModelAlbumDB
                {
                    ID = Guid.NewGuid(),
                    Artist = new ModelArtistDB
                    {
                        NickName = "Test",
                        ArtistName = "Test",
                    }
                }
            });
        }



        async IAsyncEnumerable<ModelTrackDB> Get()
        {
            foreach (var item in TestData)
            {
                await Task.Yield();
                yield return item;
            }
        }

        PgMock.Setup(x => x.GetAllAsyncEnumerableDb())
            .Returns(Get());

        var mapper = new Mock<IMapper>();

        var map_return = new RDTOCardTrack("Pupa", APIModels.RDTOModel.Tracks.MusicGenre.JAZZ, "Test");

        mapper.Setup(x => x.Map<ModelTrackDB, RDTOCardTrack>(It.IsAny<ModelTrackDB>()))
            .Returns(map_return);

        var valid = new Mock<IValidator<DTOTrack>>();

        valid.Setup(x => x.Validate(It.IsAny<DTOTrack>())).Returns(new FluentValidation.Results.ValidationResult());


        var service = new TrackService(PgMock.Object, mapper.Object, valid.Object);

        var result = await service.GetTrackCardAsync();

        Assert.IsType<List<RDTOCardTrack>>(result);

        // Assert.Contains(map_return, result);
    }
}
