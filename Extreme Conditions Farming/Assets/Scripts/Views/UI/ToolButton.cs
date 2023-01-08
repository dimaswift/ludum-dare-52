using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ECF.Views.UI
{
    public class ToolButton : MonoBehaviour
    {
        [SerializeField] private Button clickButton;
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private GameObject selection;
        private Tool tool;

        private void Start()
        {
            clickButton.onClick.AddListener(() =>
            {
                Game.Instance.Tools.SetActiveTool(tool);
            });
            Game.Instance.Tools.OnToolChanged += OnToolChanged;
        }

        private void OnToolChanged()
        {
            selection.SetActive(Game.Instance.Tools.Current == tool);
        }

        public void SetUp(Tool tool)
        {
            this.tool = tool;
            icon.sprite = tool.icon;
            title.text = tool.displayName;
            OnToolChanged();
        }
    }
}