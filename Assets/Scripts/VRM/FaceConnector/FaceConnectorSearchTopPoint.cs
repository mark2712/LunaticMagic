using System.Collections.Generic;
using UnityEngine;

public class FaceConnectorSearchTopPoint
{
    // public (Vector3 topPoint, Vector3 bottomPoint, Vector3 leftPoint, Vector3 rightPoint) VisualizeBoundary(SkinnedMeshRenderer targetPart, bool createDebugCube = false)
    public Vector3 VisualizeBoundary(SkinnedMeshRenderer targetPart, bool createDebugCube = false)
    {
        // Создаём временный Mesh для targetPart
        Mesh bakedMesh = new Mesh();
        targetPart.BakeMesh(bakedMesh);

        // Получаем границы bakedMesh
        Vector3[] vertices = bakedMesh.vertices;

        // Преобразуем вершины в мировые координаты
        Vector3 topPoint = Vector3.negativeInfinity;
        Vector3 bottomPoint = Vector3.positiveInfinity;
        Vector3 leftPoint = Vector3.positiveInfinity;
        Vector3 rightPoint = Vector3.negativeInfinity;

        foreach (var vertex in vertices)
        {
            Vector3 worldVertex = targetPart.transform.TransformPoint(vertex);

            if (worldVertex.y > topPoint.y) topPoint = worldVertex;
            if (worldVertex.y < bottomPoint.y) bottomPoint = worldVertex;
            if (worldVertex.x < leftPoint.x) leftPoint = worldVertex;
            if (worldVertex.x > rightPoint.x) rightPoint = worldVertex;
        }

        if (createDebugCube)
        {
            // Визуализируем точки
            CreateDebugCube(topPoint, Color.red);     // Верхняя точка
            CreateDebugCube(bottomPoint, Color.blue); // Нижняя точка
            CreateDebugCube(leftPoint, Color.green);  // Левая точка
            CreateDebugCube(rightPoint, Color.yellow); // Правая точка

        }

        // Возвращаем точки для дальнейшего использования
        return topPoint;
    }

    public void CreateDebugCube(Vector3 position, Color color)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = position;
        cube.transform.localScale = Vector3.one * 0.01f; // Уменьшаем размер куба
        cube.GetComponent<Renderer>().material.color = color;
    }
}