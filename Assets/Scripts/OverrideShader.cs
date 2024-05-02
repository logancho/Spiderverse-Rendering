using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverrideShader : MonoBehaviour
{
    public float DotTile = 5.16f;
    public float DotRadius = 0.34f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //SetIDs();
        //rend = GetComponent<Renderer>();
        //MaterialPropertyBlock props = new MaterialPropertyBlock();
        //rend.GetPropertyBlock(props);
        //props.Clear();
        //props.SetColor("_Color", col);
        //rend.SetPropertyBlock(props);

        //SetIDs();
    }

    private void FixedUpdate()
    {
        // Get all renderers in the object's hierarchy
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        // Create a new MaterialPropertyBlock
        MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();

        // Set the property to the MaterialPropertyBlock
        //materialPropertyBlock.SetColor("_Color", newColor);
        materialPropertyBlock.SetFloat("_DotTile", DotTile);
        materialPropertyBlock.SetFloat("_Radius", DotRadius);

        // Apply the MaterialPropertyBlock to all renderers
        foreach (Renderer renderer in renderers)
        {
            renderer.SetPropertyBlock(materialPropertyBlock);
        }
    }
}
