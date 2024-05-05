using Unity.Netcode;
using UnityEngine.SceneManagement;

public static class Loader {

  public enum Scene {
    MainMenuScene,
    GameScene,
    LoadingScene,
    LobbyScene,
    CharacterSelectScene
  }

  private static Scene targetScene;

  public static void Load(Scene targetScene) {
    Loader.targetScene = targetScene;

    SceneManager.LoadScene($"{Scene.LoadingScene}");
  }

  public static void LoaderCallBack() {
    SceneManager.LoadScene($"{targetScene}");
  }

  public static void LoadNetwork(Scene targetScene) {
    NetworkManager.Singleton.SceneManager.LoadScene($"{targetScene}", LoadSceneMode.Single);
  }
}