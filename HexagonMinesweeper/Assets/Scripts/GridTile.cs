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

    public List<GridTile> Neighbours;

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

    public bool IsBomb { get; private set; }

    [SerializeField] private Color bombColor;

    [SerializeField] private Color safeColor;

    [SerializeField] private Color warningColor;

    [SerializeField] private Color dangerousColor;

    [SerializeField] private Color alertColor;

    private Color storedColor = Color.clear;

    private EventSystem currentEventSystem;

    private bool isHovering;

    public bool IsDisarmed { get; private set; }

    private List<GridTile> neighbourBombs = new List<GridTile>();

    public bool HasFlipped { get; private set; }

    public bool IsCheckingDisarm { get; private set; }

    public bool IsSafe { get; private set; }

    private List<GridTile> allTiles = new List<GridTile>();

    public Vector3 Position;

    public Dictionary<GridTile, int> colours = new Dictionary<GridTile, int>();

    public bool IsRemovingBomb { get; private set; }

    public bool IsAnimating { get; private set; }

    public List<GridTile> RemovedBombs = new List<GridTile>();

    private bool hasAnimatedDisarm;

    private Color startColour;

    private void Awake()
    {
        background = GetComponent<Image>();
        startColour = background.color;
        button = GetComponent<Button>();
        LeanTween.init(1600);
        ///button.onClick.AddListener(Flip);
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && isHovering)
            Flip();
    }

    public void GetNeighbours(List<GridTile> allInstantiatedTiles)
    {
        storedColor = safeColor;
        Neighbours = new List<GridTile>();
        allTiles = allInstantiatedTiles;

        Vector3 newPos = Vector3.zero;
        //List<Vector3> positionsToCheck = new List<Vector3>();
        for (int i = 0; i < positions.Length; i++)
        {
            Vector3 pos = ((RectTransform)transform).anchoredPosition;
            newPos = pos + positions[i];
            //positionsToCheck.Add(newPos);

            GameObject neighbourObj = GameObject.Find(newPos.ToString());
            if (neighbourObj != null)
                Neighbours.Add(neighbourObj.GetComponent<GridTile>());
        }
        //AddNeighboursAtPositions(positionsToCheck);
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
            if (!tilesSet.Contains(Neighbours[i]) /*&& !Neighbours[i].colours.Keys.Contains(this)*/)
            {
                Neighbours[i].SetColor(0, this);
            }

            if (Neighbours[i].colours.Keys.Count >= 1)
                Debug.Log("Already added a colour", Neighbours[i].gameObject);

            

            GridTile neighbour = Neighbours[i];

            for (int a = 0; a < neighbour.Neighbours.Count; a++)
            {
                if (!tilesSet.Contains(neighbour.Neighbours[a]) /*&& !neighbour.Neighbours[a].colours.Keys.Contains(this)*/)
                {
                    neighbour.Neighbours[a].SetColor(1, this);
                }
                
                GridTile nextNeighbour = neighbour.Neighbours[a];

                for (int b = 0; b < nextNeighbour.Neighbours.Count; b++)
                {
                    if (tilesSet.Contains(nextNeighbour.Neighbours[b]) /*|| nextNeighbour.Neighbours[b].colours.Keys.Contains(this)*/)
                        continue;
                    nextNeighbour.Neighbours[b].SetColor(2, this);
                }
            }
        }
    }

    public void SetColor(int colorToSet, GridTile bomb)
    {
        if (IsBomb || colorIndex < colorToSet)
            return;

        if (colours.Count > 1)
            Debug.Log("Testing");

        switch (colorToSet)
        {
            case 0:
                storedColor = alertColor;
                neighbourBombs.Add(bomb);
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

        //background.color = storedColor;
        button.interactable = false;
        background.raycastTarget = false;
        HasFlipped = true;

        if (IsBomb)
        {
            GridGenerator.Instance.SetInteractability(false);
            StartCoroutine(AnimateBombExplosion());
            GameManager.Instance.SetGameOver();
            return;
        }
        foreach (GridTile bomb in neighbourBombs)
        {
            bomb.CheckForDisarm();
        }

        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }

    public void CheckForDisarm()
    {
        IsCheckingDisarm = true;
        bool bombActive = false;
        foreach (GridTile tile in Neighbours)
        {
            if (tile.IsBomb)
            {
                if (!tile.IsCheckingDisarm)
                {
                    tile.CheckForDisarm();
                    if (!tile.IsSafe)
                        bombActive = true;
                }

                continue;
            }   
            else if (!tile.HasFlipped)
            {
                bombActive = true;
                break;
            }
        }
            

        if (!bombActive)
        {
            IsSafe = true;
            IsBomb = false;
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

    private IEnumerator AnimateDisarm()
    {
        yield return new WaitUntil(() => !GridGenerator.Instance.IsAnimating);
        GridGenerator.Instance.IsAnimating = true;
        if (hasAnimatedDisarm)
            yield break;
        hasAnimatedDisarm = true;
        yield return new WaitForSeconds(1f);

        Dictionary<int, List<List<GridTile>>> neighboursToLoop = new Dictionary<int, List<List<GridTile>>>();
        List<List<GridTile>> tiles = new List<List<GridTile>>();
        tiles.Add(Neighbours);
        neighboursToLoop.Add(0, tiles);

        bool isEnd = GameManager.Instance.BombsToDestroy == 1;

        int amountOfTimes = isEnd ? 14 : 3;

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
                        if (isEnd)
                        {
                            LeanTween.color((RectTransform)neighboursToLoop[i][a][b].transform, safeColor, 0.1f);
                        }

                        neighboursToLoop[i][a][b].IsAnimating = true;
                        newNeighbours.Add(neighboursToLoop[i][a][b].Neighbours);
                    }
                    /*if (!neighboursToLoop[i][a][b].IsAnimating)
                    {
                        
                    }*/
                }

            }
            neighboursToLoop.Add(i + 1, newNeighbours);
            yield return new WaitForSeconds(0.05f);
        }

        /*foreach (GridTile neighbour in Neighbours)
            neighbour.RemoveBomb(0, this);*/

        GridGenerator.Instance.DisarmBomb(this);
        GridGenerator.Instance.IsAnimating = false;
        yield return new WaitForSeconds(1f);
        
        GameManager.Instance.DisarmBomb();
    }

    public void RemoveBomb(int layer, GridTile bomb, bool isEnd)
    {
        if (RemovedBombs.Contains(bomb))
            return;
        RemovedBombs.Add(bomb);

        if (isEnd)
            return;
        //IsRemovingBomb = true;
        if (neighbourBombs.Contains(bomb))
            neighbourBombs.Remove(bomb);

        storedColor = safeColor;

        //colours.Remove(bomb);

        colorIndex = 3;

        /*if (colours.Count != 0)
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
        }
        else
        {
            storedColor = safeColor;
        }*/

        

        if (HasFlipped)
            background.color = storedColor;

        //IsRemovingBomb;

        /*layer++;
        if (layer <= 2)
        {
            foreach (GridTile neighbour in Neighbours)
                neighbour.RemoveBomb(layer, bomb);
        }*/

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

    private IEnumerator AnimateBombExplosion()
    {
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
    }

    /*private IEnumerator ResetAnimating()
    {
        yield return new WaitForSeconds(0.5f);
    }*/
}
