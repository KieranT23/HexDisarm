using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

public class GridGenerator3D : MonoBehaviour
{
#region Variables

#region Static

    public static GridGenerator3D Instance;

#endregion

#region Editor

    [SerializeField] private GridTile3D gridTilePrefab;

    [SerializeField] private Transform hexContent;

    [SerializeField] private GameObject holderPrefab;

    [SerializeField] private CanvasGroup gridCanvasGroup;

    [SerializeField] private Transform background;

    [SerializeField] private Transform background1;

    [SerializeField] private Transform background2;

    [SerializeField] private MeshRenderer background1Renderer;

    [SerializeField] private MeshRenderer background2Renderer;

    [SerializeField] private Material[] backgroundColours;

    [SerializeField] private Transform unusedObjectsHolder;

    [SerializeField] private GameObject loadingCanvas;

#endregion

#region Public

    public bool IsGeneratingGrid { get; private set; }
    public bool IsAnimating;

#endregion

#region Private

    private List<GridTile3D> gridTiles = new List<GridTile3D>();

    private List<GameObject> holders = new List<GameObject>();

    private Vector3 top = new Vector3(1.8f, 0, 0f);
    private Vector3 topRight = new Vector3(0.9f, 0, -1.55f);
    private Vector3 bottomRight = new Vector3(-0.9f, 0, -1.55f);
    private Vector3 bottom = new Vector3(-1.8f, 0, -0f);
    private Vector3 bottomLeft = new Vector3(-0.9f, 0, 1.55f);
    private Vector3 topLeft = new Vector3(0.9f, 0, 1.55f);
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
    private List<GridTile3D> instantiatedBombs = new List<GridTile3D>();

    private int timesGenerated = 0;

    private int background1Colour = 0;
    private int background2Colour = 0;

    private List<GridTile3D> unusedTiles = new List<GridTile3D>();

    private const int maxGridSize = 8;

    private int previousGridSize = 0;

#endregion

#endregion

#region Methods

#region Unity

    private void Awake()
    {
        loadingCanvas.SetActive(true);
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(this);
        }

        StartCoroutine(CreatePool());
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
        foreach (GridTile3D tile in unusedTiles)
        {
            tile.GetComponent<MeshCollider>().enabled = blockRaycasts;
        }
    }

    public IEnumerator GenerateGrid(List<int> levelInfo, bool levelCompleted = false)
    {
        hexContent.gameObject.SetActive(true);
        if (levelCompleted)
        {
            AnimateLevelComplete();
            yield return new WaitForSeconds(0.5f);
        }
            
        GrabGridFromPool(levelInfo[0]);
        //hexContent.transform.position = new Vector3(150f, -187f, 20.24f);
        SetBlocksRaycasts(false);
        //gridCanvasGroup.blocksRaycasts = false;
        timesGenerated++;
        PlayerInfoManager.Instance.TimesGridGeneratedSinceFeedback++;
        if (levelCompleted)
        {
            if (timesGenerated >= 5 && ((!GameManager.Instance.IsRandomLevel && GameManager.Instance.CurrentLevel >= 10) || GameManager.Instance.IsRandomLevel))
            {
                timesGenerated = 0;
                AdManager.Instance.ShowInGameAd();
                yield return null;
                yield return new WaitUntil(() => !AdManager.Instance.IsShowingAd);
            }
            else if (PlayerInfoManager.Instance.TimesGridGeneratedSinceFeedback >= 13 &&
                     !PlayerInfoManager.Instance.HasProvidedFeedback)
            {
                PlayerInfoManager.Instance.TimesGridGeneratedSinceFeedback = 0;

#if UNITY_ANDROID
                PopupManager.Instance.ShowFeedbackPopup();
#elif UNITY_IOS
                
                if (PlayerInfoManager.Instance.AmountOfTimesShownReviewPopup < 3)
                {
                    PlayerInfoManager.Instance.AmountOfTimesShownReviewPopup++;
                    Device.RequestStoreReview()
                }
                
#endif
            }
        }

        SetGridScale(levelInfo[0]);
        IsGeneratingGrid = true;
        /*if (hasGeneratedBefore)
        {
            yield return AnimateQuit();

            if (levelInfo[0] != gridRadius)
            {
                yield return new WaitForSeconds(0.5f);
                DestroyGrid();
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
                instantiatedBombs = new List<GridTile3D>();
                ResetGrid();
                LevelInfo = levelInfo;

                gridRadius = LevelInfo[0];
                numberOfBombs = LevelInfo[1];
                if (levelInfo.Count > 2)
                    numberOfBombsToShow = LevelInfo[2];
                yield break;
            }

        }*/

        LevelInfo = levelInfo;

        gridRadius = LevelInfo[0];
        numberOfBombs = LevelInfo[1];
        if (levelInfo.Count > 2)
            numberOfBombsToShow = LevelInfo[2];
        else
        {
            numberOfBombsToShow = 0;
        }

        StartCoroutine(WaitBeforeNeighbours());
    }

    public void ResetGrid()
    {
        foreach (GridTile3D tile in gridTiles)
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

        gridTiles = new List<GridTile3D>();
        holders = new List<GameObject>();
        posOfObjects = new List<Vector3>();
    }

    public GridTile3D GetTileAtPosition(Vector3 pos)
    {
        foreach (GridTile3D tile in gridTiles)
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
        /*hexContent.transform.position = new Vector3(150f, -187f, 20.24f);*/
        LeanTween.move(hexContent.gameObject, new Vector3(0f, -30f, 170f), 0.5f).setEase(LeanTweenType.easeOutQuint);
        for (int i = 0; i < gridTiles.Count; i++)
        {
            if (i == 1 || i == 7 || i == 19 || i == 37 || i == 61 || i == 91 || i == 127)
                yield return new WaitForSeconds(0.1f);

            /*LeanTween.alphaCanvas(gridTiles[i].GetComponent<CanvasGroup>(), 1f, 0.2f)
                .setEase(LeanTweenType.easeInOutSine);*/
        }

        yield return null;
    }

    public void DisarmBomb(GridTile3D bomb)
    {
        instantiatedBombs.Remove(bomb);
        foreach (GridTile3D tile in instantiatedBombs)
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
        foreach (Vector3 posOfObject in posOfObjects)
        {
            if (posOfObject.x.ToString("F2") == pos.x.ToString("F2") &&
                posOfObject.z.ToString("F2") == pos.z.ToString("F2"))
                return;
        }

        if (posOfObjects.Contains(pos))
            return;

        posOfObjects.Add(pos);
        GridTile3D instantiatedObject = Instantiate(gridTilePrefab, parent == null ? hexContent : parent);
        instantiatedObject.transform.localPosition = pos;
        instantiatedObject.Position = pos;
        unusedTiles.Add(instantiatedObject);
        instantiatedObject.name = pos.ToString();

    }

    private IEnumerator WaitBeforeNeighbours()
    {
        List<int> bombs = new List<int>();
        for (int i = 0; i < numberOfBombs; i++)
        {
            int seed = 0;
            if (LevelInfo.Count > 2)
                seed = LevelInfo[3 + i];

            int bomb = 0;
            do
            {
                if (GameManager.Instance.IsRandomLevel)
                    Random.InitState(System.Environment.TickCount);
                else
                    Random.InitState(seed);
                bomb = Random.Range(0, gridTiles.Count);
                seed += 100;
            } while (bombs.Contains(bomb));

            bombs.Add(bomb);
        }

        StartCoroutine(AnimateInGrid());
        StartCoroutine(UIController.Instance.AnimateInUI());

        if (LevelInfo.Count > 2)
        {
            int level = GameManager.Instance.CurrentLevel;
            if (level == 1)
                UIController.Instance.ShowTutorialTip(0);
            else if (level == 2)
            {
                UIController.Instance.ShowTutorialTip(1);
            }
            else if (level == 5)
            {
                SetGridScale(5);
                UIController.Instance.ShowTutorialTip(2);
            }
        }


        yield return new WaitForSeconds(0.5f);


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
        SetBlocksRaycasts(true);
        yield return null;
    }

    private void SetGridScale(int radius)
    {
        float scale = 0;
        switch (radius)
        {
            case 2:
                scale = 10;
                break;
            case 3:
                scale = 8;
                break;
            case 4:
                scale = 6;
                break;
            case 5:
                scale = 5.5f;
                break;
            case 6:
                scale = 4.5f;
                break;
            case 7:
                scale = 4;
                break;
            case 8:
                scale = 3.5f;
                break;
            case 9:
                scale = 3f;
                break;
        }

        hexContent.localScale = Vector3.one * scale;
    }

    private IEnumerator AnimateQuit()
    {
        //LeanTween.moveX(hexContent.gameObject, 150f, 0.5f).setEase(LeanTweenType.easeInSine);
        SetBlocksRaycasts(false);
        gridCanvasGroup.blocksRaycasts = false;
        LeanTween.moveX(hexContent.gameObject, 200f, 0.5f).setEase(LeanTweenType.easeInQuint).setOnComplete(() =>
        {
            hexContent.position = new Vector3(0f, 200f, 170f);
        });

        yield return new WaitForSeconds(0.5f);
        SetBlocksRaycasts(true);
        gridCanvasGroup.blocksRaycasts = true;
        hexContent.gameObject.SetActive(false);
    }

    public IEnumerator AnimateSkip(List<int> levelInfo)
    {
        yield return AnimateQuit();
        StartCoroutine(GenerateGrid(levelInfo));
    }

    public IEnumerator AnimateCentreBombExplosion()
    {
        yield return gridTiles[0].AnimateBombExplosion(false);
    }

    public void AnimateLevelComplete()
    {
        LeanTween.moveY(hexContent.gameObject, -200f, 0.5f).setEase(LeanTweenType.easeInQuint).setOnComplete(() =>
        {
            hexContent.position = new Vector3(0f, 200f, 170f);
        });

        LeanTween.moveY(background.gameObject, -600f, 1f).setEase(LeanTweenType.easeInOutQuint).setOnComplete(() =>
        {
            background.position = new Vector3(0f, 0f, 100f);
            SetBackgroundColours(true, false);
        });
    }

    public Color SetBackgroundColours(bool isSpawningGrid, bool start)
    {
        if (start)
        {
            background1Colour = Random.Range(0, backgroundColours.Length);
            background1Renderer.material = backgroundColours[background1Colour];
            do
            {
                Random.InitState(System.Environment.TickCount);
                background2Colour = Random.Range(0, backgroundColours.Length);

            } while (background2Colour == background1Colour);

            background2Renderer.material = backgroundColours[background2Colour];
        }
        else if (isSpawningGrid)
        {
            background1Colour = background2Colour;
            background1Renderer.material = backgroundColours[background1Colour];
            do
            {
                Random.InitState(System.Environment.TickCount);
                background2Colour = Random.Range(0, backgroundColours.Length);

            } while (background2Colour == background1Colour);

            background2Renderer.material = backgroundColours[background2Colour];
        }

        UIController.Instance.SetTextColour(background1Colour);

        return background1Renderer.material.color;
    }

    private IEnumerator CreatePool()
    {
        GridTile3D centreTile = GameObject.Instantiate(gridTilePrefab, unusedObjectsHolder);
        Vector3 centrePos = centreTile.transform.localPosition;
        centreTile.name = centrePos.ToString();
        unusedTiles.Add(centreTile);
        posOfObjects.Add(centrePos);

        for (int i = 1; i < maxGridSize - 1; i++)
        {
            GameObject holder = Instantiate(holderPrefab, unusedObjectsHolder);
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
                float zDif = positions[oneBefore].z - positions[a].z;

                xDif /= i;
                zDif /= i;

                for (int b = 1; b < i + 1; b++)
                {
                    CreateGrid(new Vector3(positions[a].x + (xDif * b), 0, positions[a].z + (zDif * b)),
                        holder.transform);
                }
            }
        }

        yield return new WaitForSeconds(0.5f);

        for (int a = 0; a < unusedTiles.Count; a++)
        {
            unusedTiles[a].GetNeighbours(unusedTiles);
        }

        unusedObjectsHolder.gameObject.SetActive(false);
        loadingCanvas.gameObject.SetActive(false);
    }

    private void GrabGridFromPool(int radius)
    {
        gridTiles = new List<GridTile3D>();

        for (int i = 0; i < holders.Count; i++)
        {
            holders[i].transform.SetParent(radius >= i + 3 ? hexContent : unusedObjectsHolder);
        }

        for (int i = 0; i < unusedTiles.Count; i++)
        {
            unusedTiles[i].ResetTile();
            if (i < 1)
            {
                if (radius >= 2)
                    gridTiles.Add(unusedTiles[i]);
            }
            else if (i < 7)
            {
                if (radius >= 3)
                    gridTiles.Add(unusedTiles[i]);
            }
            else if (i < 19)
            {
                if (radius >= 4)
                    gridTiles.Add(unusedTiles[i]);
            }
            else if (i < 37)
            {
                if (radius >= 5)
                    gridTiles.Add(unusedTiles[i]);
            }
            else if (i < 61)
            {
                if (radius >= 6)
                    gridTiles.Add(unusedTiles[i]);
            }
            else if (i < 91)
            {
                if (radius >= 7)
                    gridTiles.Add(unusedTiles[i]);
            }
            else
            {
                if (radius >= 8)
                    gridTiles.Add(unusedTiles[i]);
            }

        }

        if (radius >= 2)
        {
            unusedTiles[0].transform.parent = hexContent;
            gridTiles.Add(unusedTiles[0]);
        }
    }

    public IEnumerator AnimateGridIntoView()
    {
        //LeanTween.moveX(hexContent.gameObject, 150f, 0.5f).setEase(LeanTweenType.easeInSine);
        /*SetBlocksRaycasts(false);
        gridCanvasGroup.blocksRaycasts = false;
        hexContent.position = new Vector3(200f, 200f, 170f);
        LeanTween.moveX(hexContent.gameObject, 0f, 0.5f).setEase(LeanTweenType.easeOutQuint).setOnComplete(() =>
        {
            hexContent.position = new Vector3(0f, 200f, 170f);
        });

        yield return new WaitForSeconds(0.5f);
        SetBlocksRaycasts(true);
        gridCanvasGroup.blocksRaycasts = true;*/
        yield return null;
    }


public void AnimateBackgroundIn()
    {

    }

    public void AnimateBackgroundOut()
    {

    }
#endregion
#endregion
}
