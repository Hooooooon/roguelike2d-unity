using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public Sprite dmgSprite;            // 벽을 한번 때렸을때 보여줄 스프라이트
    public int hp = 4;                  // 벽의 체력

    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void DamageWall(int loss){
        spriteRenderer.sprite = dmgSprite;
        hp -= loss;
        if(hp <= 0)
            gameObject.SetActive(false);
    }
}
