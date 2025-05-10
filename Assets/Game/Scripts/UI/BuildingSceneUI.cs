using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Scripts.UI
{
    public class BuildingSceneUI : MonoBehaviour
    {
        public void StartGame()
        {
            SceneManager.LoadScene("GameplayScene");
        }
    }
}