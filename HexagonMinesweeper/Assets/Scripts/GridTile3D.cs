using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Material))]
public class GridTile3D : MonoBehaviour
{
    #region Variables
    #region Editor
    [SerializeField] private Color bombColor;

    [SerializeField] private Color safeColor;

    [SerializeField] private Color warningColor;

    [SerializeField] private Color dangerousColor;

    [SerializeField] private Color alertColor;

    [SerializeField] private Color multiAlertColor;

    [SerializeField] private Color threeBombColor;
    #endregion
    #region Public
    public List<GridTile3D> Neighbours;
    public bool IsBomb { get; private set; }
    public bool IsDisarmed { get; private set; }
    public bool HasFlipped { get; private set; }
    public bool IsCheckingDisarm { get; private set; }
    public bool IsSafe { get; private set; }
    public Vector3 Position;

    public Dictionary<GridTile3D, int> colours = new Dictionary<GridTile3D, int>();

    public bool IsRemovingBomb { get; private set; }

    public bool IsAnimating { get; private set; }

    public List<GridTile3D> RemovedBombs = new List<GridTile3D>();

    #endregion
    #region Private
    private Vector3[] positions =
    {
        new Vector3(1.8f, 0, 0f),
        new Vector3(0.9f, 0, -1.55f),
        new Vector3(-0.9f, 0, -1.55f),
        new Vector3(-1.8f, 0, -0f),
        new Vector3(-0.9f, 0, 1.55f),
        new Vector3(0.9f, 0, 1.55f)
    };

    private Vector3 top = new Vector3(1.8f, 0, 0f);
    private Vector3 topRight = new Vector3(0.9f, 0, -1.55f);
    private Vector3 bottomRight = new Vector3(-0.9f, 0, -1.55f);
    private Vector3 bottom = new Vector3(-1.8f, 0, -0f);
    private Vector3 bottomLeft = new Vector3(-0.9f, 0, 1.55f);
    private Vector3 topLeft = new Vector3(0.9f, 0, 1.55f);

    private Material background;
    //private Button button;

    private int colorIndex = 3;

    public Color storedColor = Color.clear;

    private EventSystem currentEventSystem;

    private bool isHovering;
    private List<GridTile3D> neighbourBombs = new List<GridTile3D>();
    private List<GridTile3D> allTiles = new List<GridTile3D>();
    private bool hasAnimatedDisarm;

    private Color startColour;

    private bool wasFormerBomb;

    private Vector3 standardSize = new Vector3(100, 100, 1000);
    #endregion
    #endregion

    #region Methods
    #region Unity
    private void Awake()
    {
        background = GetComponent<MeshRenderer>().material;
        startColour = background.color;
        //button = GetComponent<Button>();
        LeanTween.init(1600);
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && isHovering && !PopupManager.Instance.IsShowingPopup)
            Flip();
    }

    /*public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }*/

    public void OnMouseDown()
    {

    }

    public void OnMouseEnter()
    {
        if (!PopupManager.Instance.IsShowingPopup)
            isHovering = true;
    }

    public void OnMouseOver()
    {

    }

    public void OnMouseExit()
    {
        isHovering = false;
    }
    #endregion
    #region Public
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

            GameObject neighbourObj = GameObject.Find(newPos.ToString());
            if (neighbourObj != null)
                Neighbours.Add(neighbourObj.GetComponent<GridTile3D>());
        }
    }

    public void SetBomb(bool show = false)
    {
        storedColor = bombColor;
        if (show)
        {
            background.color = storedColor;
            HasFlipped = true;
            /*button.interactable = false;
            button.interactable = false;
            background.raycastTarget = false;*/
        }

        IsBomb = true;

        List<GridTile3D> tilesSet = new List<GridTile3D>();
        tilesSet.Add(this);

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

    public void SetColor(int colorToSet, GridTile3D bomb)
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
    }

    public void CheckForDisarm(bool hasTriggeredFromBomb = false)
    {
        IsCheckingDisarm = true;
        bool bombActive = false;
        foreach (GridTile3D tile in Neighbours)
        {
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


        if (!bombActive && !hasTriggeredFromBomb)
        {
            wasFormerBomb = true;
            IsSafe = true;
            IsBomb = false;
            HasFlipped = true;
            /*button.interactable = false;
            background.raycastTarget = false;*/
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

                background.color = storedColor;
            }
            else
                background.color = safeColor;

            StartCoroutine(AnimateDisarm());
        }

        IsCheckingDisarm = false;

    }

    private IEnumerator CheckDisarmAgain()
    {
        yield return new WaitForSeconds(0.5f);
        CheckForDisarm();
    }

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

    public void ResetTile()
    {
        colorIndex = 3;
        IsBomb = false;
        storedColor = Color.clear;
        IsDisarmed = false;
        neighbourBombs = new List<GridTile3D>();
        HasFlipped = false;
        //Neighbours = new List<GridTile3D>();
        isHovering = false;
        background.color = startColour;
        //button.interactable = true;
        //background.raycastTarget = true;
        IsCheckingDisarm = false;
        IsSafe = false;
        //allTiles = new List<GridTile3D>();
        Position = Vector3.zero;
        colours = new Dictionary<GridTile3D, int>();
        IsRemovingBomb = false;
        IsAnimating = false;
        RemovedBombs = new List<GridTile3D>();
        hasAnimatedDisarm = false;
        transform.localScale = standardSize;
        //GetComponent<CanvasGroup>().alpha = 0f;
        wasFormerBomb = false;
    }

    public void AddNeighboursAtPositions(List<Vector3> positions)
    {
        int neighbours = 0;
        foreach (GridTile3D tile in allTiles)
        {
            if (positions.Contains(tile.Position))
            {
                Neighbours.Add(tile);
                neighbours++;
                if (neighbours >= 6)
                    return;
            }

        }
    }
    #endregion
    #region Private

    private void Flip()
    {
        if (HasFlipped)
            return;

        if (storedColor == warningColor)
            iOSHapticFeedback.Instance.Trigger(iOSHapticFeedback.iOSFeedbackType.ImpactLight);
        else if (storedColor == dangerousColor)
            iOSHapticFeedback.Instance.Trigger(iOSHapticFeedback.iOSFeedbackType.ImpactMedium);
        else if (storedColor == alertColor || storedColor == multiAlertColor || storedColor == threeBombColor)
            iOSHapticFeedback.Instance.Trigger(iOSHapticFeedback.iOSFeedbackType.ImpactHeavy);
        else if (IsBomb)
            iOSHapticFeedback.Instance.Trigger(iOSHapticFeedback.iOSFeedbackType.Failure);

        if (storedColor == Color.clear)
            storedColor = safeColor;

        if (!IsBomb)
            LeanTween.color(gameObject, storedColor, 0.25f).setEase(LeanTweenType.easeInOutSine);

        LeanTween.scaleZ(gameObject, standardSize.z * 1.25f, 0.1f).setEase(LeanTweenType.easeInOutSine)
            .setOnComplete(() =>
            {
                LeanTween.scale(gameObject, standardSize, 0.15f).setEase(LeanTweenType.easeInOutSine);
            });

        /*button.interactable = false;
        background.raycastTarget = false;*/
        HasFlipped = true;

        if (IsBomb)
        {
            GridGenerator3D.Instance.SetInteractability(false);
            StartCoroutine(AnimateBombExplosion());
            AudioManager.Instance.PlayEffect(AudioManager.AudioEffects.LOSE);
            return;
        }
        else
        {
            AudioManager.Instance.PlayEffect(AudioManager.AudioEffects.FLIP);
        }

        if (!GameManager.Instance.IsRandomLevel && GameManager.Instance.CurrentLevel == 1)
        {
            StartCoroutine(HandleTutorialLevelFinished());
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

    private IEnumerator HandleTutorialLevelFinished()
    {
        Confetti.Instance.Play();
        UIController.Instance.HideCurrentlyActiveTip();
        AudioManager.Instance.PlayEffect(AudioManager.AudioEffects.WIN);
        GridGenerator3D.Instance.SetBlocksRaycasts(false);
        yield return UIController.Instance.ShowCompleteLevelText();
        GameManager.Instance.FinishTutorialLevel();
    }

    private IEnumerator WaitBeforeCheckingBomb(GridTile3D bomb)
    {
        yield return null;
        bomb.CheckForDisarm();
    }

    private IEnumerator AnimateDisarm()
    {
        yield return new WaitUntil(() => !GridGenerator3D.Instance.IsAnimating);
        if (hasAnimatedDisarm)
            yield break;
        GridGenerator3D.Instance.IsAnimating = true;
        hasAnimatedDisarm = true;
        yield return new WaitForSeconds(1f);
        iOSHapticFeedback.Instance.Trigger(iOSHapticFeedback.iOSFeedbackType.Success);

        Dictionary<int, List<List<GridTile3D>>> neighboursToLoop = new Dictionary<int, List<List<GridTile3D>>>();
        List<List<GridTile3D>> tiles = new List<List<GridTile3D>>();
        tiles.Add(Neighbours);
        neighboursToLoop.Add(0, tiles);

        bool isEnd = GameManager.Instance.BombsToDestroy == 1;

        int amountOfTimes = isEnd ? 14 : 3;
        List<GridTile3D> animatedTiles = new List<GridTile3D>();

        if (isEnd)
        {
            Confetti.Instance.Play();
            StartCoroutine(UIController.Instance.ShowCompleteLevelText());
            AudioManager.Instance.PlayEffect(AudioManager.AudioEffects.WIN);
            GridGenerator3D.Instance.SetBlocksRaycasts(false);
        }


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
                        {
                            LeanTween.color(neighboursToLoop[i][a][b].gameObject, safeColor, 0.1f);
                        }

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
        /*if (isEnd)
            GridGenerator3D.Instance.AnimateLevelComplete();*/
    }



    public IEnumerator AnimateBombExplosion(bool reset = true)
    {
        GridGenerator3D.Instance.SetBlocksRaycasts(false);
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

        for (int i = 0; i < 14; i++)
        {
            if (neighboursToLoop.Keys.Count <= i)
                break;
            List<List<GridTile3D>> newNeighbours = new List<List<GridTile3D>>();
            for (int a = 0; a < neighboursToLoop[i].Count; a++)
            {
                for (int b = 0; b < neighboursToLoop[i][a].Count; b++)
                {
                    if (neighboursToLoop[i][a][b].Position == new Vector3(-5.4f, 0.0f, 0.0f))
                        Debug.Log("What is going on here?");
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

    public IEnumerator AnimateDisarmTile()
    {
        IsAnimating = true;

        LeanTween.scaleZ(gameObject, standardSize.z * 1.15f, 0.15f).setEase(LeanTweenType.easeOutSine);
        yield return new WaitForSeconds(0.15f);
        LeanTween.scaleZ(gameObject, standardSize.z, 0.25f).setEase(LeanTweenType.easeInSine);
        yield return new WaitForSeconds(0.25f);
        IsAnimating = false;
    }
    #endregion
    #endregion
}