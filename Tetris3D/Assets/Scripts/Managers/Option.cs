using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Option : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Toggle ghostModeToggle;
    [SerializeField] private Button confirm;
    [SerializeField] private Button cancel;

    private float bgmVolume = 0.2f;
    private float sfxVolume = 1f;
    private bool isGhostMode = true;

    public UnityEvent<float> OnChangedBGM { private set; get; } = new UnityEvent<float>();
    public UnityEvent<float> OnChangedSFX { private set; get; } = new UnityEvent<float>();
    public UnityEvent<bool> OnChangedGhostMode { private set; get; } = new UnityEvent<bool>();
    public UnityEvent OnConfirm { private set; get; } = new UnityEvent();
    public UnityEvent OnCancel { private set; get; } = new UnityEvent();

    private void Start()
    {
        OnConfirm.AddListener(Confirm);
        OnCancel.AddListener(Cancel);

        OnChangedBGM.AddListener(bgmSlider.onValueChanged.Invoke);
        OnChangedSFX.AddListener(sfxSlider.onValueChanged.Invoke);
        OnChangedGhostMode.AddListener(ghostModeToggle.onValueChanged.Invoke);

        confirm.onClick.AddListener(OnConfirm.Invoke);
        cancel.onClick.AddListener(OnCancel.Invoke);
    }

    public void Init()
    {
        OnChangedBGM.Invoke(bgmVolume);
        OnChangedSFX.Invoke(sfxVolume);
        OnChangedGhostMode.Invoke(isGhostMode);
    }

    private void Confirm()
    {
        if (bgmVolume != bgmSlider.value)
        {
            bgmVolume = bgmSlider.value;
            OnChangedBGM.Invoke(bgmVolume);
        }
        if (sfxVolume != sfxSlider.value)
        {
            sfxVolume = sfxSlider.value;
            OnChangedSFX.Invoke(sfxVolume);
        }
        if (isGhostMode != ghostModeToggle.isOn)
        {
            isGhostMode = ghostModeToggle.isOn;
            OnChangedGhostMode.Invoke(isGhostMode);
        }
    }

    private void Cancel()
    {
        bgmSlider.value = bgmVolume;
        sfxSlider.value = sfxVolume;
        ghostModeToggle.isOn = isGhostMode;
    }
}
