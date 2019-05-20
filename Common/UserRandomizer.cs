using Crypteron.Internal.Entropy;

namespace Crypteron.SampleApps.CommonCode
{
    public static class UserRandomizer
    {
        readonly static string[] _items = { "Apples", "Ball", "Cellphone", "DVD player", "Ear covers", "Firewood", "Glasses", "Headset", 
                             "Ink", "Jam", "Keyboard","Lens Cover", "Magazine", "Notebook", "Oranges", "Pail",  
                             "Quiver", "Raincoat", "Saddle", "Truffles", "Underwater camera", "Visor", "Xbox", "Yams", "Zebra"};

        readonly static string[] _names = { "Albert", "Beethoven", "Cherry", "Derrek", "Einstein", "Frank", "Gina", "Hank", "Igor", "Janice", 
                             "Kelly", "Lisa", "Mike", "Nancy", "Olga", "Perry", "Quincy", "Randy", "Sid", "Tracy", "Ursula", 
                             "Victoria", "Wendy", "Xin", "Yanni", "Zack" };

        public static string GetRandomItem()
        {
            return Randomizer.GetRandom(_items);
        }

        public static string GetRandomNames()
        {
            return Randomizer.GetRandom(_names);
        }

        public static string GetRandomPIN()
        {
            return $"{Randomizer.GetRandom(new[] {"Alpha", "Bravo", "Charlie", "Echo"})}-{Randomizer.GetRandom(10000):D4}";
        }

        public static string GetRandomSSN()
        {
            return $"{Randomizer.GetRandom(1000):D3}-{Randomizer.GetRandom(100):D2}-{Randomizer.GetRandom(10000):D4}";
        }

        public static string GetRandomCC()
        {
            return $"{Randomizer.GetRandom(10000):D4}-{Randomizer.GetRandom(10000):D4}-{Randomizer.GetRandom(10000):D4}-{Randomizer.GetRandom(10000):D4}";
        }
    }
}
