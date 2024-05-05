using System;
using UnityEngine;
using UnityEngine.EventSystems;
//using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour {
  //public static GameInput Instance { get; private set; }

  //public enum Binding {
  //  Move_Up,
  //  Move_Down,
  //  Move_Left,
  //  Move_Right,
  //  Pause,
  //}

  //private PlayerInputActions playerInputActions;

  //public event EventHandler OnPauseAction;

  ///// tab tuþu için event
  //public event EventHandler<OnVisualToggleActionEventArgs> OnVisualToggleAction;

  //public class OnVisualToggleActionEventArgs : EventArgs {
  //  public bool ShowVisual;
  //}

  //private bool ShowVisual = true;

  ///// click
  //public event EventHandler<OnClickActionEventArgs> OnClickAction;

  //public event EventHandler<OnClickActionEventArgs> OnTradeClickAction;

  ////public event EventHandler<OnClickActionEventArgs> OnLandClickAction;

  //public class OnClickActionEventArgs : EventArgs {
  //  public RaycastHit Hit;
  //  public bool isThiefPlaced;
  //}

  //[SerializeField] private LayerMask clickLayer;
  //[SerializeField] private LayerMask tradeLayer;

  //private void OnDestroy() {
  //  //playerInputActions.Player.Pause.performed -= Pause_performed;
  //  playerInputActions.Player.Click.performed -= Click_performed;

  //  playerInputActions.Dispose();
  //}

  //private void Awake() {
  //  Instance = this;

  //  /// open new input system
  //  playerInputActions = new PlayerInputActions();

  //  /// sistemi aç
  //  playerInputActions.Player.Enable();

  //  playerInputActions.Player.Pause.performed += Pause_performed;
  //  playerInputActions.Player.Click.performed += Click_performed;
  //  playerInputActions.Player.VisualToggle.performed += VisualToggle_performed;
  //}

  //private void VisualToggle_performed(InputAction.CallbackContext obj) {
  //  ShowVisual = !ShowVisual;
  //  OnVisualToggleAction?.Invoke(this, new OnVisualToggleActionEventArgs {
  //    ShowVisual = ShowVisual,
  //  });
  //}

  //private void Click_performed(InputAction.CallbackContext obj) {
  //  if (GameManager.Instance.IsGameOver()) {
  //    return;
  //  }

  //  Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
  //  if (Physics.Raycast(ray, out RaycastHit hit, 50f, clickLayer)) {
  //    if (!IsMouseOverUI(hit)) {
  //      // make move in your turn
  //      if (!TurnManager.Instance.IsMyTurn()) {
  //        return;
  //      }

  //      OnClickAction?.Invoke(this, new OnClickActionEventArgs {
  //        Hit = hit,
  //        isThiefPlaced = GameManager.Instance.IsThiefPlaced,
  //      });
  //      return;
  //    }
  //  }
  //  if (Physics.Raycast(ray, out hit, 50f, tradeLayer)) {
  //    if (!IsMouseOverUI(hit)) {
  //      // make move in your turn
  //      if (!TurnManager.Instance.IsMyTurn()) {
  //        return;
  //      }

  //      OnTradeClickAction?.Invoke(this, new OnClickActionEventArgs {
  //        Hit = hit,
  //      });
  //    }
  //  }
  //}

  //private bool IsMouseOverUI(RaycastHit hit) {
  //  var isPointOverUI = EventSystem.current.IsPointerOverGameObject();
  //  return isPointOverUI;
  //  //if (isPointOverUI) {
  //  //  // hýrsýz için özel kod
  //  //  if (!GameManager.Instance.IsThiefRolled()) {
  //  //    return false;
  //  //  }
  //  //  if (hit.transform.parent.TryGetComponent(out LandVisual _)) {
  //  //    return false;
  //  //  }

  //  //  return true;
  //  //} else {
  //  //  return false;
  //  //}
  //}

  //private void Pause_performed(InputAction.CallbackContext obj) {
  //  OnPauseAction?.Invoke(this, EventArgs.Empty);
  //}

  //public Vector2 GetMovementVectorNormalized() {
  //  Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

  //  inputVector = inputVector.normalized;

  //  return inputVector;
  //}
}