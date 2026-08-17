using cuteAudioNet.APIModels.RDTOModel.Albums;

namespace cuteAudioNet.APIModels.RDTOModel.Artists
{
    public record RDTOArtistCard(string NickName, IEnumerable<RDTOAlbumCard> AlbumCards);

    public class RDTOArtist(string Name, string NickName, string? Surname, DateTime? BordDate, string? Pathonymic, IEnumerable<RDTOAlbum> Albums)
    {
        public string Name { get; init; } = Name;
        public string NickName { get; init; } = NickName;
        public string? Surname { get; init; } = Surname;
        public DateTime? BordDate { get; init; } = BordDate;
        public string? Pathonymic { get; init; } = Pathonymic;
        public IEnumerable<RDTOAlbum> Albums { get; set; } = Albums;

        public override bool Equals(object? obj)
        {
            return obj is RDTOArtist artist &&
                   Name == artist.Name &&
                   NickName == artist.NickName &&
                   Surname == artist.Surname &&
                   BordDate == artist.BordDate &&
                   Pathonymic == artist.Pathonymic &&
                   EqualityComparer<IEnumerable<RDTOAlbum>>.Default.Equals(Albums, artist.Albums);
        }
    }
    public record RDTOOnlyArtistInfo(string Name, string NickName, string? Surname, DateTime? BordDate, string? Pathonymic);
    
}
