using UnityEngine;
using UnityEngine.UI;

public class Cat_Players : MonoBehaviour
{
    public int playerID; // Se crea en void Start, no manipular en Inspector
    [SerializeField] private Renderer[] catMeshRenderers; // Body , Eyes, Nose
    [SerializeField] private Renderer extraEye; // Eye



    [System.Serializable]
    public struct IndividualAttributes
    {
        public Material[] catMaterials; // Body , Eyes, Nose
    }
    [SerializeField] private IndividualAttributes[] individualAttributes;

    [SerializeField] private Animator eatBarAnimator;
    [SerializeField] private Vector3 eatBarObjOffset;
    [SerializeField] private GameObject eatBarObj;
    [SerializeField] private Image eatBarFill;
    private bool lockAppearEatBar;


    // Library for management scripts
    public Cat_Movement catMovementScr;
    public Cat_Inputs catInputsScr;
    public Cat_Attack catAttackScr;
    public Cat_Lives catLivesScr;
    public Cat_Eat catEatScr;
    public Cat_Animations catAnimationsScr;
    public Gatocidad gatocidadScr;




    void Start()
    {
        playerID = GameObject.FindGameObjectsWithTag("Player").Length;
        int index = 0;
        foreach (Renderer mesh in catMeshRenderers)
        {
            mesh.material = individualAttributes[playerID - 1].catMaterials[index];
            index++;
        }
        extraEye.material = individualAttributes[playerID - 1].catMaterials[1];

        eatBarObj.SetActive(false);
        eatBarObj.transform.SetParent(GameObject.Find("CanvasUI-NoCambiarNombre").transform);

        UI_Cats.instance.JoinGame(this);
    }

    void FixedUpdate()
    {
        if(catEatScr.targetFood != null && catEatScr.foodProgress.ContainsKey(catEatScr.targetFood) &&
            catEatScr.holdTime > 0.2f && catEatScr.currentGrabbedFood == null)
        {
            if(lockAppearEatBar == false)
            {
                eatBarObj.SetActive(false);
                eatBarObj.SetActive(true);
                catAnimationsScr.animator.SetBool("Eating", true);
                lockAppearEatBar = true;
            }

            eatBarFill.fillAmount = catEatScr.foodProgress[catEatScr.targetFood] / catEatScr.targetFood.timeToEat;
        }
        else
        {
            if (lockAppearEatBar == true)
            {
                eatBarAnimator.SetBool("Dissapear", true);
                catAnimationsScr.animator.SetBool("Eating", false);
                lockAppearEatBar = false;
            }
        }
        // set eatBarObj position in canvas relative to this transform.position
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + eatBarObjOffset);
        eatBarObj.transform.position = screenPos;


    }
}
