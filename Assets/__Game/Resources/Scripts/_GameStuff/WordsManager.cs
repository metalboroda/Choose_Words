using __Game.Resources.Scripts.EventBus;
using Assets.__Game.Resources.Scripts._GameStuff;
using Assets.__Game.Resources.Scripts.Game.States;
using Assets.__Game.Resources.Scripts.SOs;
using Assets.__Game.Scripts.Infrastructure;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordsManager : MonoBehaviour
{
  [Header("")]
  [SerializeField] private SubLevel[] _subLevels;
  [Header("")]
  [SerializeField] private Button _submitButton;
  [Header("Effects")]
  [SerializeField] private ParticleSystem _winParticles;
  [SerializeField] private AudioSource _winAudioSource;
  [SerializeField] private ParticleSystem _loseParticles;
  [SerializeField] private AudioSource _loseAudioSource;

  private CorrectValuesContainerSo _currentCorrectValuesContainerSo;
  private List<WordButton> _wordButtons = new List<WordButton>();
  private HashSet<string> _correctValuesSet;
  private bool _canSubmit = true;
  private int _currentSubLevelIndex = 0;

  private Canvas _canvas;

  private GameBootstrapper _gameBootstrapper;

  private void Awake()
  {
    _gameBootstrapper = GameBootstrapper.Instance;

    _canvas = GetComponent<Canvas>();

    _wordButtons.AddRange(GetComponentsInChildren<WordButton>());
  }

  private void OnEnable()
  {
    _submitButton.onClick.AddListener(OnSubmitButtonClick);
  }

  private void Start()
  {
    _canvas.worldCamera = Camera.main;

    EventBus<EventStructs.VariantsAssignedEvent>.Raise(new EventStructs.VariantsAssignedEvent());

    ActivateSubLevel(_currentSubLevelIndex);
  }

  private void OnSubmitButtonClick()
  {
    if (_canSubmit == false) return;

    HashSet<string> selectedWords = new HashSet<string>();
    bool hasIncorrectSelection = false;

    foreach (var wordButton in _wordButtons)
    {
      string wordText = wordButton.GetWordText();

      if (wordButton.IsClicked)
      {
        selectedWords.Add(wordText);

        if (_correctValuesSet.Contains(wordText) == false)
        {
          hasIncorrectSelection = true;

          break;
        }
      }
      else
      {
        if (_correctValuesSet.Contains(wordText))
        {
          hasIncorrectSelection = true;

          break;
        }
      }
    }

    if (hasIncorrectSelection || selectedWords.Count != _correctValuesSet.Count)
    {
      _loseParticles.Play();
      _loseAudioSource.Play();

      _gameBootstrapper.StateMachine.ChangeStateWithDelay(new GameLoseState(_gameBootstrapper), 0.5f, this);
    }
    else
    {
      _currentSubLevelIndex++;

      PlayWinningAudioClip();

      _winParticles.Play();
      _winAudioSource.Play();

      if (_currentSubLevelIndex < _subLevels.Length)
        ActivateSubLevel(_currentSubLevelIndex);
      else
        _gameBootstrapper.StateMachine.ChangeStateWithDelay(new GameWinState(_gameBootstrapper), 1f, this);
    }

    EventBus<EventStructs.UiButtonEvent>.Raise(new EventStructs.UiButtonEvent());
  }

  private void ActivateSubLevel(int index)
  {
    for (int i = 0; i < _subLevels.Length; i++)
    {
      _subLevels[i].gameObject.SetActive(i == index);
    }

    _wordButtons.Clear();
    _wordButtons.AddRange(_subLevels[index].GetComponentsInChildren<WordButton>());

    _currentCorrectValuesContainerSo = _subLevels[index].GetCorrectValuesContainerSo();
    _correctValuesSet = new HashSet<string>(_currentCorrectValuesContainerSo.CorrectValues);

    _canSubmit = true;
  }

  private void PlayWinningAudioClip()
  {
    foreach (var wordButton in _wordButtons)
    {
      if (_correctValuesSet.Contains(wordButton.GetWordText()))
      {
        wordButton.PlayWordAudioCLip();

        break;
      }
    }
  }
}