using System.Collections.Generic;
using UnityEngine;

namespace UICanvas
{
    /// <summary>
    /// Отвечает за Canvas и жизненный цикл Root компонента
    /// </summary>
    public class UIManager
    {
        private const string CanvasPrefabPath = "Prefabs/UI/Root/RootCanvas";
        public GameObject CanvasRoot { get; private set; }
        public UIComponent RootComponent { get; private set; }

        /// <summary>
        /// Создать Root Canvas с EventSystem и Root Component
        /// </summary>
        public void Init()
        {
            DestroyEditorPreviewCanvases();
            CreateRuntimeCanvasRoot();
            CreateRootComponent();
        }

        /// <summary>
        /// Удалить все превью канвасы (могут быть на сценах чтобы сразу в редакторе видеть внешний вид UI)
        /// </summary>
        private void DestroyEditorPreviewCanvases()
        {
            var previews = Object.FindObjectsByType<RootCanvasEditorPreview>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var preview in previews)
            {
                Object.Destroy(preview.gameObject);
            }
        }

        /// <summary>
        /// Создать корневой кансвас который будет жить между сценами
        /// </summary>
        private void CreateRuntimeCanvasRoot()
        {
            var prefab = PrefabManager.Load(CanvasPrefabPath);
            CanvasRoot = Object.Instantiate(prefab);
            Object.DontDestroyOnLoad(CanvasRoot);
        }

        /// <summary>
        /// создать Root компонент
        /// </summary>
        private void CreateRootComponent()
        {
            GameObject prefabRoot = PrefabManager.Load("Prefabs/UI/Root/ComponentRoot");
            RootComponent = new ComponentRoot(null).CreateRootNode().AddPrefab(prefabRoot, CanvasRoot);
        }
    }
}