using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class AudioPlayer : MonoBehaviour
{
    public AudioSource AudioSource;
    public Button NextButton;
    public Button PreviousButton;
    public Slider VolumeSlider;
    public TMP_Text TrackNameText1;
    public TMP_Text TrackNameText2;
    public RectTransform TrackNamePanel; 
    public AudioClip[] AudioClips;
    public float FadeDuration = 1.0f;
    public float ScrollSpeed = 50f;
    public float TextSpacing = 50f;

    private int currentClipIndex = 0;
    private const string VolumePrefKey = "PlayerVolume";
    private Coroutine scrollCoroutine;

    void Start()
    {
        NextButton.onClick.AddListener(NextAudio);
        PreviousButton.onClick.AddListener(PreviousAudio);

        float savedVolume = PlayerPrefs.GetFloat(VolumePrefKey, 0.5f);
        AudioSource.volume = savedVolume;
        VolumeSlider.value = savedVolume;
        VolumeSlider.onValueChanged.AddListener(SetVolume);

        if (AudioClips.Length > 0)
        {
            AudioSource.clip = AudioClips[currentClipIndex];
            UpdateTrackName();
            StartCoroutine(FadeInAudio());
        }
    }

    void Update()
    {
        if (!AudioSource.isPlaying && AudioSource.time >= AudioSource.clip.length)
        {
            NextAudio();
        }
    }

    void NextAudio()
    {
        if (AudioClips.Length > 0)
        {
            currentClipIndex = (currentClipIndex + 1) % AudioClips.Length;
            AudioSource.clip = AudioClips[currentClipIndex];
            UpdateTrackName();
            StartCoroutine(FadeInAudio());
        }
    }

    void PreviousAudio()
    {
        if (AudioClips.Length > 0)
        {
            currentClipIndex = (currentClipIndex - 1 + AudioClips.Length) % AudioClips.Length;
            AudioSource.clip = AudioClips[currentClipIndex];
            UpdateTrackName();
            StartCoroutine(FadeInAudio());
        }
    }

    void SetVolume(float volume)
    {
        AudioSource.volume = volume;
        PlayerPrefs.SetFloat(VolumePrefKey, volume);
        PlayerPrefs.Save();
    }

    IEnumerator FadeInAudio()
    {
        AudioSource.volume = 0;
        AudioSource.Play();
        float targetVolume = PlayerPrefs.GetFloat(VolumePrefKey, 0.5f);
        float currentTime = 0;

        while (currentTime < FadeDuration)
        {
            currentTime += Time.deltaTime;
            AudioSource.volume = Mathf.Lerp(0, targetVolume, currentTime / FadeDuration);
            yield return null;
        }
        
        AudioSource.volume = targetVolume;
    }

    void UpdateTrackName()
    {
        if (TrackNameText1 != null && AudioSource.clip != null)
        {
            TrackNameText1.text = AudioSource.clip.name;
            TrackNameText2.text = AudioSource.clip.name;

            if (scrollCoroutine != null)
            {
                StopCoroutine(scrollCoroutine);
            }

            scrollCoroutine = StartCoroutine(ScrollTrackName());
        }
    }

    IEnumerator ScrollTrackName()
    {
        float textWidth = TrackNameText1.preferredWidth;
        float panelWidth = TrackNamePanel.rect.width;

        TrackNameText1.rectTransform.anchoredPosition = new Vector2(0, TrackNameText1.rectTransform.anchoredPosition.y);
        TrackNameText2.rectTransform.anchoredPosition = new Vector2(textWidth + TextSpacing, TrackNameText2.rectTransform.anchoredPosition.y);

        float startX1 = TrackNameText1.rectTransform.anchoredPosition.x;
        float startX2 = TrackNameText2.rectTransform.anchoredPosition.x;

        while (true)
        {
            startX1 -= ScrollSpeed * Time.deltaTime;
            startX2 -= ScrollSpeed * Time.deltaTime;

            if (startX1 <= -textWidth)
            {
                startX1 = startX2 + textWidth + TextSpacing;
            }

            if (startX2 <= -textWidth)
            {
                startX2 = startX1 + textWidth + TextSpacing;
            }

            TrackNameText1.rectTransform.anchoredPosition = new Vector2(startX1, TrackNameText1.rectTransform.anchoredPosition.y);
            TrackNameText2.rectTransform.anchoredPosition = new Vector2(startX2, TrackNameText2.rectTransform.anchoredPosition.y);

            yield return null;
        }
    }
}
