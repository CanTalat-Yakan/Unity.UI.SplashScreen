using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Unity.Essentials
{
    public class SplashScreenCustomizer : MonoBehaviour
    {
        [Header("Text")]
        [SerializeField, TextArea(9, 7)] private string _paragraph;

        [Header("Textures")]
        [SerializeField] private Texture2D _topLeftLogo;

        [Space]
        [SerializeField] private Texture2D _leftLogo;
        [SerializeField] private Texture2D _rightLogo;

        [Space]
        [SerializeField] private Texture2D _logoSmall1;
        [SerializeField] private Texture2D _logoSmall2;
        [SerializeField] private Texture2D _logoSmall3;
        [SerializeField] private Texture2D _logoSmall4;

        [Header("Events")]
        [SerializeField] private UnityEvent _act1Event;
        [SerializeField] private UnityEvent _act3Event;
        [SerializeField] private UnityEvent _act2Event;
        [SerializeField] private UnityEvent _finalEvent;
        public bool IsLoadingScenes = false;

        public void OnFinalization() =>
            _finalEvent?.Invoke();

        public void OnAct1Enabled() =>
            _act1Event?.Invoke();

        public void OnAct2Enabled(UIDocument act2)
        {
            var rootAct2 = act2.rootVisualElement;
            if (rootAct2 is null)
                return;

            var logoElement = rootAct2.Q<VisualElement>("TopLeftLogo");
            if (logoElement != null && _topLeftLogo != null)
                logoElement.style.backgroundImage = new StyleBackground(_topLeftLogo);

            var unityLogoElem = rootAct2.Q<VisualElement>("LeftLogo");
            if (unityLogoElem != null && _leftLogo != null)
                unityLogoElem.style.backgroundImage = new StyleBackground(_leftLogo);

            var graphicsElem = rootAct2.Q<VisualElement>("RightLogo");
            if (graphicsElem != null && _rightLogo != null)
                graphicsElem.style.backgroundImage = new StyleBackground(_rightLogo);

            _act2Event?.Invoke();
        }

        public void OnAct3Enabled(UIDocument act3)
        {
            var rootAct3 = act3.rootVisualElement;
            if (rootAct3 is null)
                return;

            var logoElement = rootAct3.Q<VisualElement>("TopLeftLogo");
            if (logoElement != null && _topLeftLogo != null)
                logoElement.style.backgroundImage = new StyleBackground(_topLeftLogo);

            var logo1Element = rootAct3.Q<VisualElement>("Logo1");
            if (logo1Element != null && _logoSmall1 != null)
                logo1Element.style.backgroundImage = new StyleBackground(_logoSmall1);

            var logo2Element = rootAct3.Q<VisualElement>("Logo2");
            if (logo2Element != null && _logoSmall2 != null)
                logo2Element.style.backgroundImage = new StyleBackground(_logoSmall2);

            var logo3Element = rootAct3.Q<VisualElement>("Logo3");
            if (logo3Element != null && _logoSmall3 != null)
                logo3Element.style.backgroundImage = new StyleBackground(_logoSmall3);

            var logo4Element = rootAct3.Q<VisualElement>("Logo4");
            if (logo4Element != null && _logoSmall4 != null)
                logo4Element.style.backgroundImage = new StyleBackground(_logoSmall4);

            var paragraphLabel = rootAct3.Q<Label>("Paragraph");
            if (paragraphLabel != null)
                paragraphLabel.text = _paragraph;

            _act3Event?.Invoke();
        }
    }
}
