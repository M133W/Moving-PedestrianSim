using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public UnityEngine.UI.Image fadeImage; // 引用 Canvas 上的 Image 组件
    public float pureBlackDuration = 3.0f; // 纯黑屏持续时间
    public float fadeDuration = 0.5f; // 从黑屏变透明的持续时间
    public bool isFading = false; // 是否正在进行黑屏动画
    public bool isPaused = false; // 游戏是否暂停
    private MarkerEvent markerEvent;
    private Graphic[] childGraphics;

    void Start()
    {
        ShowScreenFader();
    }
    void Awake()
    {
        markerEvent = FindObjectOfType<MarkerEvent>();
        childGraphics = fadeImage.GetComponentsInChildren<Graphic>();

    }
    public void ShowScreenFader()
    {
        StartCoroutine(BlackScreen());
        //markerEvent.RecordRestartTry();
        //markerEvent.RecordStartSimPedVrM();
        //markerEvent.RecordStartSimPedVA();
    }

    void Update()
    {
        if (isFading && Input.GetKeyDown(KeyCode.Space))
        {
            if (!isPaused)
            {
                // 暂停游戏
                markerEvent.RecordIsPaused();
                Time.timeScale = 0f;
                isPaused = true;
            }
            else
            {
                // 继续游戏
                isPaused = false;
                StartCoroutine(FadeScreen());
            }
        }
    }

    IEnumerator BlackScreen()
    {
        isFading = true;
        fadeImage.gameObject.SetActive(true);

        // 第一阶段：纯黑屏
        float timer = 0f;
        while (timer < pureBlackDuration)
        {
            float alpha = 1f;
            // 调整 fadeImage 的透明度
            fadeImage.color = new Color(0f, 0f, 0f, alpha);

            // 调整所有子物体的透明度
            foreach (Graphic graphic in childGraphics)
            {
                graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, alpha);
            }

            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        if(!isPaused){
            StartCoroutine(FadeScreen());
        }
    }
    IEnumerator FadeScreen()
    {
        Time.timeScale = 1f;
        isFading = false;
        // 第二阶段：从黑屏变透明
        float fadeTimer = 0f;
        while (fadeTimer < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, fadeTimer / fadeDuration);
            // 调整 fadeImage 的透明度
            fadeImage.color = new Color(0f, 0f, 0f, alpha);

            // 调整所有子物体的透明度
            foreach (Graphic graphic in childGraphics)
            {
                graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, alpha);
            }

            fadeTimer += Time.unscaledDeltaTime;
            yield return null;
        }
        fadeImage.gameObject.SetActive(false);
        markerEvent.RecordRestartTry();
        markerEvent.RecordStartSimPedVrM();
        //markerEvent.RecordStartSimPedVA();
    }
}
