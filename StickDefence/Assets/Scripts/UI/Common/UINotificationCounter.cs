using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Common
{
    public class UINotificationCounter : UIBehaviour
    {
        [SerializeField] protected Image _notificationImage;
        [SerializeField] protected TextMeshProUGUI _numberLabel;

        public UINotificationCounter SetNotificationCount(int count)
        {
            if (count > 0)
            {
                _notificationImage.enabled = true;
                _numberLabel.text = count.ToString();
            }
            else
            {
                _notificationImage.enabled = false;
                _numberLabel.text = string.Empty;
            }

            return this;
        }

        public void SetNotification(bool value)
        {
            SetNotificationCount(value ? 1 : 0);
        }
    }
}