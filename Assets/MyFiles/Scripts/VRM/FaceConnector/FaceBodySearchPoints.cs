using System.Collections.Generic;
using UnityEngine;

public class FaceBodySearchPoints
{
    public GameObject mainObject;
    private SkinnedMeshRenderer FaceSkinnedMesh;
    private SkinnedMeshRenderer BodySkinnedMesh;

    public Vector3 top;
    public Vector3 bottom;
    public Vector3 left;
    public Vector3 right;
    public Vector3 boundsCenter;

    public float Xdistance;
    public float Ydistance;
    public float Zdistance;

    public Vector3 pos;

    public FaceBodySearchPoints(GameObject gameObject, bool createDebugCube = false)
    {
        mainObject = gameObject;
        (top, bottom, left, right) = VisualizeBoundary(mainObject, createDebugCube);
        Xdistance = Mathf.Abs(right.x - left.x);
        Ydistance = Mathf.Abs(top.y - bottom.y);
        Zdistance = Mathf.Abs(top.z - bottom.z);
        boundsCenter = FaceSkinnedMesh.bounds.center;
        pos = FaceSkinnedMesh.transform.position;
    }

    public (Vector3 topPoint, Vector3 bottomPoint, Vector3 leftPoint, Vector3 rightPoint) VisualizeBoundary(GameObject mainObject, bool createDebugCube = false)
    {
        FaceSkinnedMesh = FindMeshRenderer(mainObject, "Face");
        BodySkinnedMesh = FindMeshRenderer(mainObject, "Body");

        // Получаем граничные вершины
        HashSet<Vector3> FaceBoundaryVertices = GetBoundaryVertices(FaceSkinnedMesh.sharedMesh);
        // Получаем граничные вершины BodySkinnedMesh
        HashSet<Vector3> bodyBoundaryVertices = GetBoundaryVertices(BodySkinnedMesh.sharedMesh);

        // Находим топовую, нижнюю точки и центр
        Vector3 topPoint, bottomPoint, centerPoint;
        GetTopBottomAndCenter(FaceBoundaryVertices, out topPoint, out bottomPoint, out centerPoint);

        if (createDebugCube)
        {
            // Визуализируем верхнюю, нижнюю точки и центр
            CreateDebugCube(topPoint, Color.red);     // Верхняя точка
            // CreateDebugCube(centerPoint, Color.red); // Центр
            CreateDebugCube(bottomPoint, Color.blue); // Нижняя точка
        }

        HashSet<Vector3> commonPoints = GetCommonOrClosePoints(FaceBoundaryVertices, bodyBoundaryVertices, (topPoint.y - bottomPoint.y) / 1000);

        Vector3 leftPoint, rightPoint;
        // Находим левую и правую точки на основе BodySkinnedMesh
        // FindLeftAndRightPointsUsingBodyX(bodyBoundaryVertices, bottomPoint, out leftPoint, out rightPoint);
        // // Находим левую и правую точки (на основе лица)
        // FindLeftAndRightPoints(FaceBoundaryVertices, topPoint, centerPoint, bottomPoint, out leftPoint, out rightPoint);
        // FindLeftAndRightPointsUsingBodyX(commonPoints, bottomPoint, out leftPoint, out rightPoint);
        FindLeftAndRightPointsUsingBodyZ(commonPoints, bottomPoint, out leftPoint, out rightPoint);

        if (createDebugCube)
        {
            // Визуализируем левую и правую точки
            CreateDebugCube(leftPoint, Color.green);  // Левая точка
            CreateDebugCube(rightPoint, Color.yellow); // Правая точка
        }


        return (topPoint, bottomPoint, leftPoint, rightPoint);
    }

    HashSet<Vector3> GetBoundaryVertices(Mesh mesh)
    {
        Dictionary<(int, int), int> edgeCount = new Dictionary<(int, int), int>();
        int[] triangles = mesh.triangles;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            int v0 = triangles[i];
            int v1 = triangles[i + 1];
            int v2 = triangles[i + 2];

            AddEdge(edgeCount, v0, v1);
            AddEdge(edgeCount, v1, v2);
            AddEdge(edgeCount, v2, v0);
        }

        HashSet<Vector3> boundaryVertices = new HashSet<Vector3>();
        Vector3[] vertices = mesh.vertices;

        foreach (var edge in edgeCount)
        {
            if (edge.Value == 1)
            {
                int v1 = edge.Key.Item1;
                int v2 = edge.Key.Item2;
                boundaryVertices.Add(FaceSkinnedMesh.transform.TransformPoint(vertices[v1])); // Преобразуем в мировые
                boundaryVertices.Add(FaceSkinnedMesh.transform.TransformPoint(vertices[v2])); // Преобразуем в мировые
            }
        }

        return boundaryVertices;
    }

    void GetTopBottomAndCenter(HashSet<Vector3> vertices, out Vector3 top, out Vector3 bottom, out Vector3 center)
    {
        top = Vector3.negativeInfinity;
        bottom = Vector3.positiveInfinity;

        // Находим верхнюю и нижнюю точки
        foreach (var vertex in vertices)
        {
            if (vertex.y > top.y)
            {
                top = vertex;
            }

            if (vertex.y < bottom.y)
            {
                bottom = vertex;
            }
        }

        // Центр между top и bottom
        center = (top + bottom) / 2;
    }

    void AddEdge(Dictionary<(int, int), int> edgeCount, int v1, int v2)
    {
        var edge = (Mathf.Min(v1, v2), Mathf.Max(v1, v2));
        if (edgeCount.ContainsKey(edge))
        {
            edgeCount[edge]++;
        }
        else
        {
            edgeCount[edge] = 1;
        }
    }

    public void CreateDebugCube(Vector3 position, Color color)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        // cube.transform.position = FaceSkinnedMesh.transform.TransformPoint(position); // Локальные координаты -> мировые
        cube.transform.position = position; // Уже в мировых координатах
        cube.transform.localScale = Vector3.one * 0.01f; // Уменьшаем размер куба
        cube.GetComponent<Renderer>().material.color = color;
    }

    SkinnedMeshRenderer FindMeshRenderer(GameObject model, string name)
    {
        foreach (var renderer in model.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            if (renderer.name == name)
                return renderer;
        }
        return null;
    }


    void FindLeftAndRightPoints(
    HashSet<Vector3> vertices,
    Vector3 topPoint,
    Vector3 centerPoint,
    Vector3 bottomPoint,
    out Vector3 leftPoint,
    out Vector3 rightPoint)
    {
        // Шаг 1: Создаем области фильтрации
        float topZ = topPoint.z; // Используем FilterVerticesByZ

        // Верхняя и нижняя средние точки
        Vector3 upperMidPoint = (topPoint + centerPoint) / 2;
        Vector3 lowerMidPoint = (centerPoint + bottomPoint) / 2;

        // Шаг 2: Фильтрация по Z и Y
        List<Vector3> filteredVertices = new List<Vector3>();
        foreach (var vertex in vertices)
        {
            if (vertex.z <= topZ && vertex.y <= upperMidPoint.y && vertex.y >= lowerMidPoint.y)
            {
                filteredVertices.Add(vertex);
            }
        }

        // Шаг 3: Поиск левой точки
        float minDistance = float.MaxValue;
        leftPoint = Vector3.zero;

        foreach (var vertex in filteredVertices)
        {
            float distance = Vector3.Distance(centerPoint, vertex);
            if (distance < minDistance)
            {
                minDistance = distance;
                leftPoint = vertex;
            }
        }

        // Шаг 4: Определение правой точки (симметрия относительно centerPoint)
        rightPoint = new Vector3(
            2 * centerPoint.x - leftPoint.x,
            leftPoint.y,
            leftPoint.z
        );
    }

    HashSet<Vector3> GetCommonOrClosePoints(
    HashSet<Vector3> faceVertices,
    HashSet<Vector3> bodyVertices,
    float threshold)
    {
        // Новый набор для хранения точек, которые общие или близкие
        HashSet<Vector3> commonPoints = new HashSet<Vector3>();

        // Проверяем каждую точку из faceVertices
        foreach (var facePoint in faceVertices)
        {
            foreach (var bodyPoint in bodyVertices)
            {
                // Если точки совпадают или находятся близко друг к другу
                if (Vector3.Distance(facePoint, bodyPoint) <= threshold)
                {
                    commonPoints.Add(facePoint); // Добавляем в результат
                    break; // Достаточно найти хотя бы одну близкую точку
                }
            }
        }

        return commonPoints;
    }

    void FindLeftAndRightPointsUsingBodyZ(
    HashSet<Vector3> bodyVertices,
    Vector3 bottomPoint,
    out Vector3 leftPoint,
    out Vector3 rightPoint)
    {
        // Шаг 1: Фильтруем вершины на основе их положения относительно bottomPoint
        List<Vector3> leftVertices = new List<Vector3>();
        List<Vector3> rightVertices = new List<Vector3>();

        foreach (var vertex in bodyVertices)
        {
            // Учитываем только вершины выше bottomPoint по Y
            if (vertex.y <= bottomPoint.y)
                continue;

            if (vertex.x < bottomPoint.x) // Вершины слева от bottomPoint
            {
                leftVertices.Add(vertex);
            }
            else if (vertex.x > bottomPoint.x) // Вершины справа от bottomPoint
            {
                rightVertices.Add(vertex);
            }
        }

        // Проверяем, что есть хотя бы одна точка для левой и правой стороны
        if (leftVertices.Count == 0)
        {
            Debug.LogError("Нет подходящих вершин слева от bottomPoint.");
            leftPoint = Vector3.zero;
        }
        else
        {
            // Шаг 2: Находим точку с минимальным Z среди левых вершин
            leftPoint = leftVertices[0];
            foreach (var vertex in leftVertices)
            {
                if (vertex.z < leftPoint.z)
                {
                    leftPoint = vertex;
                }
            }
        }

        if (rightVertices.Count == 0)
        {
            Debug.LogError("Нет подходящих вершин справа от bottomPoint.");
            rightPoint = Vector3.zero;
        }
        else
        {
            // Шаг 3: Находим точку с минимальным Z среди правых вершин
            rightPoint = rightVertices[0];
            foreach (var vertex in rightVertices)
            {
                if (vertex.z < rightPoint.z)
                {
                    rightPoint = vertex;
                }
            }
        }
    }

    void FindLeftAndRightPointsUsingBodyX(
    HashSet<Vector3> bodyVertices,
    Vector3 bottomPoint,
    out Vector3 leftPoint,
    out Vector3 rightPoint)
    {
        // Шаг 1: Фильтрация вершин выше bottomPoint по Y
        List<Vector3> filteredVertices = new List<Vector3>();
        foreach (var vertex in bodyVertices)
        {
            if (vertex.y > bottomPoint.y) // Вершины выше bottomPoint
            {
                filteredVertices.Add(vertex);
            }
        }

        if (filteredVertices.Count == 0)
        {
            Debug.LogError("Нет подходящих вершин для определения левой и правой точки.");
            leftPoint = Vector3.zero;
            rightPoint = Vector3.zero;
            return;
        }

        // Шаг 2: Определение минимального и максимального X
        leftPoint = filteredVertices[0];
        rightPoint = filteredVertices[0];

        foreach (var vertex in filteredVertices)
        {
            if (vertex.x < leftPoint.x)
            {
                leftPoint = vertex;
            }
            if (vertex.x > rightPoint.x)
            {
                rightPoint = vertex;
            }
        }
    }
}