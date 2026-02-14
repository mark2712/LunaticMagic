using System.Threading.Tasks;

namespace ResourceSystem
{
    public class ProfilesResourceController : ResourceController<IGameProfiles>
    {
        protected override IGameProfiles Load(string key)
        {
            IGameProfiles profiles = new GameProfiles();
            profiles.LoadProfiles();
            return profiles;
        }

        protected override async Task<IGameProfiles> LoadAsync(string key)
        {
            // если загрузка реально синхронная — просто оборачиваем
            return await Task.Run(() =>
            {
                IGameProfiles profiles = new GameProfiles();
                profiles.LoadProfiles();
                return profiles;
            });
        }

        protected override void UnloadInternal(ResourceAsset<IGameProfiles> asset)
        {
            asset.Resource.Dispose();
        }
    }
}