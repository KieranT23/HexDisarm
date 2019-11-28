using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

public class GridGenerator3D : MonoBehaviour
{
#region Variables
#region Static
    /// <summary>
    /// Static instance of this class
    /// </summary>
    public static GridGenerator3D Instance;

#endregion
#region Editor
    /// <summary>
    /// The grid tile prefab
    /// </summary>
    [SerializeField]
    private GridTile3D gridTilePrefab;
    /// <summary>
    /// The content to spawn the grid tiles in
    /// </summary>
    [SerializeField]
    private Transform hexContent;
    /// <summary>
    /// The prefab that is used to hold the grid tiles in
    /// </summary>
    [SerializeField]
    private GameObject holderPrefab;
    /// <summary>
    /// The background of the hex grid
    /// </summary>
    [SerializeField]
    private Transform background;
    /// <summary>
    /// The renderer for the first background
    /// </summary>
    [SerializeField]
    private MeshRenderer background1Renderer;
    /// <summary>
    /// The renderer for the second background
    /// </summary>
    [SerializeField]
    private MeshRenderer background2Renderer;
    /// <summary>
    /// All the background colours
    /// </summary>
    [SerializeField]
    private Material[] backgroundColours;
    /// <summary>
    /// The holder to hold the unused objects
    /// </summary>
    [SerializeField]
    private Transform unusedObjectsHolder;
    /// <summary>
    /// The canvas to hide the grid generation
    /// </summary>
    [SerializeField]
    private GameObject loadingCanvas;

#endregion

#region Public
    /// <summary>
    /// Is the grid currently being generated?
    /// </summary>
    public bool IsGeneratingGrid { get; private set; }
    /// <summary>
    /// Is the grid currently being animated?
    /// </summary>
    public bool IsAnimating;
    /// <summary>
    /// All the yellow tiles on the current level
    /// </summary>
    public List<GridTile3D> YellowTiles;
    /// <summary>
    /// All the red tiles on the current level
    /// </summary>
    public List<GridTile3D> RedTiles;
    /// <summary>
    /// All the orange tiles on the current level
    /// </summary>
    public List<GridTile3D> OrangeTiles;

#endregion

#region Private
    /// <summary>
    /// All the grid tiles that are being used
    /// </summary>
    private List<GridTile3D> gridTiles = new List<GridTile3D>();
    /// <summary>
    /// A list of the holders that are used to store the grid tiles for each layer
    /// </summary>
    private List<GameObject> holders = new List<GameObject>();
    /// <summary>
    /// The position of the top tile
    /// </summary>
    private Vector3 top = new Vector3(1.8f, 0, 0f);
    /// <summary>
    /// The position of the top right tile
    /// </summary>
    private Vector3 topRight = new Vector3(0.9f, 0, -1.55f);
    /// <summary>
    /// The position of the bottom right tile
    /// </summary>
    private Vector3 bottomRight = new Vector3(-0.9f, 0, -1.55f);
    /// <summary>
    /// The position of the bottom tile
    /// </summary>
    private Vector3 bottom = new Vector3(-1.8f, 0, -0f);
    /// <summary>
    /// The position of the bottom left tile
    /// </summary>
    private Vector3 bottomLeft = new Vector3(-0.9f, 0, 1.55f);
    /// <summary>
    /// The position of the top left tile
    /// </summary>
    private Vector3 topLeft = new Vector3(0.9f, 0, 1.55f);
    /// <summary>
    /// The position of all the objects
    /// </summary>
    private List<Vector3> posOfObjects = new List<Vector3>();
    /// <summary>
    /// The grid radius for the current level
    /// </summary>
    private int gridRadius = 6;
    /// <summary>
    /// The number of bombs on the current level
    /// </summary>
    private int numberOfBombs = 4;
    /// <summary>
    /// All the current level information
    /// </summary>
    private List<int> LevelInfo = new List<int>();
    /// <summary>
    /// The distance from the bomb to show a tile
    /// </summary>
    private int distanceFromBombToShow = 0;
    /// <summary>
    /// All the bombs in the current level
    /// </summary>
    private List<GridTile3D> instantiatedBombs = new List<GridTile3D>();
    /// <summary>
    /// The amount of times the grid has been generated this session
    /// </summary>
    private int timesGenerated = 0;
    /// <summary>
    /// The background 1 colour
    /// </summary>
    private int background1Colour = 0;
    /// <summary>
    /// The background 2 colour
    /// </summary>
    private int background2Colour = 0;
    /// <summary>
    /// All the tiles that have been generated
    /// </summary>
    private List<GridTile3D> unusedTiles = new List<GridTile3D>();
    /// <summary>
    /// The max size of the grid
    /// </summary>
    private const int maxGridSize = 8;

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
#endregion

#region Public
    /// <summary>
    /// Make sure the grid is not interactable
    /// </summary>
    /// <param name="blockRaycasts">Is interactable</param>
    public void SetBlocksRaycasts(bool blockRaycasts)
    {
        foreach (GridTile3D tile in unusedTiles)
        {
            tile.GetComponent<MeshCollider>().enabled = blockRaycasts;
        }
    }
    /// <summary>
    /// Generate the grid for the current level
    /// </summary>
    /// <param name="levelInfo">The level info for the current level</param>
    /// <param name="levelCompleted">Has a level previously been completed?</param>
    public IEnumerator GenerateGrid(List<int> levelInfo, bool levelCompleted = false)
    {
        YellowTiles = new List<GridTile3D>();
        RedTiles = new List<GridTile3D>();
        OrangeTiles = new List<GridTile3D>();

        hexContent.gameObject.SetActive(true);
        if (levelCompleted)
        {
            AnimateLevelComplete();
            yield return new WaitForSeconds(0.5f);
        }
            
        GrabGridFromPool(levelInfo[0]);
        SetBlocksRaycasts(false);
        timesGenerated++;
        PlayerInfoManager.Instance.TimesGridGeneratedSinceFeedback++;

        //Handle ads and feedback
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
                    Device.RequestStoreReview();
                }
#endif
            }
        }


        SetGridScale(levelInfo[0]);

        //Adjust the rotation for tutorial levels
        if ((GameManager.Instance.CurrentLevel == 2 || GameManager.Instance.CurrentLevel == 3) && !GameManager.Instance.IsRandomLevel)
            hexContent.transform.eulerAngles = new Vector3(0f, -90f, 75f);

        IsGeneratingGrid = true;


        LevelInfo = levelInfo;

        gridRadius = LevelInfo[0];
        numberOfBombs = LevelInfo[1];

        if (levelInfo.Count > 2)
            distanceFromBombToShow = LevelInfo[2];
        else
        {
            distanceFromBombToShow = 0;
        }

        //Set the rest of the level info
        StartCoroutine(SetLevelInfo());
    }

    /// <summary>
    /// Animate the grid into view
    /// </summary>
    public IEnumerator AnimateInGrid()
    {
        float valueToMoveTo = (GameManager.Instance.CurrentLevel == 3 || GameManager.Instance.CurrentLevel == 2) && !GameManager.Instance.IsRandomLevel ? -20f : -30f;
        LeanTween.move(hexContent.gameObject, new Vector3(0f, valueToMoveTo, 170f), 0.5f).setEase(LeanTweenType.easeOutQuint);

        yield return new WaitForSeconds(0.5f);
    }
    /// <summary>
    /// Disarm a bomb
    /// </summary>
    /// <param name="bomb">The bomb to disarm</param>
    public void DisarmBomb(GridTile3D bomb)
    {
        instantiatedBombs.Remove(bomb);
        foreach (GridTile3D tile in instantiatedBombs)
            tile.SetBomb();
    }
    /// <summary>
    /// Quit the current game
    /// </summary>
    public void QuitGame()
    {
        StartCoroutine(AnimateQuit());
    }

    /// <summary>
    /// Animate skip
    /// </summary>
    /// <param name="levelInfo">The level info of the next level</param>
    public IEnumerator AnimateSkip(List<int> levelInfo)
    {
        yield return AnimateQuit();
        StartCoroutine(GenerateGrid(levelInfo));
    }
    /// <summary>
    /// Animate the centre bomb explosion
    /// </summary>
    public IEnumerator AnimateCentreBombExplosion()
    {
        yield return gridTiles[0].AnimateBombExplosion(false);
    }
    /// <summary>
    /// Animate level completion
    /// </summary>
    public void AnimateLevelComplete()
    {
        LeanTween.moveY(hexContent.gameObject, -200f, 0.5f).setEase(LeanTweenType.easeInQuint).setOnComplete(() =>
        {
            hexContent.position = new Vector3(0f, 200f, 170f);
        });

        LeanTween.moveY(background.gameObject, -600f, 1f).setEase(LeanTweenType.easeInOutQuint).setOnComplete(() =>
        {
            background.position = new Vector3(0f, -30f, 100f);
            SetBackgroundColours(true, false);
        });
    }
    /// <summary>
    /// Set the background colours
    /// </summary>
    /// <param name="isSpawningGrid">Is the grid currently being spawned?</param>
    /// <param name="start">Is on level start?</param>
    /// <returns>The first background colour</returns>
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
    /// <summary>
    /// Set the tile colours - used when a theme is switched
    /// </summary>
    /// <param name="coloursToSet">The colours to set the tiles</param>
    public void SetTileColours(Color[] coloursToSet)
    {
        for (int i = 0; i < unusedTiles.Count; i++)
            unusedTiles[i].SetColours(coloursToSet);
    }

    #endregion

    #region Private
    /// <summary>
    /// Create a grid tile
    /// </summary>
    /// <param name="pos">The positon of the tile</param>
    /// <param name="parent">The parent of the tile</param>
    private void CreateGridTile(Vector3 pos, Transform parent = null)
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

    /// <summary>
    /// Set all the level info
    /// </summary>
    private IEnumerator SetLevelInfo()
    {
        //Generate the bombs
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
        
        //Set tutorial content
        if (LevelInfo.Count > 2)
        {
            int level = GameManager.Instance.CurrentLevel;
            if (level == 1)
            {
                UIController.Instance.ShowTutorialTip(0);
            }
            else if (level == 2)
            {
                UIController.Instance.ShowTutorialTip(1);
            }
            else if (level == 3)
            {
                SetGridScale(5);
                hexContent.transform.eulerAngles = new Vector3(0f, -90f, 75f);
                UIController.Instance.ShowTutorialTip(2);
            }
        }


        yield return new WaitForSeconds(0.5f);

        instantiatedBombs = new List<GridTile3D>();
        int bombsShown = 0;

        //Spawn the bombs
        for (int b = 0; b < gridTiles.Count; b++)
        {
            gridTiles[b].gameObject.name = b.ToString();
            int incrementer = b;
            if (bombs.Contains(b))
            {
                if (GameManager.Instance.CurrentLevel == 3 && bombsShown == 0)
                {
                    gridTiles[b].SetBomb(false, true);
                }
                else
                    gridTiles[b].SetBomb(GameManager.Instance.CurrentLevel == 1 && !GameManager.Instance.IsRandomLevel);
                if (bombsShown == 0)
                    StartCoroutine(UIController.Instance.AnimateInBombsRemaining());

                bombsShown++;

                instantiatedBombs.Add(gridTiles[b]);
            }
        }

        //Spawn more tutorial content
        if (!GameManager.Instance.IsRandomLevel)
        {
            int[] tilesNotToFlipLevel2 = new[] { 0, 1, 4, 7, 14 };
            if (GameManager.Instance.CurrentLevel == 2)
            {
                for (int b = 0; b < gridTiles.Count; b++)
                {
                    if (!tilesNotToFlipLevel2.Contains(b))
                    {
                        gridTiles[b].FlipForTutorial();
                    }
                }
            }
            else if (GameManager.Instance.CurrentLevel == 3)
            {
                int[] tilesToFlip = new[] { 7, 8, 9, 6, 4, 12, 15, 14 };
                for (int b = 0; b < gridTiles.Count; b++)
                {
                    if (tilesToFlip.Contains(b))
                    {
                        gridTiles[b].FlipForTutorial();
                    }
                }
            }

            //Show a tile
            if (GameManager.Instance.CurrentLevel > 3 && distanceFromBombToShow != 0)
            {
                int randomTile = 0;
                switch (distanceFromBombToShow)
                {
                    case 1:
                        randomTile = Random.Range(0, RedTiles.Count);
                        RedTiles[randomTile].FlipForTutorial();
                        break;
                    case 2:
                        randomTile = Random.Range(0, OrangeTiles.Count);
                        OrangeTiles[randomTile].FlipForTutorial();
                        break;
                    case 3:
                        randomTile = Random.Range(0, YellowTiles.Count);
                        YellowTiles[randomTile].FlipForTutorial();
                        break;
                }
            }
        }

        AdManager.Instance.ShowBanner();
        IsGeneratingGrid = false;
        SetBlocksRaycasts(true);
        yield return null;
    }

    //Set the grid scale
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


        if (radius == 2 || radius == 3)
            hexContent.transform.eulerAngles = new Vector3(0f, -90f, 75f);
        else
            hexContent.transform.eulerAngles = new Vector3(0f, -90f, 65f);

        hexContent.localScale = Vector3.one * scale;
    }

    /// <summary>
    /// Animate the quit of a level
    /// </summary>
    private IEnumerator AnimateQuit()
    {
        SetBlocksRaycasts(false);
        LeanTween.moveX(hexContent.gameObject, 200f, 0.5f).setEase(LeanTweenType.easeInQuint).setOnComplete(() =>
        {
            hexContent.position = new Vector3(0f, 200f, 170f);
        });

        yield return new WaitForSeconds(0.5f);

        SetBlocksRaycasts(true);
        hexContent.gameObject.SetActive(false);
    }

    /// <summary>
    /// Instantiate the pool to use for the grid
    /// </summary>
    private IEnumerator CreatePool()
    {
        //Instantiate the centre tile
        GridTile3D centreTile = GameObject.Instantiate(gridTilePrefab, unusedObjectsHolder);
        Vector3 centrePos = centreTile.transform.localPosition;
        centreTile.name = centrePos.ToString();
        unusedTiles.Add(centreTile);
        posOfObjects.Add(centrePos);

        //Generate the grid
        for (int i = 1; i < maxGridSize - 1; i++)
        {
            //Generate the holder for this layer
            GameObject holder = Instantiate(holderPrefab, unusedObjectsHolder);
            holder.name = i.ToString();
            holders.Add(holder);

            //The position of all the objects on this layer
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
                //Create the tile to form the web
                CreateGridTile(positions[a], holder.transform);

                if (i == 1)
                    continue;

                //Fill in all the gaps in the web
                int oneBefore = a - 1 < 0 ? 5 : a - 1;
                float xDif = positions[oneBefore].x - positions[a].x;
                float zDif = positions[oneBefore].z - positions[a].z;

                xDif /= i;
                zDif /= i;

                for (int b = 1; b < i + 1; b++)
                {
                    CreateGridTile(new Vector3(positions[a].x + (xDif * b), 0, positions[a].z + (zDif * b)),
                        holder.transform);
                }
            }
        }

        yield return new WaitForSeconds(0.5f);

        for (int a = 0; a < unusedTiles.Count; a++)
            unusedTiles[a].GetNeighbours(unusedTiles);

        ColourManager.Instance.SwitchTheme(PlayerInfoManager.Instance.CurrentColourTheme);

        unusedObjectsHolder.gameObject.SetActive(false);
        loadingCanvas.gameObject.SetActive(false);
    }

    /// <summary>
    /// Grab all the tiles that are needed from the grid pool
    /// </summary>
    /// <param name="radius">The radius of the grid to generate</param>
    private void GrabGridFromPool(int radius)
    {
        gridTiles = new List<GridTile3D>();
        instantiatedBombs = new List<GridTile3D>();

        //Set all the parents
        for (int i = 0; i < holders.Count; i++)
            holders[i].transform.SetParent(radius >= i + 3 ? hexContent : unusedObjectsHolder);

        //Add the tiles to the grid tiles list if they are useds
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
            unusedTiles[0].transform.parent = hexContent;
    }

    /// <summary>
    /// Add the tile to the red, yellow or orange lists
    /// </summary>
    /// <param name="tileToAdd">The tile to add</param>
    /// <param name="colourIndex">The index of the tile to add - 0 = red, 1 = orange, 2 = yellow</param>
    public void AddTileToList(GridTile3D tileToAdd, int colourIndex)
    {
        switch (colourIndex)
        {
            case 0:
                if (!RedTiles.Contains(tileToAdd))
                    RedTiles.Add(tileToAdd);

                if (YellowTiles.Contains(tileToAdd))
                    YellowTiles.Remove(tileToAdd);
                if (OrangeTiles.Contains(tileToAdd))
                    OrangeTiles.Remove(tileToAdd);
                break;
            case 1:
                if (!OrangeTiles.Contains(tileToAdd))
                    OrangeTiles.Add(tileToAdd);
                if (YellowTiles.Contains(tileToAdd))
                    YellowTiles.Remove(tileToAdd);
                break;
            case 2:
                if (!YellowTiles.Contains(tileToAdd))
                    YellowTiles.Add(tileToAdd);
                break;
        }
    }
#endregion
#endregion
}
