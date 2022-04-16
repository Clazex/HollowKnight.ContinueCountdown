namespace ContinueCountdown;

public sealed partial class ContinueCountdown : Mod, ITogglableMod {
	public static ContinueCountdown? Instance { get; private set; }

	private static readonly Lazy<string> Version = new(() => Assembly
		.GetExecutingAssembly()
		.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
		.InformationalVersion
#if DEBUG
		+ "-dev"
#endif
	);

	public override string GetVersion() => Version.Value;

	public override void Initialize() {
		if (Instance != null) {
			LogWarn("Attempting to initialize multiple times, operation rejected");
			return;
		}

		Instance = this;

		layout = new(true, "ContinueCountdown LayoutRoot");
		layout.VisibilityCondition = IsCountingDown;
		layout.Canvas.GetComponent<CanvasGroup>().blocksRaycasts = false;

		text = new(layout, "Countdown Number") {
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			TextAlignment = HorizontalAlignment.Center,
			FontSize = 192,
			Font = UI.TrajanBold
		};

		On.GameManager.PauseGameToggle += DoCountdown;
	}

	public void Unload() {
		On.GameManager.PauseGameToggle -= DoCountdown;
		layout!.Destroy();

		Instance = null;
	}
}
