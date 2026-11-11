using System.Collections.Generic;
using UnityEngine;

public class EmotionController
{
    private readonly SkinnedMeshRenderer _faceRenderer;
    private readonly List<string> _blendShapeNames;
    private readonly Dictionary<string, float> _currentBlendShapeValues;
    private Dictionary<string, float> _targetEmotionValues = new Dictionary<string, float>();
    private string _currentEmotionName;
    private float _currentTransitionSpeed;

    public EmotionController(Transform playerModel)
    {
        Transform faceTransform = playerModel.Find("Face");
        _faceRenderer = faceTransform.GetComponent<SkinnedMeshRenderer>();

        if (_faceRenderer == null || _faceRenderer.sharedMesh == null)
        {
            Debug.LogError("SkinnedMeshRenderer not found or mesh missing!");
            return;
        }

        _blendShapeNames = GetBlendShapeNames();
        _currentBlendShapeValues = new Dictionary<string, float>(_blendShapeNames.Count);

        foreach (var name in _blendShapeNames)
        {
            _currentBlendShapeValues[name] = 0f;
        }
    }

    /// <summary>
    /// Устанавливает эмоцию из базы по её имени.
    /// </summary>
    public void SetEmotion(string emotionName, float transitionSpeed = 1000f)
    {
        if (string.IsNullOrEmpty(emotionName)) return;

        // Если та же эмоция и скорость не изменилась — выходим
        if (_currentEmotionName == emotionName && Mathf.Approximately(_currentTransitionSpeed, transitionSpeed))
        {
            return;
        }

        if (!EmotionDatabase.Emotions.TryGetValue(emotionName, out var emotionData))
        {
            Debug.LogWarning($"Emotion '{emotionName}' not found!");
            return;
        }

        SetEmotion(emotionData, transitionSpeed);
        _currentEmotionName = emotionName;
    }

    /// <summary>
    /// Устанавливает произвольную эмоцию, переданную вручную (без записи имени).
    /// </summary>
    public void SetEmotion(Emotion emotion, float transitionSpeed = 1000f)
    {
        if (emotion == null) return;

        _targetEmotionValues = new Dictionary<string, float>(emotion.blendShapeValues);
        _currentEmotionName = null; // Не запоминаем название эмоции
        _currentTransitionSpeed = transitionSpeed;
    }

    public void LateUpdate()
    {
        if (_currentTransitionSpeed <= 0f) return;

        float maxDelta = _currentTransitionSpeed * Time.deltaTime;

        foreach (var blendShape in _blendShapeNames)
        {
            float current = _currentBlendShapeValues[blendShape];
            float target = _targetEmotionValues.TryGetValue(blendShape, out float value) ? value : 0f;

            if (!Mathf.Approximately(current, target))
            {
                _currentBlendShapeValues[blendShape] = Mathf.MoveTowards(current, target, maxDelta);
            }
        }

        ApplyCurrentValues();
    }

    private void ApplyCurrentValues()
    {
        foreach (var blendShape in _blendShapeNames)
        {
            int index = _faceRenderer.sharedMesh.GetBlendShapeIndex(blendShape);
            if (index != -1)
            {
                _faceRenderer.SetBlendShapeWeight(index, _currentBlendShapeValues[blendShape]);
            }
        }
    }

    private List<string> GetBlendShapeNames()
    {
        List<string> names = new List<string>();
        var mesh = _faceRenderer.sharedMesh;
        for (int i = 0; i < mesh.blendShapeCount; i++)
        {
            names.Add(mesh.GetBlendShapeName(i));
        }
        return names;
    }
}
