namespace Core.Interfaces
{
    public interface ISongCoverService
    {
        byte[] GenerateCover(long userSeed, int globalIndex, double likesAvg, string locale, string title, string artist);
    }
}
