using Newtonsoft.Json.Linq;

namespace RPG.Saving
{
    public interface ISaveable
    {
        JToken CaptureAsJToken();

        void RestoreFromJToken(JToken state);
    }
}
