using UnityEngine;
using TMPro;

public class SquareInfo : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI valueText;

    [HideInInspector]
    public int value = 0;

    public void UpdateText(int val)
    {
        valueText.text = val.ToString();
    }
}
