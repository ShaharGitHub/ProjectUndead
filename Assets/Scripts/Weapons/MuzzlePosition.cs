using System.Collections;
using UnityEngine;

public class MuzzlePosition : MonoBehaviour
{
    [SerializeField] private Transform m_muzzleFireLight;

    public void MuzzleLight()
    {
        StartCoroutine(MuzzleLightRoutine());
    }

    IEnumerator MuzzleLightRoutine()
    {
        m_muzzleFireLight.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.01f);
        m_muzzleFireLight.gameObject.SetActive(false);
    }
}
