using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class HitEffect : NetworkBehaviour
{
    public Renderer playerRender; // Reference to the Player Render
    public Material playerBaseMaterial; 
    public Material playerHitMaterial; 


    [SerializeField]
    private MaterialPropertyBlock materialProps;

    [SerializeField]
    private float flashDuration = 0.1f;  // Duration of the Flash Effect

    [SerializeField]
    private Color flashColor = Color.red; // Color of the Flash Effect

    [SerializeField]
    private float flashTimer = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        materialProps = new MaterialPropertyBlock();
        playerRender.material = playerBaseMaterial;
    }

   
    // Update is called once per frame
    void Update()
    {
        if(flashTimer > 0)
        {
            playerRender.material = playerHitMaterial; // change base material to hit material

            flashTimer -= Runner.DeltaTime;

            float lerpValue = Mathf.PingPong(Runner.DeltaTime * 10.0f, flashDuration) / flashDuration;
            Color lerpedColor = Color.Lerp(Color.white, flashColor, lerpValue);

            // Set Shader Properties fot the Flash Effect
            materialProps.SetColor("_FlashColor", lerpedColor);
            materialProps.SetFloat("_FlashAmount", 1.0f);

            // Apply the modified properties to the renderer
            playerRender.SetPropertyBlock(materialProps);
        }else
        {
            materialProps.SetFloat("_FlashAmount", 0f);
            playerRender.SetPropertyBlock(materialProps);
            playerRender.material = playerBaseMaterial;
        }
    }

    public void StartFlashEffect()
    {
        flashTimer = flashDuration;
    }
}
