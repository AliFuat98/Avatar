using UnityEngine;

public class PlayerVisual : MonoBehaviour {
  [SerializeField] private MeshRenderer capsuleMeshRenderer;

  private Material material;

  private void Awake() {
    material = new Material(capsuleMeshRenderer.material);
    capsuleMeshRenderer.material = material;
  }

  public void SetPlayerColor(Color color) {
    material.color = color;
  }
}