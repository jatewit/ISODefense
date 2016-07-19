using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using Jatewit;

[RequireComponent(typeof(CanvasGroup))]
public abstract class PanelWindow : MonoBehaviour{
	public delegate void PanelWindowDelegate();
	
	public PanelWindowDelegate onWindowStartedClosing;
	public PanelWindowDelegate onWindowClosed;
	public PanelWindowDelegate onWindowStartedShowing;
	public PanelWindowDelegate onWindowShown;
	
	protected CanvasGroup _canvasGroup;
	
	public bool shouldActivateOverlay = true;
	public bool isShowing { get; private set; }
	public bool isClosing { get; private set; }
	public bool isOpening { get; private set; }
	
	[SerializeField] private string openParameter = "Open";
	[SerializeField] private string openAnimationName = "Open";
	[SerializeField] private string closedAnimationName = "Close";
	[SerializeField] private bool pauseWhenShown = false;
	
	public static PanelWindow CurrentOpenPanel { get; private set; }
	public static PanelWindow LastClosePanel { get; private set; }
	public static bool IsPause { get; private set; }
	
	protected Animator _animator;
	private int _openParameterId;
	public bool IsAnimationInit {get; private set; }

	// Use this for initialization
	protected virtual void Awake() {
		_openParameterId = Animator.StringToHash (openParameter);
		_animator = GetComponent<Animator>();
		_canvasGroup = GetComponent<CanvasGroup>();
	}
	
	protected virtual void Start () {
		StartCoroutine(WaitForAnimationInit());
	}
	
	IEnumerator WaitForAnimationInit () {
		while (!_animator.isInitialized) {
			yield return null;
		}
		IsAnimationInit = true;
		Close();
	}
	
	// Update is called once per frame
	public virtual void Open() {
		gameObject.SetActive(true);
		transform.SetAsLastSibling();
		CurrentOpenPanel = this;

		if(pauseWhenShown) {
			IsPause = true;
		}
		
		isShowing = true;
		isOpening = true;
		
		if (_animator != null) {
			TimeManager.Instance.StartAnimation(_animator,()=>{
				isOpening = false; 
				if(onWindowShown != null) onWindowShown();
				SetSelected(gameObject);
			},openAnimationName);
			
			_animator.SetBool(_openParameterId,true);
		} else {
			isOpening = false; 
			if(onWindowShown != null) onWindowShown();
		}

		if(onWindowStartedShowing != null) onWindowStartedShowing();
	}
	
	public virtual void Close() {
		_canvasGroup.interactable = false;
		isClosing = true;
		SetSelected(null);
		CurrentOpenPanel = null;
		
		if (_animator != null) {
			TimeManager.Instance.StartAnimation(_animator,()=>{
				isShowing = false;
				isClosing = false;
				
				if (pauseWhenShown) {
					IsPause = false;
				}
				
				if(onWindowClosed != null) onWindowClosed();
				gameObject.SetActive(false);
				LastClosePanel = this;
			},closedAnimationName);
			
			_animator.SetBool(_openParameterId,false);
		} else {
			if(onWindowClosed != null) onWindowClosed(); 
			gameObject.SetActive(false);
		}

		if(onWindowStartedClosing != null) onWindowStartedClosing();
	}
	
	public virtual void OpenPreviousPanel() {
		if (LastClosePanel != null) {
			LastClosePanel.Open();
		}
	}
	
	protected virtual void OnDestroy() {
		onWindowStartedClosing = null;
		onWindowClosed = null;
		onWindowStartedShowing = null;
		onWindowShown = null;
	}
	
	void SetSelected(GameObject go)
	{
		EventSystem.current.SetSelectedGameObject(go);
		var standaloneInputModule = EventSystem.current.currentInputModule as StandaloneInputModule;
		if (standaloneInputModule != null)
			return;
		EventSystem.current.SetSelectedGameObject(null);
	}
}
