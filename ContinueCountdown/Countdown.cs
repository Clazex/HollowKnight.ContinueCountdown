namespace ContinueCountdown;

public sealed partial class ContinueCountdown {
	public bool CountingDown { get; private set; }

	public bool IsCountingDown() => CountingDown;

	private LayoutRoot? layout = null;
	private TextObject? text = null;

	private IEnumerator DoCountdown(On.GameManager.orig_PauseGameToggle orig, GameManager self) {
		if (CountingDown) {
			yield break;
		}

		UIManager ui = self.ui;
		InputHandler ih = InputHandler.Instance;

		if (!self.TimeSlowed // From original method
			&& self.gameState == GameState.PAUSED // is un-pausing
			&& !ih.inputActions.pause.WasPressed // not via Esc
		) {
			// Break UIManager#SetState & UIManager#UIClosePauseMenu method into parts
			ui.uiState = UIState.PLAYING;
			ih.StopUIInput();
			ui.StartCoroutine(ui.HideCurrentMenu());

			CountingDown = true;
			LogDebug("Starting countdown");

			for (int i = 3; i > 0; i--) {
				text!.Text = i.ToString();

				// WaitForSeconds does not work as the game is paused now
				yield return new WaitForSecondsRealtime(1);
			}

			CountingDown = false;
			text!.Text = "";
			LogDebug("Finished countdown");

			ReflectionHelper.CallMethod(ui, "StartMenuAnimationCoroutine",
				ui.FadeOutCanvasGroup(ui.modalDimmer)
			);
		}

		yield return orig(self);
	}
}
