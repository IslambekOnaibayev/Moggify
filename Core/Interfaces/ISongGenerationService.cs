namespace Core.Interfaces
{
    public interface ISongGenerationService
    {
        IReadOnlyList<Song> GeneratePage(string locale, long userSeed, int page, int pageSize, double likesAvg);
        Song GenerateOne(string locale, long userSeed, int globalIndex, double likesAvg);
    }
}
