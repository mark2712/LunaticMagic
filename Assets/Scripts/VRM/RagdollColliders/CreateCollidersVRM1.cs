using UnityEngine;

[System.Serializable]
public class CreateCollidersVRM1 : MonoBehaviour
{
    public Markers1 markers1;
    public GameObject mainObject;
    public SkinnedMeshRenderer bodySkinnedMeshRenderer;
    public Transform body;
    public GameObject root;
    public bool meshRendererOn = false;

    public void CreateAllPoints(GameObject mainObject1)
    {
        markers1 = mainObject1.GetComponent<Markers1>();
        if (markers1 == null)
        {
            // Если компонента нет, добавляем его
            markers1 = mainObject1.AddComponent<Markers1>();
            markers1.Initialize(mainObject1);
        }

        mainObject = mainObject1;
        body = mainObject.transform.Find("Body");
        if (body == null)
        {
            Debug.LogError("Body not found on the selected GameObject.");
            return;
        }

        bodySkinnedMeshRenderer = body.GetComponent<SkinnedMeshRenderer>();
        if (bodySkinnedMeshRenderer == null)
        {
            Debug.LogError("SkinnedMeshRenderer not found on Body.");
            return;
        }

        root = mainObject.transform.Find("Root").gameObject;

        markers1.RemoveMarkers();

        Transform J_Bip_C_Spine = markers1.FindChildByName(root.transform, "J_Bip_C_Spine");
        Transform J_Bip_C_Chest = markers1.FindChildByName(root.transform, "J_Bip_C_Chest");
        Transform J_Bip_C_UpperChest = markers1.FindChildByName(root.transform, "J_Bip_C_UpperChest");
        Transform J_Bip_C_Neck = markers1.FindChildByName(root.transform, "J_Bip_C_Neck");
        Transform J_Bip_C_Head = markers1.FindChildByName(root.transform, "J_Bip_C_Head");
        Vector3 Head_edge = new Vector3(J_Bip_C_Head.position.x, bodySkinnedMeshRenderer.bounds.max.y, J_Bip_C_Head.position.z);

        Transform J_Bip_R_UpperArm = markers1.FindChildByName(root.transform, "J_Bip_R_UpperArm");
        Transform J_Bip_R_LowerArm = markers1.FindChildByName(root.transform, "J_Bip_R_LowerArm");
        Transform J_Bip_R_Hand = markers1.FindChildByName(root.transform, "J_Bip_R_Hand");
        Vector3 R_Hand_edge = new Vector3(bodySkinnedMeshRenderer.bounds.max.x, J_Bip_R_Hand.position.y, J_Bip_R_Hand.position.z);

        Transform J_Bip_L_UpperArm = markers1.FindChildByName(root.transform, "J_Bip_L_UpperArm");
        Transform J_Bip_L_LowerArm = markers1.FindChildByName(root.transform, "J_Bip_L_LowerArm");
        Transform J_Bip_L_Hand = markers1.FindChildByName(root.transform, "J_Bip_L_Hand");
        Vector3 L_Hand_edge = new Vector3(bodySkinnedMeshRenderer.bounds.min.x, J_Bip_R_Hand.position.y, J_Bip_R_Hand.position.z);

        Transform J_Bip_R_UpperLeg = markers1.FindChildByName(root.transform, "J_Bip_R_UpperLeg");
        Transform J_Bip_R_LowerLeg = markers1.FindChildByName(root.transform, "J_Bip_R_LowerLeg");
        Transform J_Bip_R_Foot = markers1.FindChildByName(root.transform, "J_Bip_R_Foot");
        Transform J_Bip_R_ToeBase = markers1.FindChildByName(root.transform, "J_Bip_R_ToeBase");
        Vector3 R_Foot_edge = new Vector3(J_Bip_R_Foot.position.x, bodySkinnedMeshRenderer.bounds.min.y, J_Bip_R_Foot.position.z);

        Transform J_Bip_L_UpperLeg = markers1.FindChildByName(root.transform, "J_Bip_L_UpperLeg");
        Transform J_Bip_L_LowerLeg = markers1.FindChildByName(root.transform, "J_Bip_L_LowerLeg");
        Transform J_Bip_L_Foot = markers1.FindChildByName(root.transform, "J_Bip_L_Foot");
        Transform J_Bip_L_ToeBase = markers1.FindChildByName(root.transform, "J_Bip_L_ToeBase");
        Vector3 L_Foot_edge = new Vector3(J_Bip_L_Foot.position.x, bodySkinnedMeshRenderer.bounds.min.y, J_Bip_L_Foot.position.z);


        CreatePointsArms(J_Bip_R_UpperArm, J_Bip_R_LowerArm);
        CreatePointsArms(J_Bip_R_LowerArm, J_Bip_R_Hand);
        CreatePointsHands(J_Bip_R_Hand, R_Hand_edge);

        CreatePointsArms(J_Bip_L_UpperArm, J_Bip_L_LowerArm);
        CreatePointsArms(J_Bip_L_LowerArm, J_Bip_L_Hand);
        CreatePointsHands(J_Bip_L_Hand, L_Hand_edge);

        CreatePointsLegs(J_Bip_R_UpperLeg, J_Bip_R_LowerLeg, true);
        CreatePointsLegs(J_Bip_R_LowerLeg, J_Bip_R_Foot, true);
        CreatePointsFoot(J_Bip_R_Foot, R_Foot_edge, J_Bip_R_ToeBase, true);

        CreatePointsLegs(J_Bip_L_UpperLeg, J_Bip_L_LowerLeg, false);
        CreatePointsLegs(J_Bip_L_LowerLeg, J_Bip_L_Foot, false);
        CreatePointsFoot(J_Bip_L_Foot, L_Foot_edge, J_Bip_L_ToeBase, false);

        CreatePointsHips(J_Bip_C_Spine, J_Bip_L_UpperLeg, J_Bip_R_UpperLeg);
        CreatePointsChest(J_Bip_C_Chest, J_Bip_C_Spine);
        CreatePointsUpperChest(J_Bip_C_Chest, J_Bip_C_Neck, J_Bip_C_UpperChest);
        CreatePointsNeck(J_Bip_C_Neck, J_Bip_C_Head);
        CreatePointsHead(J_Bip_C_Head, Head_edge);
    }


    private void CreatePointsFoot(Transform jointA, Vector3 jointB, Transform ToeBase, bool derictionCut)
    {
        Vector3 positionA = jointA.position;
        Vector3 positionB = jointB;
        float distanceY = Vector3.Distance(positionA, positionB); // Расстояние между точками
        Vector3 middlePointMaxZ = new Vector3(positionB.x, positionB.y, bodySkinnedMeshRenderer.bounds.max.z);
        Vector3 middlePointMinZ = new Vector3(positionB.x, positionB.y, bodySkinnedMeshRenderer.bounds.min.z);
        float distanceZ = Vector3.Distance(middlePointMaxZ, middlePointMinZ);
        Vector3 middlePointZ = (middlePointMaxZ + middlePointMinZ) / 2f;
        Vector3 middlePointZY = (positionA + middlePointZ) / 2f;

        Bounds initialBounds = CreateBoundingBox(middlePointZY, new Vector3(distanceY * 2, distanceY * 0.95f, distanceZ * 0.95f));
        initialBounds = CutBoundsByPlane(initialBounds, bodySkinnedMeshRenderer.bounds.center.x, "x", derictionCut);
        // VisualizeBoundingBox(initialBounds);
        (float minX, float maxX, float minY, float maxY, float minZ, float maxZ) = GetMeshBoundsWithin(bodySkinnedMeshRenderer, initialBounds);
        Vector3 CapsuleMiddlePoint = new Vector3(middlePointZY.x, middlePointZY.y, (maxZ + minZ) / 2);
        CreateCapsule(jointA.gameObject, CapsuleMiddlePoint, new float[] { minX, maxX, minY, maxY, minZ, maxZ }, new float[] { 0.8f, 0.8f, 0.8f }, "z");
        Vector3 BoxMiddlePoint = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, (minZ + maxZ) / 2);
        // CreateCapsule(ToeBase.gameObject, BoxMiddlePoint, new float[] { minX, maxX, minY, maxY, minZ, maxZ }, new float[] { 0.8f, 0.8f, 0.8f }, "z");
        CreateBox(ToeBase.gameObject, BoxMiddlePoint, new float[] { minX, maxX, minY, maxY, minZ, maxZ }, new float[] { 0.8f, 0.8f, 0.8f });
    }

    private void CreatePointsHead(Transform jointA, Vector3 Head_edge)
    {
        Vector3 headCentre = jointA.position;
        float radius = Vector3.Distance(headCentre, Head_edge) / 2; // Расстояние между точками
        float minX = headCentre.x - radius;
        float maxX = headCentre.x + radius;
        float minY = headCentre.y - radius; // Нижняя граница капсулы
        float maxY = headCentre.y + radius; // Верхняя граница капсулы
        float minZ = headCentre.z - radius;
        float maxZ = headCentre.z + radius;
        CreateCapsule(jointA.gameObject, (jointA.position + Head_edge) / 2, new float[] { minX, maxX, minY, maxY, minZ, maxZ }, new float[] { 0.9f, 1, 1f }, "y");
    }

    private void CreatePointsNeck(Transform jointA, Transform jointB)
    {
        Vector3 positionA = jointA.position;
        Vector3 positionB = jointB.position;
        Vector3 middlePoint = (positionA + positionB) / 2f; // Центр между точками
        float distanceY = Vector3.Distance(positionA, positionB); // Расстояние между точками
        Bounds initialBounds = CreateBoundingBox(middlePoint, new Vector3(distanceY * 2, distanceY / 6, distanceY * 2));
        // VisualizeBoundingBox(initialBounds);
        (float minX, float maxX, float minY, float maxY, float minZ, float maxZ) = GetMeshBoundsWithin(bodySkinnedMeshRenderer, initialBounds);
        CreateCapsule(jointA.gameObject, middlePoint, new float[] { minX, maxX, jointA.position.y, jointB.position.y, minZ, maxZ }, new float[] { 0.9f, 0.9f, 0.9f }, "y");
    }

    private void CreatePointsUpperChest(Transform jointA, Transform jointB, Transform upperChest)
    {
        Vector3 positionA = jointA.position;
        Vector3 positionB = jointB.position;
        Vector3 middlePoint = (positionA + positionB) / 2f;
        float distanceY = Vector3.Distance(positionA, positionB);
        Bounds initialBounds = CreateBoundingBox(middlePoint, new Vector3(distanceY * 2, distanceY / 6f, distanceY * 2));
        // VisualizeBoundingBox(initialBounds);
        (float minX, float maxX, float minY, float maxY, float minZ, float maxZ) = GetMeshBoundsWithin(bodySkinnedMeshRenderer, initialBounds);
        CreateBox(upperChest.gameObject, middlePoint, new float[] { minX, maxX, positionA.y, positionB.y, minZ, maxZ }, new float[] { 0.7f, 0.7f, 0.7f });
    }

    private void CreatePointsChest(Transform jointA, Transform jointB)
    {
        Vector3 positionA = jointA.position;
        Vector3 positionB = jointB.position;
        Vector3 middlePoint = (positionA + positionB) / 2f;
        float distanceY = Vector3.Distance(positionA, positionB);
        Bounds initialBounds = CreateBoundingBox(middlePoint, new Vector3(distanceY * 4, distanceY / 3f, distanceY * 4));
        // VisualizeBoundingBox(initialBounds);
        (float minX, float maxX, float minY, float maxY, float minZ, float maxZ) = GetMeshBoundsWithin(bodySkinnedMeshRenderer, initialBounds);
        CreateBox(jointA.gameObject, middlePoint, new float[] { minX, maxX, positionB.y, positionA.y, minZ, maxZ }, new float[] { 0.7f, 0.7f, 0.7f });
    }

    private void CreatePointsHips(Transform jointA, Transform jointB, Transform jointC)
    {
        Vector3 positionA = jointA.position;
        Vector3 positionB = jointB.position;
        Vector3 positionC = jointC.position;
        Vector3 middlePointLegs = (positionC + positionB) / 2f;
        Vector3 middlePointHips = (positionA + middlePointLegs) / 2f;
        float distanceY = Vector3.Distance(positionA, middlePointLegs);
        Bounds initialBounds = CreateBoundingBox(middlePointLegs, new Vector3(distanceY * 4, distanceY / 3f, distanceY * 4));
        // VisualizeBoundingBox(initialBounds);
        (float minX, float maxX, float minY, float maxY, float minZ, float maxZ) = GetMeshBoundsWithin(bodySkinnedMeshRenderer, initialBounds);
        CreateBox(jointA.gameObject, middlePointHips, new float[] { minX, maxX, middlePointLegs.y, positionA.y, minZ, maxZ }, new float[] { 0.7f, 0.7f, 0.7f });
    }

    private void CreatePointsHands(Transform jointA, Vector3 jointB)
    {
        Vector3 positionA = jointA.position;
        Vector3 positionB = jointB;
        Vector3 middlePoint = (positionA + positionB) / 2f; // Центр между точками
        float distanceX = Vector3.Distance(positionA, positionB); // Расстояние между точками
        Bounds initialBounds = CreateBoundingBox(middlePoint, new Vector3(distanceX / 6, distanceX / 2.3f, distanceX / 2.3f));
        (float minX, float maxX, float minY, float maxY, float minZ, float maxZ) = GetMeshBoundsWithin(bodySkinnedMeshRenderer, initialBounds);
        CreateCapsule(jointA.gameObject, middlePoint, new float[] { jointA.position.x, jointB.x, minY, maxY, minZ, maxZ }, new float[] { 0.8f, 1f, 1f }, "x");
    }

    private void CreatePointsLegs(Transform jointA, Transform jointB, bool derictionCut)
    {
        Vector3 positionA = jointA.position;
        Vector3 positionB = jointB.position;
        Vector3 middlePoint = (positionA + positionB) / 2f; // Центр между точками
        float distanceX = Vector3.Distance(positionA, positionB); // Расстояние между точками
        // markers1.CreateMarker(bodySkinnedMeshRenderer.bounds.center, "Marker" + bodySkinnedMeshRenderer.bounds.center.ToString());
        Bounds initialBounds = CreateBoundingBox(middlePoint, new Vector3(distanceX / 2.3f, distanceX / 6, distanceX / 2.3f));
        // VisualizeBoundingBox(initialBounds);
        initialBounds = CutBoundsByPlane(initialBounds, bodySkinnedMeshRenderer.bounds.center.x, "x", derictionCut);
        // VisualizeBoundingBox(initialBounds);
        (float minX, float maxX, float minY, float maxY, float minZ, float maxZ) = GetMeshBoundsWithin(bodySkinnedMeshRenderer, initialBounds);
        CreateCapsule(jointA.gameObject, middlePoint, new float[] { minX, maxX, jointA.position.y, jointB.position.y, minZ, maxZ }, new float[] { 1, 1, 1 }, "y");
    }

    private void CreatePointsArms(Transform jointA, Transform jointB)
    {
        Vector3 positionA = jointA.position;
        Vector3 positionB = jointB.position;
        Vector3 middlePoint = (positionA + positionB) / 2f; // Центр между точками
        float distanceX = Vector3.Distance(positionA, positionB); // Расстояние между точками
        Bounds initialBounds = CreateBoundingBox(middlePoint, new Vector3(distanceX / 6, distanceX / 2.3f, distanceX / 2.3f));
        (float minX, float maxX, float minY, float maxY, float minZ, float maxZ) = GetMeshBoundsWithin(bodySkinnedMeshRenderer, initialBounds);
        CreateCapsule(jointA.gameObject, middlePoint, new float[] { jointA.position.x, jointB.position.x, minY, maxY, minZ, maxZ }, new float[] { 1, 1.2f, 1.2f }, "x");
    }


    private Bounds CutBoundsByPlane(Bounds bounds, float coordinate, string axis, bool keepAbove)
    {
        Vector3 center = bounds.center;
        Vector3 size = bounds.size;

        switch (axis.ToLower())
        {
            case "x":
                if (keepAbove)
                {
                    if (center.x - size.x / 2 < coordinate)
                    {
                        float delta = coordinate - (center.x - size.x / 2);
                        size.x -= delta;
                        center.x += delta / 2;
                    }
                }
                else
                {
                    if (center.x + size.x / 2 > coordinate)
                    {
                        float delta = (center.x + size.x / 2) - coordinate;
                        size.x -= delta;
                        center.x -= delta / 2;
                    }
                }
                break;

            case "y":
                if (keepAbove)
                {
                    if (center.y - size.y / 2 < coordinate)
                    {
                        float delta = coordinate - (center.y - size.y / 2);
                        size.y -= delta;
                        center.y += delta / 2;
                    }
                }
                else
                {
                    if (center.y + size.y / 2 > coordinate)
                    {
                        float delta = (center.y + size.y / 2) - coordinate;
                        size.y -= delta;
                        center.y -= delta / 2;
                    }
                }
                break;

            case "z":
                if (keepAbove)
                {
                    if (center.z - size.z / 2 < coordinate)
                    {
                        float delta = coordinate - (center.z - size.z / 2);
                        size.z -= delta;
                        center.z += delta / 2;
                    }
                }
                else
                {
                    if (center.z + size.z / 2 > coordinate)
                    {
                        float delta = (center.z + size.z / 2) - coordinate;
                        size.z -= delta;
                        center.z -= delta / 2;
                    }
                }
                break;

            default:
                Debug.LogError("Недопустимая ось: " + axis);
                break;
        }

        return new Bounds(center, size);
    }

    // Функция создания Bounds
    private Bounds CreateBoundingBox(Vector3 center, Vector3 size)
    {
        return new Bounds(center, size);
    }

    // Функция создания визуализации бокса
    private void VisualizeBoundingBox(Bounds bounds)
    {
        // Создаем объект визуализации ограничивающего объема
        GameObject boundingBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
        boundingBox.name = "MeshSizeMarker";
        boundingBox.tag = markers1.markerTag;
        boundingBox.transform.position = bounds.center; // Центр из Bounds
        boundingBox.transform.localScale = bounds.size; // Размер из Bounds
        boundingBox.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 0.3f); // Полупрозрачный зелёный

        // Помещаем в переданный объект (например, mainObject)
        boundingBox.transform.SetParent(mainObject.transform);
    }

    //вычислить границы мэша в ограничивающем объёме
    private (float minX, float maxX, float minY, float maxY, float minZ, float maxZ) GetMeshBoundsWithin(SkinnedMeshRenderer body, Bounds filterBounds)
    {
        Mesh mesh = body.sharedMesh;
        if (mesh == null)
        {
            Debug.LogError("У объекта body отсутствует mesh.");
            return (float.MaxValue, float.MinValue, float.MaxValue, float.MinValue, float.MaxValue, float.MinValue);
        }

        Vector3[] vertices = mesh.vertices;
        Matrix4x4 localToWorld = body.transform.localToWorldMatrix;

        float minX = float.MaxValue, maxX = float.MinValue;
        float minY = float.MaxValue, maxY = float.MinValue;
        float minZ = float.MaxValue, maxZ = float.MinValue;

        foreach (Vector3 vertex in vertices)
        {
            // Преобразуем вершину в мировые координаты
            Vector3 worldVertex = localToWorld.MultiplyPoint3x4(vertex);

            // Проверяем, находится ли вершина в ограничивающем объеме
            if (filterBounds.Contains(worldVertex))
            {
                // Обновляем минимальные и максимальные координаты
                if (worldVertex.x <= minX) minX = worldVertex.x;
                if (worldVertex.x >= maxX) maxX = worldVertex.x;
                if (worldVertex.y <= minY) minY = worldVertex.y;
                if (worldVertex.y >= maxY) maxY = worldVertex.y;
                if (worldVertex.z <= minZ) minZ = worldVertex.z;
                if (worldVertex.z >= maxZ) maxZ = worldVertex.z;
            }
        }

        return (minX, maxX, minY, maxY, minZ, maxZ);
    }

    private GameObject CreateCapsule(GameObject parent, Vector3 pointCenter, float[] bounds, float[] sizeCof, string orientationAxis = "Y")
    {
        float minX = bounds[0];
        float maxX = bounds[1];
        float minY = bounds[2];
        float maxY = bounds[3];
        float minZ = bounds[4];
        float maxZ = bounds[5];

        float sizeCofX = sizeCof[0];
        float sizeCofY = sizeCof[1];
        float sizeCofZ = sizeCof[2];

        float finalX = maxX - minX;
        float finalY = maxY - minY;
        float finalZ = maxZ - minZ;

        GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        capsule.name = parent.name + "_collider";
        capsule.transform.SetParent(parent.transform, false);
        capsule.tag = markers1.markerTag;

        // Отключаем видимость меша
        MeshRenderer meshRenderer = capsule.GetComponent<MeshRenderer>();
        if (meshRenderer != null && !meshRendererOn)
        {
            meshRenderer.enabled = false;
        }

        // Устанавливаем масштаб капсулы
        switch (orientationAxis.ToUpper())
        {
            case "X": // Ориентация вдоль оси X
                capsule.transform.localScale = new Vector3(finalY * sizeCofY, finalX * sizeCofX / 2, finalZ * sizeCofZ);
                capsule.transform.rotation = Quaternion.Euler(0, 0, 90);
                break;
            case "Z": // Ориентация вдоль оси Z
                capsule.transform.localScale = new Vector3(finalX * sizeCofX, finalZ * sizeCofZ / 2, finalY * sizeCofY);
                capsule.transform.rotation = Quaternion.Euler(90, 0, 0);
                break;
            default: // Ориентация вдоль оси Y (по умолчанию)
                capsule.transform.localScale = new Vector3(finalX * sizeCofX, finalY * sizeCofY / 2, finalZ * sizeCofZ);
                break;
        }

        // Устанавливаем позицию капсулы
        capsule.transform.position = pointCenter;

        Debug.Log($"Капсула успешно создана с ориентацией по оси {orientationAxis} и подогнана.");
        return capsule;
    }


    private GameObject CreateBox(GameObject parent, Vector3 pointCenter, float[] bounds, float[] sizeCof)
    {
        float minX = bounds[0];
        float maxX = bounds[1];
        float minY = bounds[2];
        float maxY = bounds[3];
        float minZ = bounds[4];
        float maxZ = bounds[5];

        float sizeCofX = sizeCof[0];
        float sizeCofY = sizeCof[1];
        float sizeCofZ = sizeCof[2];

        float finalX = maxX - minX;
        float finalY = maxY - minY;
        float finalZ = maxZ - minZ;

        // Создаем объект бокса
        GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
        box.name = parent.name + "_collider";
        box.transform.SetParent(parent.transform, false);
        box.tag = markers1.markerTag;

        MeshRenderer meshRenderer = box.GetComponent<MeshRenderer>();
        if (meshRenderer != null && !meshRendererOn)
        {
            meshRenderer.enabled = false;
        }

        // Устанавливаем размеры бокса
        box.transform.localScale = new Vector3(finalX * sizeCofX, finalY * sizeCofY, finalZ * sizeCofZ);
        box.transform.position = pointCenter;

        // // Настраиваем визуальные свойства
        // Renderer renderer = box.GetComponent<Renderer>();
        // renderer.material.color = Color.cyan; // Устанавливаем цвет для визуализации

        Debug.Log("Бокс успешно создан и подогнан.");
        return box;
    }
}
