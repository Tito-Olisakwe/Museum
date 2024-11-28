using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BioPlayer : MonoBehaviour
{
    public AudioSource audioSource;
    public RectTransform contentTransform;
    public ScrollRect scrollRect;
    public float scrollSpeed = 10f;

    private bool isPlaying = false;
    private float originalPosition;
    private float endPosition;

    void Start()
    {
        originalPosition = contentTransform.anchoredPosition.y;
        endPosition = contentTransform.sizeDelta.y - scrollRect.viewport.rect.height;
    }

    public void PlayBio()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
            isPlaying = true;
            StartCoroutine(AutoScroll());
        }
    }

    public void PauseBio()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
            isPlaying = false;
            StopAllCoroutines();
        }
    }

    private IEnumerator AutoScroll()
    {
        float speedMultiplier = 1f;
        float accelerationRate = 0.1f;

        while (isPlaying && contentTransform.anchoredPosition.y > -endPosition)
        {
            speedMultiplier += accelerationRate * Time.deltaTime;
            speedMultiplier = Mathf.Clamp(speedMultiplier, 1f, 2f);

            float scrollAmount = scrollSpeed * Time.deltaTime;
            contentTransform.anchoredPosition += new Vector2(0, scrollAmount);
            yield return null;
        }

        isPlaying = false;
    }

    public void StopBio()
    {
        audioSource.Stop();
        isPlaying = false;
        StopAllCoroutines();
        contentTransform.anchoredPosition = new Vector2(contentTransform.anchoredPosition.x, originalPosition);
    }
}