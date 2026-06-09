using UnityEngine;

namespace Legends.UI
{
    public abstract class UIScreen
    {
        public GameObject Root { get; protected set; }
        public bool IsBuilt => Root != null;

        public virtual void Show()
        {
            if (!IsBuilt) Root = Build();
            Root?.SetActive(true);
        }

        public void Hide()
        {
            if (IsBuilt) Root.SetActive(false);
        }

        protected abstract GameObject Build();
    }
}
