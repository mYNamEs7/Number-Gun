using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class UIRewardIndicator : MonoBehaviour
{
    readonly static Dictionary<Language, string> GetName = new Dictionary<Language, string> { { Language.RU, "ПОЛУЧИТЬ X" }, { Language.EN, "GET X" }, { Language.TR, "ALMAK X" } };

    [SerializeField] private Transform _rotator;
    [SerializeField] private Ease _rotatotrEase;
    [SerializeField] private float _rotatotrTime = 1.0f, _rotatorAngleLimit = 41.0f;
    [Space(10)]
    [SerializeField] private TextMeshProUGUI _rvBoostInfoText;
    [SerializeField] private TextMeshProUGUI _rvBoostText;
    private int bonus = 2;
    public int Bonus => bonus;
    bool roll;
    int reward;

    public void Play()
    {
        enabled = true;
        _rotator.DOKill();
        _rotator.rotation = Quaternion.AngleAxis(_rotatorAngleLimit, Vector3.forward);
        _rotator.DORotate(Vector3.forward * -_rotatorAngleLimit, _rotatotrTime).SetEase(_rotatotrEase).SetLoops(-1, LoopType.Yoyo);
    }
    public void Stop()
    {
        enabled = false;
        _rotator.DOKill();
    }

    private void SelectBonus(float angle)
    {
        if (angle > 180) angle -= 360;
        bonus = (int)(4 - Mathf.Abs(angle / _rotatorAngleLimit) * 2.99f);
    }

    public void Play(int rewardAmount)
    {
        Play();
        reward = rewardAmount;
        roll = true;
    }
    public void StopRewardOutput() => roll = false;

    void Update()
    {
        if (!roll) return;

        SelectBonus(_rotator.localEulerAngles.z);
        // _rvBoostInfoText.text = GetName[GameData.Language] + bonus;
        _rvBoostText.text = $"{reward * Bonus}<sprite=0>";
    }
}
