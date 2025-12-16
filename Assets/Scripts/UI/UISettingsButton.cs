using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class UISettingsButton : MonoBehaviour
{
    enum State
    {
        Close, Open
    }
    State state;

    [SerializeField] Transform iconTransform;
    [SerializeField] RectTransform dropdownBarTransform, listTransform;
    [SerializeField] Image musicImage, sfxImage;
    [SerializeField] Sprite musicSpriteOn, musicSpriteOff;
    [SerializeField] Sprite sfxSpriteOn, sfxSpriteOff;
    Tween iconRotateTween, dropdownBarTween, listTween;

    void Start()
    {
        musicImage.sprite = YandexGame.savesData.Music ? musicSpriteOn : musicSpriteOff;
        sfxImage.sprite = YandexGame.savesData.SFX ? sfxSpriteOn : sfxSpriteOff;
    }

    public void Click()
    {
        bool open = state == State.Close;
        if (state == State.Close) state = State.Open;
        else state = State.Close;

        iconRotateTween.Kill();
        iconRotateTween = iconTransform.DORotate(new Vector3(0, 0, open ? -360 : 360), 0.33f, RotateMode.FastBeyond360).SetEase(Ease.OutBack);
        dropdownBarTween.Kill();
        dropdownBarTween = DOTween.To(() => dropdownBarTransform.sizeDelta, x => dropdownBarTransform.sizeDelta = x, new Vector2(dropdownBarTransform.sizeDelta.x, open ? 300 : 0), 0.33f).SetEase(Ease.OutBack);
        listTween.Kill();
        listTween = DOTween.To(() => listTransform.sizeDelta, x => listTransform.sizeDelta = x, new Vector2(listTransform.sizeDelta.x, open ? 300 : 0), 0.33f).SetEase(Ease.OutBack);
    }

    public void ToggleMusic()
    {
        YandexGame.savesData.Music = !YandexGame.savesData.Music;
        YandexGame.SaveProgress();

        SoundHolder.Default.SetSFX(YandexGame.savesData.Music);
        musicImage.sprite = YandexGame.savesData.Music ? musicSpriteOn : musicSpriteOff;
    }

    public void ToggleSFX()
    {
        YandexGame.savesData.SFX = !YandexGame.savesData.SFX;
        YandexGame.SaveProgress();

        SoundHolder.Default.SetSFX(YandexGame.savesData.SFX);
        sfxImage.sprite = YandexGame.savesData.SFX ? sfxSpriteOn : sfxSpriteOff;
    }
}
