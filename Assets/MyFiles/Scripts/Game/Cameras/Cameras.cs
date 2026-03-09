using System;
using Entities;
using UnityEngine;

public interface ICameras
{

}

public class Cameras : ICameras
{

}

public interface ICameraBody
{
    IEntity TargetEntity { get; }
    IFirstPersonCamera FirstPersonCamera { get; }
    IThirdPersonCamera ThirdPersonCamera { get; }
    void Update(Vector2 lookInput);
    void OnScrollInputPerformed(float value);
}


[Serializable]
public class CameraBodyData // у каждой сущности могут быть свои данные
{
    public Vector3 CameraPosition = new();
    public Quaternion CameraRotation = new();
    public CamerasTypes CameraType = CamerasTypes.FirstPersonCamera;

    public float MinCameraDistance = 1;
    public float MaxCameraDistance = 5;
    public bool IsMinCameraDistance = false;
    public float LookSpeed = 12f;
    public float ScrollSpeed = 2f;
    public float SmoothCameraDistance = 0.07f;
    public float СameraRadius = 0.1f;
}

public enum CamerasTypes
{
    FirstPersonCamera,
    ThirdPersonCamera,
}


public interface IThirdPersonCamera
{
    Vector3 Position { get; } // центр системы, обычно должен совпадить с центром тела персонажа
    Camera Camera { get; } // камера с Transform
    Vector3 PointAimCamera { get; } // точка куда смотрит камера (чтобы камера была сбоку от плеча персонажа)

    float MinCameraDistance { get; set; } // Минимальное расстояние (если расстояние меньше чем минимальное камера переключится на режим от 1 лица)
    float MaxCameraDistance { get; set; } // Максимальное расстояние (при изменении растояния коллесиком мыши камера не может быть от PointAimCamera дальше)
    bool IsMinCameraDistance { get; } // камера переключится на режим от 1 лица

    float LookSpeed { get; } // скорость вращения

    float ScrollSpeed { get; } // Скорость изменения дистанции при изменении растояния коллесиком мыши
    float SmoothCameraDistance { get; } // Переменная для сглаживания движения при изменении растояния коллесиком мыши

    float СameraRadius { get; } // радиус луча от головы к камере

    void Update(Vector2 lookInput);
    void OnScrollInputPerformed(float value);
}

public interface IFirstPersonCamera
{
    Vector3 Position { get; } // центр системы, обычно должен совпадить с центром тела персонажа
    Camera Camera { get; } // камера с Transform
    Vector3 PointCamera { get; } // точка где находится камера (чтобы камера была на лице персонажа)
    float LookSpeed { get; } // скорость вращения
    void Update(Vector2 lookInput);
}



// float PointAimCameraX { get; set; } // сдвиг по X относительно Position (обычно используется чтобы камера была на уровне головы персонажа)
// float PointAimCameraY { get; set; } // сдвиг по Y относительно Position (обычно используется чтобы камера была сбоку от плеча персонажа)
// Vector3 PointAimCamera { get; } // точка куда смотрит камера (чтобы камера была сбоку от плеча персонажа)