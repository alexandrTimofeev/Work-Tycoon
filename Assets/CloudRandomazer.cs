using UnityEngine;
using DG.Tweening;

public class CloudRandomazer : MonoBehaviour
{
    [Header("Scale")]
    [SerializeField] private Vector2 scaleCoefMinMax = new Vector2(0.5f, 2f);
    [SerializeField] private float scaleDuration = 1f;
    [SerializeField] private float scalePause = 1f;

    [Header("Position")]
    [SerializeField] private Vector2 positionOffsetMinMax = new Vector2(0.2f, 1f);
    [SerializeField] private float positionDuration = 1f;
    [SerializeField] private float positionPause = 1f;

    [Header("Rotation")]
    [SerializeField] private Vector2 rotationAngleMinMax = new Vector2(-10f, 10f);
    [SerializeField] private float rotationDuration = 1f;
    [SerializeField] private float rotationPause = 1f;

    private Vector3 scaleStart;
    private Vector3 positionStart;
    private Vector3 rotationStart;

    private void Start()
    {
        scaleStart = transform.localScale;
        positionStart = transform.localPosition;
        rotationStart = transform.localEulerAngles;

        StartScaleAnimation();
        StartPositionAnimation();
        //StartRotationAnimation();
    }

    private void StartScaleAnimation()
    {
        float scaleCoef = Random.Range(scaleCoefMinMax.x, scaleCoefMinMax.y);
        Vector3 scaleTarget = scaleStart * scaleCoef;

        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOScale(scaleTarget, scaleDuration).SetEase(Ease.InOutSine));
        seq.AppendInterval(scalePause);
        seq.Append(transform.DOScale(scaleStart, scaleDuration).SetEase(Ease.InOutSine));
        seq.AppendInterval(scalePause);

        seq.SetLoops(-1);
    }

    private void StartPositionAnimation()
    {
        float offset = Random.Range(positionOffsetMinMax.x, positionOffsetMinMax.y);

        Vector3 direction = new Vector3(
            Random.Range(-1f, 1f),
            0,
            Random.Range(-1f, 1f)
        ).normalized;

        Vector3 positionTarget = positionStart + direction * offset;

        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOLocalMove(positionTarget, positionDuration).SetEase(Ease.InOutSine));
        seq.AppendInterval(positionPause);
        seq.Append(transform.DOLocalMove(positionStart, positionDuration).SetEase(Ease.InOutSine));
        seq.AppendInterval(positionPause);

        seq.SetLoops(-1);
    }

    private void StartRotationAnimation()
    {
        float angle = Random.Range(rotationAngleMinMax.x, rotationAngleMinMax.y);

        Vector3 rotationTarget = rotationStart + new Vector3(0f, angle, 0f);

        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOLocalRotate(rotationTarget, rotationDuration).SetEase(Ease.InOutSine));
        seq.AppendInterval(rotationPause);
        seq.Append(transform.DOLocalRotate(rotationStart, rotationDuration).SetEase(Ease.InOutSine));
        seq.AppendInterval(rotationPause);

        seq.SetLoops(-1);
    }
}
