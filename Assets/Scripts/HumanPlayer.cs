using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerMovement : PlayerAbilites
{
    private float horizontalInput;
    private float verticalInput;
    public float speed = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        transform.Translate(Vector3.forward * Time.deltaTime * speed * verticalInput);
        transform.Translate(Vector3.right * Time.deltaTime * speed * horizontalInput);

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            CickBall();
        }
    }

    void PauseGame()
    {
        Application.Quit();
    }
}
