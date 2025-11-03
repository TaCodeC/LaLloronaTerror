using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class menuManag : MonoBehaviour
{
    public GameObject Creditos;
    public GameObject creditsbtn;
    public GameObject exitCreditsbtn;
    public GameObject playbtn;
    void Start()
    {
        exitCreditsbtn.SetActive(false);
        Creditos.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void play()
    {
        SceneManager.LoadScene("Cutscene");
    }
    public void showCredits()
    {
        exitCreditsbtn.SetActive(true);
        Creditos.SetActive(true);
        playbtn.SetActive(false);
        creditsbtn.SetActive(false);
    }
    public void hideCredits()
    {
        Creditos.SetActive(false);
        exitCreditsbtn.SetActive(false);
        playbtn.SetActive(true);
        creditsbtn.SetActive(true);
    }
}
