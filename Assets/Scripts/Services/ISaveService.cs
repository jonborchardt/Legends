using Legends.Data;

namespace Legends.Services
{
    public interface ISaveService
    {
        void Save(GameState state);
        GameState Load();
        void Delete();
        bool Exists();
    }
}
