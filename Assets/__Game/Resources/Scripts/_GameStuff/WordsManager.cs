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
  [SerializeField] private CorrectValuesContainerSo _correctValuesContainerSo;
  [Header("")]
  [SerializeField] private GameObject[] _subLevels;
  [Header("")]
  [SerializeField] private Button _submitButton;

  private List<WordButton> _wordButtons = new List<WordButton>();
  private HashSet<string> _correctValuesSet;
  private bool _canSubmit = true;
  private int _currentSubLevelIndex = 0;

  private GameBootstrapper _gameBootstrapper;

  private void Awake()
  {
    _gameBootstrapper = GameBootstrapper.Instance;

    _wordButtons.AddRange(GetComponentsInChildren<WordButton>());
    _submitButton.onClick.AddListener(OnSubmitButtonClick);
  }

  private void Start()
  {
    _correctValuesSet = new HashSet<string>(_correctValuesContainerSo.CorrectValues);

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
      _gameBootstrapper.StateMachine.ChangeState(new GameLoseState(_gameBootstrapper));
    }
    else
    {
      _currentSubLevelIndex++;

      if (_currentSubLevelIndex < _subLevels.Length)
      {
        ActivateSubLevel(_currentSubLevelIndex);
      }
      else
      {
        PlayWinningAudioClip();

        _gameBootstrapper.StateMachine.ChangeStateWithDelay(new GameWinState(_gameBootstrapper), 1.5f, this);
      }
    }
  }

  private void ActivateSubLevel(int index)
  {
    for (int i = 0; i < _subLevels.Length; i++)
    {
      _subLevels[i].SetActive(i == index);
    }

    _wordButtons.Clear();
    _wordButtons.AddRange(_subLevels[index].GetComponentsInChildren<WordButton>());

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