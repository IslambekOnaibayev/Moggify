namespace Core.Interfaces
{
    public interface ISongAudioService
    {
        byte[] GenerateAudio(long userSeed, int globalIndex);
    }
}
