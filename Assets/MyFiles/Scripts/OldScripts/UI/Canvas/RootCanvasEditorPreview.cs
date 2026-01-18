using UnityEngine;

namespace UICanvas
{
    public class RootCanvasEditorPreview : MonoBehaviour
    {
        private void Awake()
        {
            Destroy(gameObject);
        }
    }
}