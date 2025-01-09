using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
public class GunManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject confetti;
    public GameObject flag;

    public void SetGunOnTable()
    {

    }
    public void SetGunOnHand()
    {

    }
    [Button("Éä»÷×Ô¼º")]
    public async void Shot(bool isBullet)
    {
        flag.SetActive(true);
        //confetti.SetActive(true);
        confetti.GetComponent<ParticleSystem>().Play();
        GetComponent<AudioSource>().Play();
        await Task.Delay(2500);
        //confetti.GetComponent<ParticleSystem>().Stop();
        flag.SetActive(false);
        //confetti.SetActive(false);
    }
}
