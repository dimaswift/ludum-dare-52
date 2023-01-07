using System.Threading.Tasks;
using ECF.Domain.Common;
using UnityEngine;

namespace ECF.Views.UI
{
    public class Window : MonoBehaviour
    {
        public ObservableValue<bool> IsShown { get; } = new (false);
        protected virtual Task ProcessShow() { return Task.CompletedTask; }
        protected virtual Task ProcessHide() { return Task.CompletedTask; }
        
        protected virtual void OnInit() {}

        public void Init()
        {
            OnInit();
        }

        public async Task Show()
        {
            gameObject.SetActive(true);
            await ProcessShow();
            IsShown.Value = true;
        }
        
        public async Task Hide()
        {
            await ProcessHide();
            gameObject.SetActive(false);
            IsShown.Value = false;
        }
    }
}