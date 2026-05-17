using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UI_Cats : MonoBehaviour
{
    public static UI_Cats instance;

    [System.Serializable]
    public struct UIStruct
    {
        public GameObject wholePlayerUI;
        public Image catocidadFillImage;
        public TextMeshProUGUI catLives;

        [Header("No Tocar")]
        public Cat_Players catPlayersScr;
    }
    [SerializeField] private UIStruct[] catUI;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public void JoinGame(Cat_Players selectedCMScr)
    {
        catUI[selectedCMScr.playerID - 1].catPlayersScr = selectedCMScr;
        catUI[selectedCMScr.playerID - 1].wholePlayerUI.SetActive(true);
    }



    void Start()
    {
        StartCoroutine(ResetUI_Values());
    }

    void FixedUpdate()
    {
        
    }

    public IEnumerator ResetUI_Values()
    {
        while(true)
        {
            
            foreach (UIStruct ui in catUI)
            {
                if (ui.catPlayersScr != null)
                {
                    ui.catocidadFillImage.fillAmount = ui.catPlayersScr.gatocidadScr.gatocidad / ui.catPlayersScr.gatocidadScr.maxGatocidad;

                }
            }

            yield return new WaitForSeconds(0.25f);
        }
    }    
}
