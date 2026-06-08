using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Legends.Data;

namespace Legends.UI
{
    public class AthleteCardUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI nameLabel;
        [SerializeField] Slider dexBar, conBar, focBar, strBar, lckBar;

        public void Bind(AthleteState athlete)
        {
            nameLabel.text  = athlete.Name;
            dexBar.value    = athlete.Stats.Dexterity    / 10f;
            conBar.value    = athlete.Stats.Constitution / 10f;
            focBar.value    = athlete.Stats.Focus        / 10f;
            strBar.value    = athlete.Stats.Strength     / 10f;
            lckBar.value    = athlete.Stats.Luck         / 10f;
        }
    }
}
