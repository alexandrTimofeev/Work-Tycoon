using TMPro;
using UnityEngine;

public class CellInfoGO : MonoBehaviour
{
    [SerializeField] private TMP_Text tmp;
    [SerializeField] private Renderer renderer;
    [SerializeField] private Color colorZero = Color.white;
    [SerializeField] private Color colorUp = Color.yellow;

    public void SetInfo(ReedOnlyCellInfluenceInformation info)
    {
        tmp.text = info.GetText();

        renderer.material.color = info.IsZero ? colorZero : colorUp;
    }
}
