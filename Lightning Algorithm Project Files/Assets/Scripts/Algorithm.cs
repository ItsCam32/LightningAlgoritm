using System.Collections;
using UnityEngine;
using TMPro;

public class Algorithm : MonoBehaviour
{
    [SerializeField] GameObject emptySquare, filledSquare, lightningSquare;
    [SerializeField] Sprite lightningImage;
    [SerializeField] Material lightningMat;
    [SerializeField] Transform squareParent;

    private int[] directions = new int[] {1, -1, 87, -87 };
    private int totalRows = 11;
    private int totalColumns = 87;
    private bool viablePath = false;
    private int lowestFinishPoint = 99999;
    private GameObject lowestFinishPointGO = null;


    private void Start()
    {
        BeginAlgorithm();
    }

    private void Update()
    {
       if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
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
        Debug.Log("Reset");
        foreach (Transform square in squareParent)
        {
            Destroy(square.gameObject);
        }

        yield return new WaitForSeconds(0.1f);

        lowestFinishPoint = 99999;
        lowestFinishPointGO = null;
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
                //float startingNumber = Mathf.Ceil(totalColumns / 2.0f) * (totalColumns - 1) + (totalColumns / 2);

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
                        GameObject newSquare = GameObject.Find((int.Parse(square.gameObject.name) + direction).ToString());

                        if (newSquare != null && newSquare.tag != "Filled" && newSquare.tag != "Used" && IsConnectedSquare(square.gameObject, newSquare))
                        {
                            newSquare.GetComponent<SquareInfo>().value = (square.GetComponent<SquareInfo>().value + 1);
                            newSquare.GetComponent<SquareInfo>().UpdateText((square.GetComponent<SquareInfo>().value + 1));
                            newSquare.tag = "Available";
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
                lowestFinishPointGO = square.gameObject;
            }
        }
    }

    private IEnumerator DisplayShortestPath()
    {
        GameObject previousSquare = lowestFinishPointGO;
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

        yield return new WaitForSeconds(5);
        StartCoroutine(Reset());
    }

    private bool IsConnectedSquare(GameObject originalSquare, GameObject newSquare)
    {
        if (Vector3.Distance(originalSquare.transform.position, newSquare.transform.position) < 4)
        {
            return true;
        }

        return false;
    }
}
