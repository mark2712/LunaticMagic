using System;
using System.Threading.Tasks;
using UnityEngine;
using UniVRM10;


namespace ResourceSystem
{
    public sealed class VrmFileManager : ResourceController<Vrm10Instance>
    {
        protected override async Task<Vrm10Instance> LoadAsync(string path)
        {
            Vrm10Instance instance = await Vrm10.LoadPathAsync(
                path,
                controlRigGenerationOption: ControlRigGenerationOption.Generate,
                // awaitCaller: new ImmediateCaller(),
                showMeshes: true
            );

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