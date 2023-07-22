using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SquareInfo : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI valueText;

    public int value = 0;

    public void UpdateText(int val)
    {
        valueText.text = val.ToString();
    }
}
