using UnityEngine;

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


        UI_Cats.instance.JoinGame(this);
    }

    void FixedUpdate()
    {
        
    }
}
