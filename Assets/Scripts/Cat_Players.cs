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
    }

    void FixedUpdate()
    {
        
    }
}
