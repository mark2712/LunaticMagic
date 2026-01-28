using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ResourceSystem;
using UniRx;
using UnityEngine;
using UniVRM10;


public sealed class SpawnVrm : IDisposable
{
    private readonly List<IResourceBinding<Vrm10Instance>> _bindings = new();
    private readonly List<Vrm10Instance> _instances = new();

    private float _currentZ = 3f;

    // реактивно: есть ли хотя бы одна модель
    public readonly ReactiveProperty<bool> HasAny = new(false);

    // последняя заспавненная
    public Vrm10Instance Last => _instances.Count > 0 ? _instances[^1] : null;

    // ─────────────────────────────

    public async Task SpawnOneAsync(string path)
    {
        var instance = await LoadAsync(path);
        Place(instance);
    }

    public async Task SpawnAllAsync(string directory)
    {
        if (!Directory.Exists(directory))
            return;

        var files = Directory.GetFiles(directory, "*.vrm");
        foreach (var path in files)
        {
            var instance = await LoadAsync(path);
            Place(instance);
        }
    }

    public void CloneLast()
    {
        if (Last == null)
            return;

        var clone = GameObject.Instantiate(Last.gameObject);
        clone.SetActive(true);

        clone.transform.position = new Vector3(0, 0, _currentZ);
        _currentZ += 2f;
    }

    // ─────────────────────────────

    private async Task<Vrm10Instance> LoadAsync(string path)
    {
        var binding = await ResourceManager.FileVRM.BindAsync(path);
        var instance = binding.Resource;

        instance.gameObject.SetActive(true);

        _bindings.Add(binding);
        _instances.Add(instance);
        HasAny.Value = true;

        return instance;
    }

    private void Place(Vrm10Instance instance)
    {
        instance.gameObject.transform.position = new Vector3(0, 0, _currentZ);
        _currentZ += 2f;
    }

    public void Dispose()
    {
        foreach (var b in _bindings)
            b.Dispose();

        _bindings.Clear();
        _instances.Clear();
        HasAny.Value = false;
    }
}



// public class SpawnVrm : IDisposable
// {
//     private IResourceBinding<Vrm10Instance> _binding;

//     // Реактивное свойство для отслеживания готового экземпляра
//     public readonly ReactiveProperty<Vrm10Instance> Instance = new ReactiveProperty<Vrm10Instance>(null);

//     public async Task<Vrm10Instance> SpawnAsync(string path)
//     {
//         _binding = await ResourceManager.FileVRM.BindAsync(path);

//         var instance = _binding.Resource;
//         if (instance == null)
//         {
//             Debug.LogError("VRM instance == null");
//             return null;
//         }

//         instance.gameObject.SetActive(false); // изначально выключен
//         Instance.Value = instance;             // уведомляем подписчиков
//         return instance;
//     }

//     public void Dispose()
//     {
//         _binding?.Dispose();
//     }
// }


// public class SpawnVrm : IDisposable
// {
//     private IResourceBinding<Vrm10Instance> _binding;

//     public async void Spawn()
//     {
//         string path = Path.Combine(DataPathManager.ModelsVRM, "mainVRM1.vrm");
//         Vrm10Instance instance = await SpawnAsync(path);
//         instance.gameObject.transform.position = new(0, 0, 3);
//         instance.gameObject.SetActive(false);
//     }

//     private async Task<Vrm10Instance> SpawnAsync(string path)
//     {
//         // Асинхронно привязываем ресурс
//         _binding = await ResourceManager.FileVRM.BindAsync(path);

//         var instance = _binding.Resource;
//         if (instance == null)
//         {
//             Debug.LogError("VRM instance == null");
//             return null;
//         }
//         return instance;
//     }

//     public void Dispose()
//     {
//         _binding?.Dispose();
//     }
// }