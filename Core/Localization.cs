namespace Core
{
    public static class Localization
    {
        public static ILocalizationContext? Current { get; set; }

        public sealed class LocalizationContext : ILocalizationContext
        {
            public IStringLocalizer Localizer { get; }

            public LocalizationContext(IStringLocalizer localizer) =>
                Localizer = localizer;
        }
    }

    public interface ILocalizationContext
    {
        IStringLocalizer Localizer { get; }
    }
}
