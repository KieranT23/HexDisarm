using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

public class GridGenerator : MonoBehaviour
{

    #region Variables
    #region Static
    public static GridGenerator Instance;
    #endregion
    #region Editor
    [SerializeField] private GridTile gridTilePrefab;

    [SerializeField] private RectTransform hexContent;

    [SerializeField] private GameObject holderPrefab;

    [SerializeField] private CanvasGroup gridCanvasGroup;
    #endregion
    #region Public
    public bool IsGeneratingGrid { get; private set; }
    public bool IsAnimating;
    #endregion
    #region Private
    private List<GridTile> gridTiles = new List<GridTile>();

    private List<GameObject> holders = new List<GameObject>();

    private Vector2 top = new Vector2(0, 220);
    private Vector2 topRight = new Vector2(200, 110);
    private Vector2 bottomRight = new Vector2(200, -110);
    private Vector2 bottom = new Vector2(0, -220);
    private Vector2 bottomLeft = new Vector2(-200, -110);
    private Vector2 topLeft = new Vector2(-200, 110);
    private List<Vector3> posOfObjects = new List<Vector3>();

    private int gridRadius = 6;

    private int numberOfBombs = 4;

    private float startPos = 0;
    private float xMovementAmount = 200;
    private float yMovementAmount = 110;

    private List<int> LevelInfo = new List<int>();

    private int seed = 1;
    private int numberOfBombsToShow = 0;
    private bool hasGeneratedBefore = false;
    private List<GridTile> instantiatedBombs = new List<GridTile>();

    private int timesGenerated = 0;
    
    #endregion
    #endregion

    #region Methods
    #region Unity
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(this);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetGrid();
        }
    }
    #endregion
    #region Public

    public void SetBlocksRaycasts(bool blockRaycasts)
    {
        gridCanvasGroup.blocksRaycasts = blockRaycasts;
    }

    public IEnumerator GenerateGrid(List<int> levelInfo, bool levelCompleted = false)
    {
        gridCanvasGroup.blocksRaycasts = false;
        timesGenerated++;
        PlayerInfoManager.Instance.TimesGridGeneratedSinceFeedback++;
        if (levelCompleted)
        {
            if (timesGenerated >= 5)
            {
                timesGenerated = 0;
                AdManager.Instance.ShowInGameAd();
                yield return null;
                yield return new WaitUntil(() => !AdManager.Instance.IsShowingAd);
            }
            else if (PlayerInfoManager.Instance.TimesGridGeneratedSinceFeedback >= 13)
            {
                PlayerInfoManager.Instance.TimesGridGeneratedSinceFeedback = 0;
                PopupManager.Instance.ShowFeedbackPopup();
            }
        }
        SetGridScale(levelInfo[0]);
        IsGeneratingGrid = true;
        if (hasGeneratedBefore)
        {
            yield return AnimateQuit();

            if (levelInfo[0] != gridRadius)
            {
                yield return new WaitForSeconds(0.5f);
                DestroyGrid();
                yield return new WaitForSeconds(2f);
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
                instantiatedBombs = new List<GridTile>();
                ResetGrid();
                LevelInfo = levelInfo;

                gridRadius = LevelInfo[0];
                numberOfBombs = LevelInfo[1];
                numberOfBombsToShow = LevelInfo[2];
                yield break;
            }

        }

        LevelInfo = levelInfo;

        gridRadius = LevelInfo[0];
        numberOfBombs = LevelInfo[1];
        numberOfBombsToShow = LevelInfo[2];

        GridTile centreTile = GameObject.Instantiate(gridTilePrefab, hexContent);
        Vector3 centrePos = ((RectTransform)centreTile.transform).anchoredPosition;
        centreTile.name = centrePos.ToString();
        gridTiles.Add(centreTile);
        posOfObjects.Add(centrePos);

        for (int i = 1; i < gridRadius - 1; i++)
        {
            GameObject holder = Instantiate(holderPrefab, hexContent);
            holder.name = i.ToString();
            holders.Add(holder);
            Vector3[] positions =
            {
                top * i,
                topRight * i,
                bottomRight * i,
                bottom * i,
                bottomLeft * i,
                topLeft * i
            };

            int amountOfGaps = i - 1;

            for (int a = 0; a < 6; a++)
            {
                CreateGrid(positions[a], holder.transform);

                if (i == 1)
                    continue;

                int oneBefore = a - 1 < 0 ? 5 : a - 1;
                float xDif = positions[oneBefore].x - positions[a].x;
                float yDif = positions[oneBefore].y - positions[a].y;

                xDif /= i;
                yDif /= i;

                for (int b = 1; b < i + 1; b++)
                {
                    CreateGrid(new Vector3(positions[a].x + (xDif * b), positions[a].y + (yDif * b), 0f), holder.transform);
                }
            }
        }

        StartCoroutine(WaitBeforeNeighbours());
    }

    public void ResetGrid()
    {
        foreach (GridTile tile in gridTiles)
            tile.ResetTile();

        StartCoroutine(WaitBeforeNeighbours());
    }

    public void DestroyGrid()
    {
        for (int i = gridTiles.Count - 1; i >= 0; i--)
        {
            DestroyImmediate(gridTiles[i].gameObject);
        }
        for (int i = holders.Count - 1; i >= 0; i--)
            DestroyImmediate(holders[i]);

        gridTiles = new List<GridTile>();
        holders = new List<GameObject>();
        posOfObjects = new List<Vector3>();
    }

    public GridTile GetTileAtPosition(Vector3 pos)
    {
        foreach (GridTile tile in gridTiles)
        {
            if (tile.name == pos.ToString())
                return tile;
        }

        return null;
    }

    public void SetInteractability(bool interactable)
    {
        hexContent.GetComponent<CanvasGroup>().blocksRaycasts = interactable;
    }

    public IEnumerator AnimateInGrid()
    {
        for (int i = 0; i < gridTiles.Count; i++)
        {
            if (i == 1 || i == 7 || i == 19 || i == 37 || i == 61 || i == 91 || i == 127)
                yield return new WaitForSeconds(0.1f);

            LeanTween.alphaCanvas(gridTiles[i].GetComponent<CanvasGroup>(), 1f, 0.2f)
                .setEase(LeanTweenType.easeInOutSine);
        }
        yield return null;
    }

    public void DisarmBomb(GridTile bomb)
    {
        instantiatedBombs.Remove(bomb);
        foreach (GridTile tile in instantiatedBombs)
            tile.SetBomb();
    }

    public void QuitGame()
    {
        StartCoroutine(AnimateQuit());
    }
    #endregion
    #region Private
    private void CreateGrid(Vector3 pos, Transform parent = null)
    {
        if (posOfObjects.Contains(pos))
            return;

        posOfObjects.Add(pos);
        GridTile instantiatedObject = Instantiate(gridTilePrefab, parent == null ? hexContent : parent);
        ((RectTransform) instantiatedObject.transform).anchoredPosition = pos;
        instantiatedObject.Position = pos;
        gridTiles.Add(instantiatedObject);
        instantiatedObject.name = pos.ToString();
        
    }

    private IEnumerator WaitBeforeNeighbours()
    {
        yield return new WaitForSeconds(1.0f);

        List<int> bombs = new List<int>();
        for (int i = 0; i < numberOfBombs; i++)
        {
            Random.InitState(LevelInfo[3 + i]);
            int bomb = 0;
            do
                bomb = Random.Range(0, gridTiles.Count);
            while (bombs.Contains(bomb));
            bombs.Add(bomb);
        }

        for (int a = 0; a < gridTiles.Count; a++)
        {
            gridTiles[a].GetNeighbours(gridTiles);
        }

        StartCoroutine(AnimateInGrid());
        StartCoroutine(UIController.Instance.AnimateInUI());
        yield return new WaitForSeconds(1f);


        int bombsShown = 0;
        for (int b = 0; b < gridTiles.Count; b++)
        {
            int incrementer = b;
            if (bombs.Contains(b))
            {
                gridTiles[b].SetBomb(bombsShown < numberOfBombsToShow);
                if (bombsShown == 0)
                    StartCoroutine(UIController.Instance.AnimateInBombsRemaining());

                bombsShown++;

                instantiatedBombs.Add(gridTiles[b]);
            }    
        }

        SetInteractability(true);
        AdManager.Instance.ShowBanner();
        hasGeneratedBefore = true;
        IsGeneratingGrid = false;
        gridCanvasGroup.blocksRaycasts = true;
    }

    private void SetGridScale(int radius)
    {
        float scale = 0;
        switch (radius)
        {
            case 3:
                scale = 0.6f;
                break;
            case 4:
                scale = 0.6f;
                break;
            case 5:
                scale = 0.5f;
                break;
            case 6:
                scale = 0.4f;
                break;
            case 7:
                scale = 0.3f;
                break;
            case 8:
                scale = 0.275f;
                break;
            case 9:
                scale = 0.25f;
                break;
        }

        hexContent.localScale = Vector3.one * scale;
    }

    private IEnumerator AnimateQuit()
    {
        gridCanvasGroup.blocksRaycasts = false;
        for (int i = gridTiles.Count - 1; i >= 0; i--)
        {
            if (i == 1 || i == 7 || i == 19 || i == 37 || i == 61 || i == 91 || i == 127)
                yield return new WaitForSeconds(0.05f);

            LeanTween.scale(gridTiles[i].gameObject, Vector3.zero, 0.1f)
                .setEase(LeanTweenType.easeInSine);
        }

        yield return new WaitForSeconds(0.5f);
        gridCanvasGroup.blocksRaycasts = true;
    }

    public IEnumerator AnimateSkip(List<int> levelInfo)
    {
        yield return AnimateQuit();
        StartCoroutine(GenerateGrid(levelInfo));
    }
    #endregion
    #endregion 
}
