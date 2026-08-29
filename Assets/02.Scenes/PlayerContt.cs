using System;
using _02.Scenes;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerContt : MonoBehaviour
{
    private PlayerInputt input;
    private Rigidbody2D rigid;
    
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInputt>();
        input.OnJumpKeyPressed += Jump;
    }

    public void Jump()
    {
        rigid.AddForceY(12,ForceMode2D.Impulse);
    }
}

