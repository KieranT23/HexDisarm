using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Material))]
public class GridTile3D : MonoBehaviour
{
    #region Variables
    #region Editor
    /// <summary>
    /// The bomb colour
    /// </summary>
    [SerializeField] private Color bombColor;
    /// <summary>
    /// The safe colour
    /// </summary>
    [SerializeField] private Color safeColor;
    /// <summary>
    /// The warning colour - 3 tiles away from bomb
    /// </summary>
    [SerializeField] private Color warningColor;
    /// <summary>
    /// The dangerous colour - 2 tiles away from bomb
    /// </summary>
    [SerializeField] private Color dangerousColor;
    /// <summary>
    /// The alert colour - 1 tile away from bomb
    /// </summary>
    [SerializeField] private Color alertColor;
    /// <summary>
    /// The colour for when the tile is next to 2 bombs
    /// </summary>
    [SerializeField] private Color multiAlertColor;
    /// <summary>
    /// The colour for whne a tile is next to 3 or more bombs
    /// </summary>
    [SerializeField] private Color threeBombColor;
    /// <summary>
    /// The bomb canvas
    /// </summary>
    [SerializeField] private GameObject bombCanvas;
    /// <summary>
    /// The bomb image
    /// </summary>
    [SerializeField] private Image bombImage;
    #endregion
    #region Public
    /// <summary>
    /// The bombs neighbours
    /// </summary>
    public List<GridTile3D> Neighbours;
    /// <summary>
    /// Is this tile a bomb?
    /// </summary>
    public bool IsBomb { get; private set; }
    /// <summary>
    /// Has this bomb been disarmed?
    /// </summary>
    public bool IsDisarmed { get; private set; }
    /// <summary>
    /// Has this tile been flipped
    /// </summary>
    public bool HasFlipped { get; private set; }
    /// <summary>
    /// Is this tile checking to see if it can disarm?
    /// </summary>
    public bool IsCheckingDisarm { get; private set; }
    /// <summary>
    /// Is the tile safe?
    /// </summary>
    public bool IsSafe { get; private set; }
    /// <summary>
    /// The position of the tile
    /// </summary>
    public Vector3 Position;
    /// <summary>
    /// All the colours on the current tile
    /// </summary>
    public Dictionary<GridTile3D, int> colours = new Dictionary<GridTile3D, int>();
    /// <summary>
    /// Is this tile currently removing a bomb
    /// </summary>
    public bool IsRemovingBomb { get; private set; }
    /// <summary>
    /// Is this tile animating?
    /// </summary>
    public bool IsAnimating { get; private set; }
    /// <summary>
    /// All the bombs that have been removed from this tile
    /// </summary>
    public List<GridTile3D> RemovedBombs = new List<GridTile3D>();

    #endregion
    #region Private
    /// <summary>
    /// The possible positions of the tiles
    /// </summary>
    private Vector3[] positions =
    {
        new Vector3(1.8f, 0, 0f),
        new Vector3(0.9f, 0, -1.55f),
        new Vector3(-0.9f, 0, -1.55f),
        new Vector3(-1.8f, 0, -0f),
        new Vector3(-0.9f, 0, 1.55f),
        new Vector3(0.9f, 0, 1.55f)
    };
    /// <summary>
    /// The material that is attached to this object
    /// </summary>
    private Material background;
    /// <summary>
    /// The colour index of this tile
    /// </summary>
    private int colorIndex = 3;
    /// <summary>
    /// The stored colour of this object
    /// </summary>
    private Color storedColor = Color.clear;
    /// <summary>
    /// Check if the user is currently hovering over this tile
    /// </summary>
    private bool isHovering;
    /// <summary>
    /// All the neighbour bombs
    /// </summary>
    private List<GridTile3D> neighbourBombs = new List<GridTile3D>();
    /// <summary>
    /// All the tiles in the grid
    /// </summary>
    private List<GridTile3D> allTiles = new List<GridTile3D>();
    /// <summary>
    /// Check if this tile has animated the disarm
    /// </summary>
    private bool hasAnimatedDisarm;
    /// <summary>
    /// The colour the tile starts as
    /// </summary>
    private Color startColour;
    /// <summary>
    /// Check if the tile was a bomb previously
    /// </summary>
    private bool wasFormerBomb;
    /// <summary>
    /// The standard size of the tile
    /// </summary>
    private Vector3 standardSize = new Vector3(100, 100, 1000);
    /// <summary>
    /// Check if the next tutorial step should be triggered
    /// </summary>
    private bool triggerNextTutorialStep;
    #endregion
    #endregion

    #region Methods
    #region Unity
    private void Awake()
    {
        background = GetComponent<MeshRenderer>().material;
        startColour = background.color;
        LeanTween.init(1600);
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && isHovering && !PopupManager.Instance.IsShowingPopup)
            Flip();
    }

    public void OnMouseEnter()
    {
        if (!PopupManager.Instance.IsShowingPopup)
            isHovering = true;
    }

    public void OnMouseExit()
    {
        isHovering = false;
    }
    #endregion
    #region Public
    /// <summary>
    /// Get all the neighbours of this tile
    /// </summary>
    /// <param name="allInstantiatedTiles">All the tiles in the grid</param>
    public void GetNeighbours(List<GridTile3D> allInstantiatedTiles)
    {
        storedColor = safeColor;
        Neighbours = new List<GridTile3D>();
        allTiles = allInstantiatedTiles;

        Vector3 newPos = Vector3.zero;
        for (int i = 0; i < positions.Length; i++)
        {
            Vector3 pos = transform.localPosition;
            newPos = pos + positions[i];

            Vector3 newVector3 = new Vector3(float.Parse(newPos.x.ToString("F2")), newPos.y, float.Parse(newPos.z.ToString("F2")));

            //Using GameObject.Find instead of searching for a vector 3 in a list as this is quicker
            GameObject neighbourObj = GameObject.Find(newVector3.ToString());
            if (neighbourObj != null)
                Neighbours.Add(neighbourObj.GetComponent<GridTile3D>());
            else
            {
                //Check for error - usually occurs in the corners
                neighbourObj = GameObject.Find(new Vector3(newVector3.x, newVector3.y, newVector3.z - 0.1f).ToString());
                if (neighbourObj != null)
                    Neighbours.Add(neighbourObj.GetComponent<GridTile3D>());
                else
                {
                    neighbourObj = GameObject.Find(new Vector3(newVector3.x, newVector3.y, newVector3.z + 0.1f).ToString());
                    if (neighbourObj != null)
                        Neighbours.Add(neighbourObj.GetComponent<GridTile3D>());
                }
            }
        }
    }
    /// <summary>
    /// Set the tile as a bomb
    /// </summary>
    /// <param name="show">Whether the bomb should be shown</param>
    /// <param name="shouldTriggerNextTutorialStep">Whether the tile should trigger the next tutorial step</param>
    public void SetBomb(bool show = false, bool shouldTriggerNextTutorialStep = false)
    {
        if (show)
            bombCanvas.gameObject.SetActive(true);

        triggerNextTutorialStep = shouldTriggerNextTutorialStep;
        storedColor = bombColor;
        if (show)
        {
            background.color = storedColor;
            HasFlipped = true;
        }

        IsBomb = true;

        List<GridTile3D> tilesSet = new List<GridTile3D>();
        tilesSet.Add(this);

        //Search through all the neighbours and then set their colour depending on how far they are away from the bomb
        for (int i = 0; i < Neighbours.Count; i++)
        {
            if (!tilesSet.Contains(Neighbours[i]) && Neighbours[i].gameObject.activeInHierarchy)
            {
                Neighbours[i].SetColor(0, this);
            }
            GridTile3D neighbour = Neighbours[i];

            for (int a = 0; a < neighbour.Neighbours.Count; a++)
            {
                
                if (!tilesSet.Contains(neighbour.Neighbours[a]) && Neighbours[i].gameObject.activeInHierarchy)
                {
                    neighbour.Neighbours[a].SetColor(1, this);
                }

                GridTile3D nextNeighbour = neighbour.Neighbours[a];

                for (int b = 0; b < nextNeighbour.Neighbours.Count; b++)
                {
                    if (tilesSet.Contains(nextNeighbour.Neighbours[b]) && Neighbours[i].gameObject.activeInHierarchy)
                        continue;
                    nextNeighbour.Neighbours[b].SetColor(2, this);
                }
            }
        }
    }

    /// <summary>
    /// Set the colour of the tile
    /// </summary>
    /// <param name="colorToSet">The colour to set</param>
    /// <param name="bomb">The bomb that is setting the colour</param>
    /// <param name="flip">Whether the tile should be flipped</param>
    public void SetColor(int colorToSet, GridTile3D bomb, bool flip = false)
    {
        if (IsBomb || colorIndex < colorToSet)
            return;

        switch (colorToSet)
        {
            case 0:
                storedColor = alertColor;

                if (!neighbourBombs.Contains(bomb))
                    neighbourBombs.Add(bomb);

                if (neighbourBombs.Count >= 3)
                    storedColor = threeBombColor;

                else if (neighbourBombs.Count == 2)
                    storedColor = multiAlertColor;

                break;
            case 1:
                storedColor = dangerousColor;
                break;
            case 2:
                storedColor = warningColor;
                break;
        }

        colorIndex = colorToSet;

        if (colours.ContainsKey(bomb))
            colours[bomb] = colorIndex;

        if (HasFlipped)
            background.color = storedColor;

        if (!HasFlipped && gameObject.activeInHierarchy)
            GridGenerator3D.Instance.AddTileToList(this, colorToSet);
    }

    /// <summary>
    /// Check to see if the bomb can be disarmed
    /// </summary>
    /// <param name="hasTriggeredFromBomb">Check if this has been triggered from another bomb</param>
    public void CheckForDisarm(bool hasTriggeredFromBomb = false)
    {
        IsCheckingDisarm = true;
        bool bombActive = false;

        //Check all the neighbours to see if there are any unused tiles or bombs
        foreach (GridTile3D tile in Neighbours)
        {
            //If there is a bomb then it must also check to see if that bomb can be disarmed
            if (tile.IsBomb)
            {
                if (!tile.IsCheckingDisarm)
                {
                    tile.CheckForDisarm(true);
                    if (!tile.IsSafe)
                        bombActive = true;
                }
                continue;
            }
            else if (!tile.HasFlipped && tile.gameObject.activeInHierarchy)
            {
                bombActive = true;
                IsSafe = false;
                break;
            }
        }

        if (!bombActive && hasTriggeredFromBomb)
        {
            IsSafe = true;
            StartCoroutine(CheckDisarmAgain());
        }

        //Then set the colour depending on how far the tile is away from another bomb
        if (!bombActive && !hasTriggeredFromBomb)
        {
            wasFormerBomb = true;
            IsSafe = true;
            IsBomb = false;
            HasFlipped = true;
            background.color = bombColor;
            if (colours.Count != 0)
            {
                int lowestColour = 3;
                foreach (GridTile3D key in colours.Keys)
                {
                    if (colours[key] < lowestColour)
                        lowestColour = colours[key];
                }

                switch (lowestColour)
                {
                    case 0:
                        storedColor = alertColor;
                        break;
                    case 1:
                        storedColor = dangerousColor;
                        break;
                    case 2:
                        storedColor = warningColor;
                        break;
                }
            }
            else
                storedColor = safeColor;

            StartCoroutine(AnimateDisarm());
        }

        IsCheckingDisarm = false;

    }

    /// <summary>
    /// Remove a bomb
    /// </summary>
    /// <param name="layer">The layer that the bomb was on</param>
    /// <param name="bomb">The bomb to remove</param>
    /// <param name="isEnd">Is this the end of the level?</param>
    public void RemoveBomb(int layer, GridTile3D bomb, bool isEnd)
    {
        if (RemovedBombs.Contains(bomb))
            return;

        RemovedBombs.Add(bomb);

        if (isEnd)
            return;

        if (neighbourBombs.Contains(bomb))
            neighbourBombs.Remove(bomb);

        if ((IsBomb && IsSafe) || !IsBomb)
            storedColor = safeColor;

        colorIndex = 3;

        if (HasFlipped)
            background.color = storedColor;

    }

    /// <summary>
    /// Reset the tile so it can be used again
    /// </summary>
    public void ResetTile()
    {
        colorIndex = 3;
        IsBomb = false;
        storedColor = Color.clear;
        IsDisarmed = false;
        neighbourBombs = new List<GridTile3D>();
        HasFlipped = false;
        isHovering = false;
        background.color = startColour;
        IsCheckingDisarm = false;
        IsSafe = false;
        Position = Vector3.zero;
        colours = new Dictionary<GridTile3D, int>();
        IsRemovingBomb = false;
        IsAnimating = false;
        RemovedBombs = new List<GridTile3D>();
        hasAnimatedDisarm = false;
        transform.localScale = standardSize;
        wasFormerBomb = false;
        triggerNextTutorialStep = false;
        bombCanvas.SetActive(false);
    }

    /// <summary>
    /// Flip a tile as part of the tutorial
    /// </summary>
    public void FlipForTutorial()
    {
        Flip(false);
    }

    /// <summary>
    /// Animate a bomb explosion
    /// </summary>
    /// <param name="reset">Whether it should reset the grid afterwards</param>
    public IEnumerator AnimateBombExplosion(bool reset = true)
    {
        GridGenerator3D.Instance.SetBlocksRaycasts(false);

        //Animate the bomb
        yield return new WaitForSeconds(0.25f);

        LeanTween.scaleZ(gameObject, standardSize.z * 1.25f, 0.85f).setEase(LeanTweenType.easeInQuint);
        LeanTween.color(gameObject, storedColor, 0.15f).setLoopPingPong(1)
            .setEase(LeanTweenType.easeInOutSine);

        yield return new WaitForSeconds(0.3f);

        LeanTween.color(gameObject, storedColor, 0.1f).setLoopPingPong(1)
            .setEase(LeanTweenType.easeInOutSine);

        yield return new WaitForSeconds(0.2f);

        LeanTween.color(gameObject, storedColor, 0.05f)
            .setEase(LeanTweenType.easeInOutSine);

        yield return new WaitForSeconds(0.5f);

        LeanTween.scaleZ(gameObject, 0f, 0.1f).setEase(LeanTweenType.easeInSine);

        yield return new WaitForSeconds(0.1f);

        Dictionary<int, List<List<GridTile3D>>> neighboursToLoop = new Dictionary<int, List<List<GridTile3D>>>();
        List<List<GridTile3D>> tiles = new List<List<GridTile3D>>();
        tiles.Add(Neighbours);
        neighboursToLoop.Add(0, tiles);

        //Animate all the tiles around the bomb
        for (int i = 0; i < 14; i++)
        {
            if (neighboursToLoop.Keys.Count <= i)
                break;

            List<List<GridTile3D>> newNeighbours = new List<List<GridTile3D>>();
            for (int a = 0; a < neighboursToLoop[i].Count; a++)
            {
                for (int b = 0; b < neighboursToLoop[i][a].Count; b++)
                {
                    if (!neighboursToLoop[i][a][b].IsAnimating)
                    {
                        LeanTween.scale(neighboursToLoop[i][a][b].gameObject, new Vector3(0.01f, 0.01f, 0.01f), 0.1f).setEase(LeanTweenType.easeInSine);
                        neighboursToLoop[i][a][b].IsAnimating = true;
                        newNeighbours.Add(neighboursToLoop[i][a][b].Neighbours);
                    }
                }

            }
            neighboursToLoop.Add(i + 1, newNeighbours);

            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(0.5f);

        if (reset)
            GameManager.Instance.SetGameOver();
    }

    /// <summary>
    /// Animate the disarming of a tile
    /// </summary>
    public IEnumerator AnimateDisarmTile()
    {
        IsAnimating = true;

        LeanTween.scaleZ(gameObject, standardSize.z * 1.15f, 0.15f).setEase(LeanTweenType.easeOutSine);

        yield return new WaitForSeconds(0.15f);

        LeanTween.scaleZ(gameObject, standardSize.z, 0.25f).setEase(LeanTweenType.easeInSine);

        yield return new WaitForSeconds(0.25f);

        IsAnimating = false;
    }

    /// <summary>
    /// Change the current theme
    /// </summary>
    /// <param name="coloursToSet">The colours to change the theme to</param>
    public void SetColours(Color[] coloursToSet)
    {
        safeColor = coloursToSet[0];
        warningColor = coloursToSet[1];
        dangerousColor = coloursToSet[2];
        alertColor = coloursToSet[3];
        multiAlertColor = coloursToSet[4];
        threeBombColor = coloursToSet[5];
        bombColor = coloursToSet[6];
        startColour = coloursToSet[7];
        bombImage.color = coloursToSet[8];

        switch (colorIndex)
        {
            case 0:
                storedColor = alertColor;
                if (neighbourBombs.Count >= 3)
                    storedColor = threeBombColor;
                else if (neighbourBombs.Count == 2)
                    storedColor = multiAlertColor;
                break;
            case 1:
                storedColor = dangerousColor;
                break;
            case 2:
                storedColor = warningColor;
                break;
            case 3:
                storedColor = safeColor;
                break;
        }

        if (IsBomb)
        {
            storedColor = bombColor;
        }

        if (HasFlipped)
            background.color = storedColor;
        else
        {
            background.color = startColour;
        }
    }
    #endregion
    #region Private
    /// <summary>
    /// Flip the tile
    /// </summary>
    /// <param name="hapticFeedback">Play haptic feedback</param>
    private void Flip(bool hapticFeedback = true)
    {
        if (HasFlipped)
            return;

        if (hapticFeedback)
        {
            if (!IsBomb)
            {
                if (storedColor == dangerousColor)
                    iOSHapticFeedback.Instance.Trigger(iOSHapticFeedback.iOSFeedbackType.ImpactMedium);
                else if (storedColor == alertColor || storedColor == multiAlertColor || storedColor == threeBombColor)
                    iOSHapticFeedback.Instance.Trigger(iOSHapticFeedback.iOSFeedbackType.ImpactHeavy);
                else
                    iOSHapticFeedback.Instance.Trigger(iOSHapticFeedback.iOSFeedbackType.ImpactLight);
            }
            else
                iOSHapticFeedback.Instance.Trigger(iOSHapticFeedback.iOSFeedbackType.Failure);
        }
        
            

        if (storedColor == Color.clear)
            storedColor = safeColor;

        if (!IsBomb)
            LeanTween.color(gameObject, storedColor, 0.25f).setEase(LeanTweenType.easeInOutSine);

        LeanTween.scaleZ(gameObject, standardSize.z * 1.25f, 0.1f).setEase(LeanTweenType.easeInOutSine)
            .setOnComplete(() =>
            {
                LeanTween.scale(gameObject, standardSize, 0.15f).setEase(LeanTweenType.easeInOutSine);
            });

        HasFlipped = true;

        if (IsBomb)
        {
            StartCoroutine(AnimateBombExplosion());
            bombCanvas.SetActive(true);
            return;
        }

        int bombsChecked = 0;
        foreach (GridTile3D bomb in neighbourBombs)
        {
            if (bombsChecked > 0)
                StartCoroutine(WaitBeforeCheckingBomb(bomb));
            else
                bomb.CheckForDisarm();

            bombsChecked++;
        }
    }
    /// <summary>
    /// Wait before checking bomb to make sure that bombs are not checking disarms at the same time
    /// </summary>
    /// <param name="bomb">The bomb to check</param>
    private IEnumerator WaitBeforeCheckingBomb(GridTile3D bomb)
    {
        yield return null;
        bomb.CheckForDisarm();
    }
    /// <summary>
    /// Animate the disarm of a tile
    /// </summary>
    /// <returns></returns>
    private IEnumerator AnimateDisarm()
    {
        //Make sure the grid is not being animated
        yield return new WaitUntil(() => !GridGenerator3D.Instance.IsAnimating);
        
        if (triggerNextTutorialStep)
            UIController.Instance.ShowTutorialTip(3);
        if (hasAnimatedDisarm)
            yield break;

        //Animate the bomb
        GridGenerator3D.Instance.IsAnimating = true;
        hasAnimatedDisarm = true;
        bombCanvas.gameObject.SetActive(true);
        CanvasGroup bombCanvasGroup = bombCanvas.GetComponent<CanvasGroup>();
        bombCanvasGroup.alpha = 0f;
        LeanTween.alphaCanvas(bombCanvasGroup, 1f, 0.25f).setEase(LeanTweenType.easeOutSine);

        yield return new WaitForSeconds(0.5f);

        LeanTween.alphaCanvas(bombCanvas.GetComponent<CanvasGroup>(), 0f, 0.5f).setOnComplete(() =>
        {
            bombCanvas.gameObject.SetActive(false);
            bombCanvas.GetComponent<CanvasGroup>().alpha = 1f;
        });

        LeanTween.value(gameObject, background.color, storedColor, 0.5f).setEase(LeanTweenType.easeInOutSine)
            .setOnUpdate(
                (Color value) => { background.color = value; });


        yield return new WaitForSeconds(0.4f);

        StartCoroutine(AnimateDisarmTile());

        yield return new WaitForSeconds(0.1f);
        
        Dictionary<int, List<List<GridTile3D>>> neighboursToLoop = new Dictionary<int, List<List<GridTile3D>>>();
        List<List<GridTile3D>> tiles = new List<List<GridTile3D>>();
        tiles.Add(Neighbours);
        neighboursToLoop.Add(0, tiles);

        bool isEnd = GameManager.Instance.BombsToDestroy == 1;
        int amountOfTimes = isEnd ? 14 : 3;
        List<GridTile3D> animatedTiles = new List<GridTile3D>();
        if (isEnd)
            iOSHapticFeedback.Instance.Trigger(iOSHapticFeedback.iOSFeedbackType.Success);
        else
            iOSHapticFeedback.Instance.Trigger(iOSHapticFeedback.iOSFeedbackType.ImpactHeavy);

        if (isEnd)
        {
            Confetti.Instance.Play();
            StartCoroutine(UIController.Instance.ShowCompleteLevelText());
            GridGenerator3D.Instance.SetBlocksRaycasts(false);
        }

        //Animate all the tiles around the bomb that is being disarmed
        for (int i = 0; i < amountOfTimes; i++)
        {
            if (!isEnd)
            {
                if (neighboursToLoop.Keys.Count <= i)
                    break;
            }

            List<List<GridTile3D>> newNeighbours = new List<List<GridTile3D>>();
            for (int a = 0; a < neighboursToLoop[i].Count; a++)
            {
                for (int b = 0; b < neighboursToLoop[i][a].Count; b++)
                {
                    if (!neighboursToLoop[i][a][b].RemovedBombs.Contains(this))
                    {
                        neighboursToLoop[i][a][b].RemoveBomb(i, this, isEnd);
                        StartCoroutine(neighboursToLoop[i][a][b].AnimateDisarmTile());

                        if (isEnd)
                            LeanTween.color(neighboursToLoop[i][a][b].gameObject, safeColor, 0.1f);

                        neighboursToLoop[i][a][b].IsAnimating = true;
                        newNeighbours.Add(neighboursToLoop[i][a][b].Neighbours);
                        animatedTiles.Add(neighboursToLoop[i][a][b]);
                    }
                }

            }
            neighboursToLoop.Add(i + 1, newNeighbours);

            yield return new WaitForSeconds(0.1f);
        }

        GridGenerator3D.Instance.DisarmBomb(this);
        GridGenerator3D.Instance.IsAnimating = false;

        yield return new WaitForSeconds(1f);

        foreach (GridTile3D tile in animatedTiles)
            tile.IsAnimating = false;

        GameManager.Instance.DisarmBomb();
    }

    /// <summary>
    /// Check for a disarm at a latter stage to make sure that bombs don't check if they can be disarmed at the same time
    /// </summary>
    private IEnumerator CheckDisarmAgain()
    {
        yield return new WaitForSeconds(0.5f);
        CheckForDisarm();
    }
    #endregion
    #endregion
}