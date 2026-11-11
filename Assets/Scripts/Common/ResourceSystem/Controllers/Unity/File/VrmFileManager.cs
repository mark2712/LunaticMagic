using System;
using System.Threading.Tasks;
using UnityEngine;
using UniVRM10;


namespace ResourceSystem
{
    /// <summary>
    /// Менеджер загрузки и выгрузки VRM 1.0 моделей напрямую из файловой системы.
    /// 
    /// TODO: В дальнейшем систему работы с VRM необходимо улучшить и оптимизировать:
    /// 1. Интеграция с пулом и "Комнатой для моделей" (Model Staging Room):
    ///    Создание/парсинг VRM на лету вызывает ощутимые микрофризы (шейдеры MToon, меши, кости SpringBone).
    ///    Необходимо реализовать прогрев и хранение инстансов моделей в скрытой системной комнате под сценой (0, -2000, 0),
    ///    чтобы при спавне NPC прикрепление модели происходило мгновенно за 1 кадр.
    /// 2. Вынос пула моделей в глобальный контекст (DontDestroyOnLoad), чтобы прогретые модели не уничтожались
    ///    при смене игровых сцен и могли повторно использоваться между сессиями.
    /// </summary>
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