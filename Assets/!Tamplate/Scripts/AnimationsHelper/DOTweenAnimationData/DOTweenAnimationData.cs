using System;
using UnityEngine;
using DG.Tweening; // для Ease

/// <summary>
/// Базовый класс для всех анимаций DOTween
/// </summary>
[Serializable]
public class DOTweenAnimationData
{
    [Header("Основные параметры")]
    public float Duration = 0.5f;   // Длительность анимации
    public Ease Ease = Ease.Linear;  // Тип easing

    [Tooltip("Если true, анимация будет играть при старте автоматически")]
    public bool PlayOnStart = true;

    [Space, Header("Дополнительные параметры")]
    public float durationOnEnd = 0f;
}

[Serializable]
public class DOTweenPositionData : DOTweenAnimationData
{
    [Header("Позиция")]
    public Vector3 TargetPosition;
    public bool IsRelative = false;
    public bool Snapping = false;

    // Для Transform
    public Tweener TransformMove(Transform transform)
    {
        return TransformMove(transform, TargetPosition);
    }

    public Tweener TransformMove(Transform transform, Vector3 target)
    {
        return transform.DOMove(target, Duration, Snapping).SetEase(Ease);
    }

    // Для Vector3 с callback
    public Tweener PositionMove(Vector3 from, Vector3 to, Action<Vector3> onUpdate)
    {
        return DOVirtual.Vector3(from, to, Duration, v => onUpdate?.Invoke(v)).SetEase(Ease);
    }

    public Tweener PositionMove(Vector3 to, Transform transform)
    {
        return PositionMove(transform.position, to, v => transform.position = v);
    }
}

[Serializable]
public class DOTweenRotationData : DOTweenAnimationData
{
    [Header("Вращение")]
    public Vector3 TargetEulerAngles;
    public bool IsRelative = false;
    public RotateMode RotateMode = RotateMode.Fast;

    // Для Transform
    public Tweener TransformRotate(Transform transform)
    {
        return TransformRotate(transform, TargetEulerAngles);
    }

    public Tweener TransformRotate(Transform transform, Vector3 target)
    {
        return transform.DORotate(target, Duration, RotateMode).SetEase(Ease);
    }

    // Для Vector3 с callback
    public Tweener RotationMove(Vector3 from, Vector3 to, Action<Vector3> onUpdate)
    {
        return DOVirtual.Vector3(from, to, Duration, v => onUpdate?.Invoke(v)).SetEase(Ease);
    }

    public Tweener RotationMove(Vector3 to, Transform transform)
    {
        return RotationMove(transform.eulerAngles, to, v => transform.eulerAngles = v);
    }
}

[Serializable]
public class DOTweenScaleData : DOTweenAnimationData
{
    [Header("Масштаб")]
    public Vector3 TargetScale = Vector3.one;
    public bool IsRelative = false;

    // Для Transform
    public Tweener TransformScale(Transform transform)
    {
        return TransformScale(transform, TargetScale);
    }

    public Tweener TransformScale(Transform transform, Vector3 target)
    {
        return transform.DOScale(target, Duration).SetEase(Ease);
    }

    // Для Vector3 с callback
    public Tweener ScaleMove(Vector3 from, Vector3 to, Action<Vector3> onUpdate)
    {
        return DOVirtual.Vector3(from, to, Duration, v => onUpdate?.Invoke(v)).SetEase(Ease);
    }

    public Tweener ScaleMove(Vector3 to, Transform transform)
    {
        return ScaleMove(transform.localScale, to, v => transform.localScale = v);
    }
}

[Serializable]
public class DOTweenPunchData : DOTweenAnimationData
{
    [Header("Punch")]
    public Vector3 Punch = Vector3.one;
    public int Vibrato = 10;
    public float Elasticity = 1f;

    // Transform методы
    public Tweener TransformPunchPosition(Transform transform)
    {
        return transform.DOPunchPosition(Punch, Duration, Vibrato, Elasticity).SetEase(Ease);
    }

    public Tweener TransformPunchRotation(Transform transform)
    {
        return transform.DOPunchRotation(Punch, Duration, Vibrato, Elasticity).SetEase(Ease);
    }

    public Tweener TransformPunchScale(Transform transform)
    {
        return transform.DOPunchScale(Punch, Duration, Vibrato, Elasticity).SetEase(Ease);
    }

    // Пример с Vector3 и callback (для кастомной логики)
    public Tweener PunchVector(Vector3 from, Vector3 to, Action<Vector3> onUpdate)
    {
        return DOVirtual.Vector3(from, to, Duration, v => onUpdate?.Invoke(v)).SetEase(Ease);
    }
}