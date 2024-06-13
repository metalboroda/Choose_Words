using Assets.__Game.Resources.Scripts.SOs;
using UnityEngine;

namespace Assets.__Game.Resources.Scripts._GameStuff
{
  public class SubLevel : MonoBehaviour
  {
    [SerializeField] private CorrectValuesContainerSo _correctValuesContainerSo;

    public CorrectValuesContainerSo GetCorrectValuesContainerSo() => _correctValuesContainerSo;
  }
}