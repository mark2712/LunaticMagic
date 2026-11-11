using UnityEngine;
using UnityEngine.UIElements;

namespace UITK
{
    /// <summary>
    /// Отвечает за UIDocument и жизненный цикл Root компонента
    /// </summary>
    public class UIManager
    {
        public GameObject UIDocumentRoot { get; private set; }
        public UIComponent RootComponent { get; private set; }

        /// <summary>
        /// Создать Root UIDocument с EventSystem и Root Component
        /// </summary>
        public void Init()
        {
            DestroyEditorPreviewUIDocumentes();
            CreateRuntimeUIDocumentRoot();
            CreateRootComponent();
        }

        /// <summary>
        /// Удалить все превью UIDocument
        /// </summary>
        private void DestroyEditorPreviewUIDocumentes()
        {
            var previews = Object.FindObjectsByType<RootUIDocument>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var preview in previews)
            {
                if (UIDocumentRoot != null && preview != UIDocumentRoot)
                {
                    Object.Destroy(preview.gameObject);
                }
            }
        }

        /// <summary>
        /// Создать корневой UIDocument который будет жить между сценами
        /// </summary>
        private void CreateRuntimeUIDocumentRoot()
        {
            // var prefab = ResourceManager.ResourcesGameObject.Bind("Prefabs/UITK/RootUIDocument").Resource;
            var prefab = ResourceManager.AddressableGameObject.Bind("Assets/MyFiles/UITK/RootUIDocument.prefab").Resource;
            UIDocumentRoot = Object.Instantiate(prefab);
            Object.DontDestroyOnLoad(UIDocumentRoot);
        }

        /// <summary>
        /// создать Root компонент
        /// </summary>
        private void CreateRootComponent()
        {
            UIDocument uiDocument = UIDocumentRoot.GetComponent<UIDocument>();
            VisualElement rootVisualElement = uiDocument.rootVisualElement;
            RootComponent = new UIComponentRoot(null).AddTemplate(null, rootVisualElement).CreateRootNode();
        }
    }
}