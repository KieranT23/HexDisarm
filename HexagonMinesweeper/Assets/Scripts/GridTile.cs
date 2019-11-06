using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public class GridTile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
    public List<GridTile> Neighbours;
    public bool IsBomb { get; private set; }
    public bool IsDisarmed { get; private set; }
    public bool HasFlipped { get; private set; }
    public bool IsCheckingDisarm { get; private set; }
    public bool IsSafe { get; private set; }
    public Vector3 Position;

    public Dictionary<GridTile, int> colours = new Dictionary<GridTile, int>();

    public bool IsRemovingBomb { get; private set; }

    public bool IsAnimating { get; private set; }

    public List<GridTile> RemovedBombs = new List<GridTile>();
    #endregion
    #region Private
    private Vector3[] positions =
    {
        new Vector2(0, 220),
        new Vector2(200, 110),
        new Vector2(200, -110),
        new Vector2(0, -220),
        new Vector2(-200, -110),
        new Vector2(-200, 110),
    };

    Vector2 top = new Vector2(0, 220);
    Vector2 topRight = new Vector2(200, 110);
    Vector2 bottomRight = new Vector2(200, -110);
    Vector2 bottom = new Vector2(0, -220);
    Vector2 bottomLeft = new Vector2(-200, -110);
    Vector2 topLeft = new Vector2(-200, 110);

    private Image background;
    private Button button;

    private int colorIndex = 3;

    public Color storedColor = Color.clear;

    private EventSystem currentEventSystem;

    private bool isHovering;
    private List<GridTile> neighbourBombs = new List<GridTile>();
    private List<GridTile> allTiles = new List<GridTile>();
    private bool hasAnimatedDisarm;

    private Color startColour;

    private bool wasFormerBomb;
    #endregion
    #endregion

    #region Methods
    #region Unity
    private void Awake()
    {
        background = GetComponent<Image>();
        startColour = background.color;
        button = GetComponent<Button>();
        LeanTween.init(1600);
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && isHovering)
            Flip();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }
    #endregion
    #region Public
    public void GetNeighbours(List<GridTile> allInstantiatedTiles)
    {
        storedColor = safeColor;
        Neighbours = new List<GridTile>();
        allTiles = allInstantiatedTiles;

        Vector3 newPos = Vector3.zero;
        for (int i = 0; i < positions.Length; i++)
        {
            Vector3 pos = ((RectTransform)transform).anchoredPosition;
            newPos = pos + positions[i];

            GameObject neighbourObj = GameObject.Find(newPos.ToString());
            if (neighbourObj != null)
                Neighbours.Add(neighbourObj.GetComponent<GridTile>());
        }
    }

    public void SetBomb(bool show = false)
    {
        storedColor = bombColor;
        if (show)
        {
            background.color = storedColor;
            HasFlipped = true;
            button.interactable = false;
            button.interactable = false;
            background.raycastTarget = false;
        }
            
        IsBomb = true;

        List<GridTile> tilesSet = new List<GridTile>();
        tilesSet.Add(this);

        for (int i = 0; i < Neighbours.Count; i++)
        {
            if (!tilesSet.Contains(Neighbours[i]))
            {
                Neighbours[i].SetColor(0, this);
            }
            GridTile neighbour = Neighbours[i];

            for (int a = 0; a < neighbour.Neighbours.Count; a++)
            {
                if (!tilesSet.Contains(neighbour.Neighbours[a]))
                {
                    neighbour.Neighbours[a].SetColor(1, this);
                }
                
                GridTile nextNeighbour = neighbour.Neighbours[a];

                for (int b = 0; b < nextNeighbour.Neighbours.Count; b++)
                {
                    if (tilesSet.Contains(nextNeighbour.Neighbours[b]))
                        continue;
                    nextNeighbour.Neighbours[b].SetColor(2, this);
                }
            }
        }
    }

    public void SetColor(int colorToSet, GridTile bomb)
    {
        if (wasFormerBomb)
            Debug.Log("Testing");
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
        foreach (GridTile tile in Neighbours)
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
            else if (!tile.HasFlipped)
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
            button.interactable = false;
            background.raycastTarget = false;
            if (colours.Count != 0)
            {
                int lowestColour = 3;
                foreach (GridTile key in colours.Keys)
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

    public void RemoveBomb(int layer, GridTile bomb, bool isEnd)
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
        neighbourBombs = new List<GridTile>();
        HasFlipped = false;
        Neighbours = new List<GridTile>();
        isHovering = false;
        background.color = startColour;
        button.interactable = true;
        background.raycastTarget = true;
        IsCheckingDisarm = false;
        IsSafe = false;
        allTiles = new List<GridTile>();
        Position = Vector3.zero;
        colours = new Dictionary<GridTile, int>();
        IsRemovingBomb = false;
        IsAnimating = false;
        RemovedBombs = new List<GridTile>();
        hasAnimatedDisarm = false;
        transform.localScale = Vector3.one;
        GetComponent<CanvasGroup>().alpha = 0f;
        wasFormerBomb = false;
    }

    public void AddNeighboursAtPositions(List<Vector3> positions)
    {
        int neighbours = 0;
        foreach (GridTile tile in allTiles)
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
        
        if (storedColor == Color.clear)
            storedColor = safeColor;

        if (!IsBomb)
            LeanTween.color((RectTransform)transform, storedColor, 0.25f).setEase(LeanTweenType.easeInOutSine);

        LeanTween.scale(gameObject, Vector3.one * 1.2f, 0.1f).setEase(LeanTweenType.easeInOutSine)
            .setOnComplete(() =>
                {
                    LeanTween.scale(gameObject, Vector3.one, 0.15f).setEase(LeanTweenType.easeInOutSine);
                });

        button.interactable = false;
        background.raycastTarget = false;
        HasFlipped = true;

        if (IsBomb)
        {
            GridGenerator.Instance.SetInteractability(false);
            StartCoroutine(AnimateBombExplosion());
            AudioManager.Instance.PlayEffect(AudioManager.AudioEffects.LOSE);
            return;
        }
        else
        {
            AudioManager.Instance.PlayEffect(AudioManager.AudioEffects.FLIP);
        }

        if (Neighbours.Count == 0)
        {
            GameManager.Instance.FinishTutorialLevel();
            return;
        }
            

        int bombsChecked = 0;
        foreach (GridTile bomb in neighbourBombs)
        {
            if (bombsChecked > 0)
                StartCoroutine(WaitBeforeCheckingBomb(bomb));
            else
                bomb.CheckForDisarm();

            bombsChecked++;
        }
    }

    private IEnumerator WaitBeforeCheckingBomb(GridTile bomb)
    {
        yield return null;
        bomb.CheckForDisarm();
    }

    private IEnumerator AnimateDisarm()
    {
        yield return new WaitUntil(() => !GridGenerator.Instance.IsAnimating);
        if (hasAnimatedDisarm)
            yield break;
        GridGenerator.Instance.IsAnimating = true;
        hasAnimatedDisarm = true;
        yield return new WaitForSeconds(1f);

        Dictionary<int, List<List<GridTile>>> neighboursToLoop = new Dictionary<int, List<List<GridTile>>>();
        List<List<GridTile>> tiles = new List<List<GridTile>>();
        tiles.Add(Neighbours);
        neighboursToLoop.Add(0, tiles);

        bool isEnd = GameManager.Instance.BombsToDestroy == 1;

        int amountOfTimes = isEnd ? 14 : 3;
        List<GridTile> animatedTiles = new List<GridTile>();

        if (isEnd)
        {
            Confetti.Instance.Play();
            StartCoroutine(UIController.Instance.ShowCompleteLevelText());
            AudioManager.Instance.PlayEffect(AudioManager.AudioEffects.WIN);
            GridGenerator.Instance.SetBlocksRaycasts(false);
        }
            

        for (int i = 0; i < amountOfTimes; i++)
        {
            if (!isEnd)
            {
                if (neighboursToLoop.Keys.Count <= i)
                    break;
            }
            
            List<List<GridTile>> newNeighbours = new List<List<GridTile>>();
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
                            LeanTween.color((RectTransform)neighboursToLoop[i][a][b].transform, safeColor, 0.1f);
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

        GridGenerator.Instance.DisarmBomb(this);
        GridGenerator.Instance.IsAnimating = false;
        yield return new WaitForSeconds(1f);
        foreach (GridTile tile in animatedTiles)
            tile.IsAnimating = false;
        
        GameManager.Instance.DisarmBomb();
    }

    

    public IEnumerator AnimateBombExplosion(bool reset = true)
    {
        GridGenerator.Instance.SetBlocksRaycasts(false);
        yield return new WaitForSeconds(0.25f);
        LeanTween.scale(gameObject, Vector3.one * 1.25f, 0.85f).setEase(LeanTweenType.easeInQuint);
        LeanTween.color((RectTransform) transform, storedColor, 0.15f).setLoopPingPong(1)
            .setEase(LeanTweenType.easeInOutSine);
        yield return new WaitForSeconds(0.3f);
        LeanTween.color((RectTransform)transform, storedColor, 0.1f).setLoopPingPong(1)
            .setEase(LeanTweenType.easeInOutSine);
        yield return new WaitForSeconds(0.2f);
        LeanTween.color((RectTransform)transform, storedColor, 0.05f)
            .setEase(LeanTweenType.easeInOutSine);
        yield return new WaitForSeconds(0.5f);
        LeanTween.scale(gameObject, Vector3.zero, 0.1f).setEase(LeanTweenType.easeInSine);
        yield return new WaitForSeconds(0.1f);

        Dictionary<int, List<List<GridTile>>> neighboursToLoop = new Dictionary<int, List<List<GridTile>>>();
        List<List<GridTile>> tiles = new List<List<GridTile>>();
        tiles.Add(Neighbours);
        neighboursToLoop.Add(0, tiles);

        for (int i = 0; i < 14; i++)
        {
            if (neighboursToLoop.Keys.Count <= i)
                break;
            List<List<GridTile>> newNeighbours = new List<List<GridTile>>();
            for (int a = 0; a < neighboursToLoop[i].Count; a++)
            {
                for (int b = 0; b < neighboursToLoop[i][a].Count; b++)
                {
                    if (!neighboursToLoop[i][a][b].IsAnimating)
                    {
                        LeanTween.scale(neighboursToLoop[i][a][b].gameObject, Vector3.zero, 0.1f).setEase(LeanTweenType.easeInSine);
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

        LeanTween.scale(gameObject, Vector3.one * 1.15f, 0.15f).setEase(LeanTweenType.easeOutSine);
        yield return new WaitForSeconds(0.15f);
        LeanTween.scale(gameObject, Vector3.one, 0.25f).setEase(LeanTweenType.easeInSine);
        yield return new WaitForSeconds(0.25f);
        IsAnimating = false;
    }
    #endregion
    #endregion
}
