using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;

public class MeshSaver
{
    public static void SaveMesh(Mesh mesh, string fileName, bool isEditor)
    {
        string folderPath;
        string fullPath;

        if (isEditor)
        {
            #if UNITY_EDITOR
            folderPath = "Assets/ModelsVRM/GeneratedMeshes/";
            #else
            Debug.LogError("Editor functionality is not available in the build.");
            return;
            #endif
        }
        else
        {
            folderPath = $"{Application.persistentDataPath}/GeneratedMeshes/";
        }

        // Ensure the folder exists
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Full path to save the mesh
        fullPath = Path.Combine(folderPath, fileName);

        #if UNITY_EDITOR
        if (isEditor)
        {
            AssetDatabase.CreateAsset(mesh, fullPath);
            AssetDatabase.SaveAssets();
            Debug.Log($"Mesh saved in Editor: {fullPath}");
        }
        else
        #endif
        {
            // Serialize the mesh as a .asset file or other format
            SaveMeshToFile(mesh, fullPath);
            Debug.Log($"Mesh saved in Game: {fullPath}");
        }
    }

    private static void SaveMeshToFile(Mesh mesh, string path)
    {
        // Пример простой сериализации меша в runtime
        // (для сложных данных может потребоваться кастомная сериализация)
        using (FileStream fs = new FileStream(path, FileMode.Create))
        {
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                // Сохраните данные меша (пример, можно улучшить)
                writer.Write(mesh.vertices.Length);
                foreach (Vector3 vertex in mesh.vertices)
                {
                    writer.Write(vertex.x);
                    writer.Write(vertex.y);
                    writer.Write(vertex.z);
                }
            }
        }
    }
}
