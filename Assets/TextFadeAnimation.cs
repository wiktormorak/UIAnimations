using System.Collections;
using TMPro;
using UnityEngine;

public class TextFadeAnimation : MonoBehaviour
{
    private bool isEnabled;
    private bool textCreated;
    
    [Header("GameObjects")]
    #region GameObjects
    public GameObject canvas;
    public GameObject textParent;
    public GameObject textObject;
    private Color textColor;
    private float textAlpha;
    private RectTransform rectTransform;
    public string textObjectName; // if not set it defaults to "FadeAnimText"
    #endregion
    [Header("Animation Configuration")]
    #region Animation Configuration
    public bool doesTextMove;
    public bool doesColourFade;
    public bool doesAlphaFade; // if this is false, the script uses startAlpha as the text alpha.
    public bool isFadeIn; // if this is false, it's a fade out effect.
    public FadeDirection fadeDirection; // this isn't necessary to change in editor, as there is a method that functions as an automatic check.
    #endregion
    [Header("Fade Configuration")]
    #region Fade Configuration
    public string textString;
    [Range(-200, 200)] public float animationDuration; // recommended value is 40.
    [Range(-200, 200)] public float fadeDuration;
    [Range(0, 255)]public float startAlpha; // you should use 255 rgb values as it turns it into 1 rgb automatically.
    [Range(0, 255)]public float targetAlpha;
    public Vector3 startPosition;
    public Vector3 targetPosition; // if this isn't set, the text will keep moving until it hits the targetAlpha
    public Color textStartColor; // if textColourFade is false, this is the colour that is used for the text.
    public Color textEndColor;
    #endregion
    #region Unity Methods
    public void Enable() {
        isEnabled = true;
        GetAnimationDirection();
    }
    void Update() {
        if (isEnabled){
            StartCoroutine(FadeAnimationCoroutine());
        }
    }
    #endregion
    void SetupTextObject() {
        textObject = new GameObject("FadeAnimText");
        textObject.transform.parent = canvas.transform;
        TextMeshProUGUI textMesh = textObject.AddComponent<TextMeshProUGUI>();
        TMP_Text tmpText = textMesh.GetComponent<TMP_Text>();
        textMesh.color = textStartColor;
        startAlpha = startAlpha / 255f;
        targetAlpha = targetAlpha / 255f;
        #region Is FadeIn or FadeOut
        if (!isFadeIn){
            textMesh.alpha = startAlpha;
        }
        else{
            textMesh.alpha = targetAlpha;
        }
        #endregion
        tmpText.text = textString;
        textObject.transform.localPosition = startPosition;
        textCreated = true;
    }
    void GetAnimationDirection() {
        #region Up or Down
        if (targetPosition.y != 0f){
            if (startPosition.y <= targetPosition.y){
                fadeDirection = FadeDirection.up;
            }
            else if (startPosition.y >= targetPosition.y){
                fadeDirection = FadeDirection.down;
            }
        }
        else if (targetPosition.x != 0f){
            if (startPosition.x <= targetPosition.x){
                fadeDirection = FadeDirection.right;
            }
            else if (startPosition.x >= targetPosition.x){
                fadeDirection = FadeDirection.left;
            }
        }
        #endregion
    }
    IEnumerator FadeAnimationCoroutine() {
        #region Setup
        if (!textCreated){
            SetupTextObject();
            rectTransform = textObject.GetComponent<RectTransform>();
            textColor = textObject.GetComponent<TextMeshProUGUI>().color;
            textAlpha = textObject.GetComponent<TextMeshProUGUI>().color.a;
        }
        #endregion
        FadeAlphaFunction();
        TextMove();
        #region Delete Object
        if(doesAlphaFade && textAlpha == targetAlpha) {
            Destroy(textObject.transform);
        }
        else if (doesTextMove && rectTransform.localPosition == targetPosition){
            Destroy(textObject.transform);
        }
        #endregion
        yield return null;
    }
    void FadeAlphaFunction() {
        if (doesAlphaFade){
            if (!isFadeIn){
                if (textObject.GetComponent<TextMeshProUGUI>().color.a >= targetAlpha){
                    Color newColor = textObject.GetComponent<TextMeshProUGUI>().color;
                    newColor.a -= Time.deltaTime * fadeDuration;
                    textObject.GetComponent<TextMeshProUGUI>().color = newColor;
                }
            }
            else{
                if (textObject.GetComponent<TextMeshProUGUI>().color.a <= startAlpha){
                    Color newColor = textObject.GetComponent<TextMeshProUGUI>().color;
                    newColor.a += Time.deltaTime * fadeDuration;
                    textObject.GetComponent<TextMeshProUGUI>().color = newColor;
                }
            }
        }
    }
    void FadeColorFunction() {
        
    }
    void TextMove() {
        if (doesTextMove){
            #region Up or Down
            if (fadeDirection == FadeDirection.up){
                rectTransform.localPosition += new Vector3(0f, Time.deltaTime * animationDuration, 0f);
            }
            else if (fadeDirection == FadeDirection.down){
                rectTransform.localPosition -= new Vector3(0f, Time.deltaTime * animationDuration, 0f);
            }
            #endregion
            #region Right or Left
            if (fadeDirection == FadeDirection.left){
                rectTransform.localPosition -= new Vector3(Time.deltaTime * animationDuration, 0f, 0f);
            }
            else if (fadeDirection == FadeDirection.right){
                rectTransform.localPosition += new Vector3(Time.deltaTime * animationDuration, 0f, 0f);
            }
            #endregion
        }
    }
}

public enum FadeDirection
{
    notSet,
    left,
    right,
    up,
    down
}