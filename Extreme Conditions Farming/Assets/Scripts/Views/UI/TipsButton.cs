using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ECF.Views.UI
{
    [RequireComponent(typeof(Button))]
    public class TipsButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI title;

        private GameObject[] tips;
        private bool isTipsActive;
        private Button button;
        private void Awake()
        {
            tips = GameObject.FindGameObjectsWithTag("Tutorial");
            button = GetComponent<Button>();
            button.onClick.AddListener(ToggleTips);
            isTipsActive = true;
            ToggleTips();
        }

        private void ToggleTips()
        {
            
            isTipsActive = !isTipsActive;
            button.image.color = isTipsActive ? Color.green : Color.gray;
            title.text = isTipsActive ? "Show Tips" : "Hide Tips";
            foreach (GameObject tip in tips)
            {
                tip.SetActive(isTipsActive);
            }
        }
    }
}