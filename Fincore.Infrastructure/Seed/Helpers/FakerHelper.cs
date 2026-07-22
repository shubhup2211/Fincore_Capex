using Bogus;

namespace Fincore.Infrastructure.Seed.Helpers
{
    /// <summary>
    /// Centralised Bogus helpers – fixed randomizer seed guarantees the same
    /// deterministic dataset is generated across every developer environment.
    /// </summary>
    public static class FakerHelper
    {
        public const int GlobalSeed = 20260101;

        static FakerHelper()
        {
            Randomizer.Seed = new System.Random(GlobalSeed);
        }

        public static Faker<T> New<T>(string locale = "en") where T : class
        {
            var faker = new Faker<T>(locale)
                .UseSeed(GlobalSeed);
            return faker;
        }

        /// <summary>
        /// Deterministic PRNG that can be used outside Faker&lt;T&gt; contexts.
        /// </summary>
        public static readonly Randomizer Rng = new Randomizer(GlobalSeed);
    }
}
