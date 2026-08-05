using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XUnit;

namespace cuteAudioNet.UnitTests.TrackUnitTests
{
    public class XTestAlbum
    {
        [Fact (Skip = "Not implemented now(")]
        public async void TestGetAlbumByIDWhisNotFound()
        {
            var repoMock = new Mock<IAlbumsRepository>();
            Guid id = Guid.NewGuid();
            repoMock.Setup( x=> x.GetByIdAsyncDb(id)).ReturnsAsync(null);


        }
    }
}