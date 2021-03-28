using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    public GameObject gameManager;      // gameManager 프리팹 저장

    private void Awake() {
        // gameManager 값이 없으면 gameManager 생성
        if(GameManager.instance == null)    
            Instantiate(gameManager);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
