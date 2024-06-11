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
  [SerializeField] private Button _submitButton;

  private List<WordButton> _wordButtons = new List<WordButton>();
  private HashSet<string> _correctValuesSet;

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
  }

  private void OnSubmitButtonClick()
  {
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
      _gameBootstrapper.StateMachine.ChangeState(new GameLoseState(_gameBootstrapper));
    else
      _gameBootstrapper.StateMachine.ChangeState(new GameWinState(_gameBootstrapper));
  }
}