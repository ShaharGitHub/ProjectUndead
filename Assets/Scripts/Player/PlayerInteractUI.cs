using TMPro;
using UnityEngine;

public class PlayerInteractUI : BasePlayerService
{
    [Header("References:")]
    [SerializeField] private TextMeshProUGUI m_InfoText;
    [SerializeField] private TextMeshProUGUI m_costText;


    private void Awake()
    {
        if (m_InfoText != null)
            m_InfoText.gameObject.SetActive(false);

        if (m_costText != null)
            m_costText.gameObject.SetActive(false);
    }

    public void HandleInteractRay(IInteractable interactable, bool toShow)
    {
        if (m_InfoText == null || m_costText == null || interactable == null)
            return;

        // Info text
        m_InfoText.text = $"{GlobalData.Prompts.Interact} <color=#08FFFA>{interactable.GetInteractPrompt()}</color>";
        m_InfoText.gameObject.SetActive(toShow);


        if (interactable is IPriceable priceable)
        {
            string pricePrompt = priceable.GetPricePrompt();

            if (pricePrompt != "")
            {
                // Cost text (Showen only for shop type)
                m_costText.text = $"{GlobalData.Prompts.Price} <color=#FFCD20>{priceable.GetPricePrompt()}$</color>";
                m_costText.gameObject.SetActive(toShow);
            }
            else
            {
                m_costText.gameObject.SetActive(false);
            }
        }
        else
        {
            m_costText.gameObject.SetActive(false);
        }
    }
}
