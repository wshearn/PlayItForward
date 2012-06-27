namespace PiF.Models
{
    public partial class Game
    {
        public int Count { get; set; }

        public string ImageURL
        {
            get
            {
                if (SteamID > 0)
                {
                    return string.Format(
                        "http://cdn.steampowered.com/v/gfx/{0}/{1}/capsule_184x69.jpg",
                        IsSteamSubscription ? "subs" : "apps",
                        SteamID);
                }

                return null;
            }
        }
    }
}