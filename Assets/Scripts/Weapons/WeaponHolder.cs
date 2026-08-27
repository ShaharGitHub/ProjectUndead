using TMPro;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    //[Header("References:")]
    //[SerializeField] private Transform m_promptCanvas;
    //[SerializeField] private TextMeshProUGUI m_InfoText;
    //[SerializeField] private TextMeshProUGUI m_costText;
    //private bool m_isShowen = false;

    //private WeaponManager m_weaponManager;
    //private Transform m_target;


    //private void Awake()
    //{
    //    m_weaponManager = GetComponentInChildren<WeaponManager>();
    //    if (m_weaponManager == null)
    //        return;

    //    m_weaponManager.OnInteractRay += HandleInteractRay;

    //    if (m_InfoText != null)
    //        m_InfoText.gameObject.SetActive(false);

    //    if (m_costText != null)
    //        m_costText.gameObject.SetActive(false);
    //}

    //#region Not sure if I want to...
    ////private void Start()
    ////{
    ////    m_target = FindFirstObjectByType<PlayerManager>().transform;
    ////}

    ////private void Update()
    ////{
    ////    CanvasFollowPlayer();
    ////}

    ////private void CanvasFollowPlayer()
    ////{
    ////    if (m_promptCanvas == null || m_target == null)
    ////        return;

    ////    if (m_isShowen)
    ////        m_promptCanvas.transform.rotation = m_target.rotation;
    ////    //m_promptCanvas.transform.LookAt(m_target);
    ////}
    //#endregion

    //private void HandleInteractRay(bool toShow)
    //{
    //    if (m_InfoText == null || m_costText == null)
    //        return;

    //    m_isShowen = toShow;

    //    // Info text
    //    m_InfoText.text = $"{GlobalData.Prompts.Interact} <color=#08FFFA>{m_weaponManager.GetInteractPrompt()}$</color>";
    //    m_InfoText.gameObject.SetActive(toShow);

    //    // Cost text (Showen only for shop type)
    //    if (!m_weaponManager.GetDestroyOnEquip())
    //    {
    //        string cost = m_weaponManager.GetLogic().GetData().Cost.ToString();

    //        m_costText.text = $"{GlobalData.Prompts.Price} <color=#FFCD20>{cost}$</color>";
    //        m_costText.gameObject.SetActive(toShow);
    //    }
    //}
}
