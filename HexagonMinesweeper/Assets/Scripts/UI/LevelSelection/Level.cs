using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Canvas))]
public class Level : MonoBehaviour
{
    [SerializeField] private Button btn_startLevel;

    [SerializeField] private TextMeshProUGUI txt_level;

    

    private Image background;

    private Canvas canvas;

    [SerializeField] private Color backgroundColor;

    private void Start()
    {
        
    }

    public void Init(int level, bool isUnlocked, bool isLastUnlockedLevel, Color colorToSet)
    {
        background = GetComponent<Image>();
        canvas = GetComponent<Canvas>();
        txt_level.text = level.ToString();
        btn_startLevel.onClick.AddListener(() => { StartCoroutine(AnimateLevelSelect(level)); });   
        background.color = colorToSet;

        transform.localScale = isLastUnlockedLevel ? Vector3.one * 1.25f : Vector3.one;

        if (isLastUnlockedLevel)
            Debug.Log(transform.position);
    }

    public void SetUnlocked(bool animation)
    {

    }

    private IEnumerator AnimateLevelSelect(int level)
    {
        LeanTween.alphaCanvas(txt_level.GetComponent<CanvasGroup>(), 0f, 0.15f).setEase(LeanTweenType.easeInSine);
        canvas.overrideSorting = true;
        yield return new WaitForSeconds(0.15f);
        RectTransform rect = (RectTransform) transform;
        LeanTween.value(gameObject, rect.sizeDelta.x, 5000, 0.35f).setEase(LeanTweenType.easeInSine).setOnUpdate(
            (float value) => { rect.sizeDelta = new Vector2(value, value); });
        yield return new WaitForSeconds(0.35f);
        GameManager.Instance.StartLevel(level);
        LeanTween.color(rect, backgroundColor, 0.15f).setEase(LeanTweenType.easeInOutSine);
        yield return new WaitForSeconds(0.5f);
        CanvasGroup levelSelectionCanvasGroup = LevelSelection.Instance.GetComponent<CanvasGroup>();
        levelSelectionCanvasGroup.alpha = 0f;
        levelSelectionCanvasGroup.blocksRaycasts = false;
        LevelSelection.Instance.SetBlocksRaycast(false);
        //yield return GridGenerator.Instance.AnimateInGrid();
        //yield return UIController.Instance.AnimateInUI();



    }
}
