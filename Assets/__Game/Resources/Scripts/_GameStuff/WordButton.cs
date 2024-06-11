using __Game.Resources.Scripts.EventBus;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.__Game.Resources.Scripts._GameStuff
{
  public class WordButton : MonoBehaviour
  {
    [Header("Visual")]
    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] private Sprite _selectedSprite;
    [Header("Audio")]
    [SerializeField] private AudioClip _wordAudioClip;

    public bool IsClicked { get; private set; }

    private bool _canPlayAudio = true;

    private Image _image;
    private Button _button;
    private TextMeshProUGUI _textMeshPro;

    private void Awake()
    {
      _image = GetComponent<Image>();
      _button = GetComponent<Button>();
      _textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
      _button.onClick.AddListener(OnButtonClick);
    }

    private void OnDisable()
    {
      _button.onClick.RemoveListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
      if (IsClicked == false)
        PlayWordAudioCLip();

      IsClicked = !IsClicked;
      _image.sprite = IsClicked ? _selectedSprite : _defaultSprite;
    }

    public string GetWordText()
    {
      return _textMeshPro.text;
    }

    private void PlayWordAudioCLip()
    {
      if (_canPlayAudio == false) return;

      EventBus<EventStructs.VariantAudioClickedEvent>.Raise(new EventStructs.VariantAudioClickedEvent { AudioClip = _wordAudioClip });

      StartCoroutine(DoResetPosibilityToPlayWordAudioCLip());
    }

    private IEnumerator DoResetPosibilityToPlayWordAudioCLip()
    {
      yield return new WaitForSeconds(3);

      _canPlayAudio = true;
    }
  }
}