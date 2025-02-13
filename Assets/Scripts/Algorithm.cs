using System.Collections;
using UnityEngine;
using TMPro;

public class Algorithm : MonoBehaviour
{
    // vv Private Exposed vv //

    [SerializeField]
    private GameObject emptySquare;

    [SerializeField]
    private GameObject filledSquare;

    [SerializeField]
    private GameObject lightningSquare;
    
    [SerializeField]
    private Sprite lightningImage;

    [SerializeField]
    private Material lightningMat;

    [SerializeField]
    private Transform squareParent;

    // vv Private vv //

    private int[] directions = new int[] {1, -1, 87, -87 };
    private int totalRows = 11;
    private int totalColumns = 87;
    private bool viablePath = false;
    private int lowestFinishPoint = 99999;
    private GameObject lowestFinishPointObj = null;

    ////////////////////////////////////////

    #region Private Functions

    private void Start()
    {
        BeginAlgorithm();
    }

    private void BeginAlgorithm()
    {
        GenerateGrid();
        SearchGrid();
        EvaluateGrid();

        if (viablePath)
        {
            StartCoroutine(DisplayShortestPath());
        }

        else
        {
            StartCoroutine(Reset());
        }
    }

    private IEnumerator Reset()
    {
        foreach (Transform square in squareParent)
        {
            Destroy(square.gameObject);
        }

        yield return new WaitForSeconds(0.1f);

        lowestFinishPoint = 99999;
        lowestFinishPointObj = null;
        viablePath = false;

        BeginAlgorithm();
    }

    private void GenerateGrid()
    {
        int x = 0;
        int y = 0;
        int squareNumber = 0;
        GameObject square = null;

        for (int row = 0; row < totalRows; row++)
        {
            for (int column = 0; column < totalColumns; column++)
            {
                if (squareNumber == 521)
                {
                    square = Instantiate(lightningSquare, new Vector3(x, y, 0), Quaternion.identity);
                    square.tag = "Available";
                }
                else
                {
                    switch (Random.Range(0, 3))
                    {
                        case 0: case 1:
                            square = Instantiate(emptySquare, new Vector3(x, y, 0), Quaternion.identity);
                            break;

                        case 2:
                            square = Instantiate(filledSquare, new Vector3(x, y, 0), Quaternion.identity);
                            break;
                    }
                }

                square.name = squareNumber.ToString();
                square.transform.parent = squareParent;
                squareNumber++;
                y++;
            }

            y = 0;
            x++;
        }
    }

    private void SearchGrid()
    {
        for (int i = 0; i < (totalRows * totalColumns); i++)
        {
            bool isPossible = false;

            foreach (Transform square in squareParent)
            {
                if (square.gameObject.tag == "Available")
                {
                    isPossible = true;
                    square.gameObject.tag = "Used";

                    foreach (int direction in directions)
                    {
                        GameObject newSquareObj = GameObject.Find((int.Parse(square.gameObject.name) + direction).ToString());
                        if (newSquareObj)
                        {
                            bool isConnectedSquare = Vector3.Distance(square.position, newSquareObj.transform.position) < 4;
                            if (newSquareObj.tag != "Filled" && newSquareObj.tag != "Used" && isConnectedSquare)
                            {
                                newSquareObj.GetComponent<SquareInfo>().value = (square.GetComponent<SquareInfo>().value + 1);
                                newSquareObj.GetComponent<SquareInfo>().UpdateText((square.GetComponent<SquareInfo>().value + 1));
                                newSquareObj.tag = "Available";
                            }
                        }
                    }
                }
            }

            if (!isPossible)
            {
                i = totalRows * totalColumns + 100;
            }
        }
    }

    private void EvaluateGrid()
    {
        foreach (Transform square in squareParent)
        {
            int val = square.GetComponent<SquareInfo>().value;
            if (int.Parse(square.name) % totalColumns == 0 && val < lowestFinishPoint && val > 0)
            {
                viablePath = true;
                lowestFinishPoint = square.GetComponent<SquareInfo>().value;
                lowestFinishPointObj = square.gameObject;
            }
        }
    }

    private IEnumerator DisplayShortestPath()
    {
        GameObject previousSquare = lowestFinishPointObj;
        previousSquare.transform.Find("Canvas").Find("ValueText").GetComponent<TextMeshProUGUI>().color = Color.black;
        SpriteRenderer renderer = previousSquare.GetComponent<SpriteRenderer>();
        renderer.material = lightningMat;
        renderer.sprite = lightningImage;

        for (int i = lowestFinishPoint - 1; i > 0; i--)
        {
            foreach (Transform square in squareParent)
            {
                if (square.GetComponent<SquareInfo>().value == i && Vector3.Distance(square.position, previousSquare.transform.position) < 2f)
                {
                    previousSquare = square.gameObject;
                    previousSquare.transform.Find("Canvas").Find("ValueText").GetComponent<TextMeshProUGUI>().color = Color.black;
                    SpriteRenderer rend = previousSquare.GetComponent<SpriteRenderer>();
                    rend.material = lightningMat;
                    rend.sprite = lightningImage;
                    break;
                }
            }

            yield return new WaitForSeconds(0.01f);
        }

        yield return new WaitForSeconds(2);
        StartCoroutine(Reset());
    }

    #endregion

    #region Public Functions

    public void OnExitButtonClicked()
    {
        Application.Quit();
    }

    #endregion
}
