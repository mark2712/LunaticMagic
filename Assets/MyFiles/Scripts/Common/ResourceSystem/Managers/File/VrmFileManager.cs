using System;
using System.Threading.Tasks;
using UnityEngine;
using UniVRM10;


namespace ResourceSystem
{
    public sealed class VrmFileManager : ResourceManager<Vrm10Instance>
    {
        protected override async Task<Vrm10Instance> LoadAsync(string path)
        {
            Vrm10Instance instance = await Vrm10.LoadPathAsync(
                path,
                controlRigGenerationOption: ControlRigGenerationOption.Generate,
                // awaitCaller: new ImmediateCaller(),
                showMeshes: true
             );

            //if (instance != null)
            //{
            //    Debug.Log("VRM-файл успешно загружен.");

            //    // Получаем загруженный GameObject
            //    GameObject model = instance.gameObject;
            //    return model;
            //}
            if (instance == null)
            {
                throw new Exception($"Failed to load VRM: {path}");
            }

            return instance;
        }

        protected override Vrm10Instance Load(string key)
        {
            throw new NotSupportedException("Sync VRM load is not supported");
        }

        protected override void UnloadInternal(ResourceAsset<Vrm10Instance> asset)
        {
            asset.Resource.DisposeRuntime();
        }
    }
}