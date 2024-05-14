using UnityEngine;
using UnityEngine.UI;

public class DynamicCellSize : MonoBehaviour {
  public GridLayoutGroup gridLayoutGroup;

  private RectTransform gridRectTransform;
  private Vector2 lastSize;

  private void Start() {
    gridRectTransform = gridLayoutGroup.GetComponent<RectTransform>();
    lastSize = new Vector2(gridRectTransform.rect.width, gridRectTransform.rect.height);

    UpdateCellSize();
  }

  private void Update() {
    if (gridRectTransform.rect.width != lastSize.x || gridRectTransform.rect.height != lastSize.y) {
      lastSize = new Vector2(gridRectTransform.rect.width, gridRectTransform.rect.height);

      UpdateCellSize();
    }
  }

  private void UpdateCellSize() {
    int rowCount = gridLayoutGroup.constraintCount;
    float cellWidth = (gridRectTransform.rect.width - ((rowCount - 1) * gridLayoutGroup.spacing.x)) / rowCount;
    float cellHeight = (gridRectTransform.rect.height - ((rowCount - 1) * gridLayoutGroup.spacing.y)) / rowCount;

    gridLayoutGroup.cellSize = new Vector2(cellWidth, cellHeight);
  }
}