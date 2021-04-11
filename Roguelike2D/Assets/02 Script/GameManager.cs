using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;  // 다른 클래스에서도 GameManager 접근 가능 (singleton)
    public BoardManager boardScript;        // 레벨을 설정할 boardManager에 대한 참조를 저장
    public int playerFoodPoints = 100;      // 
    [HideInInspector]public bool playerTurn = true;
    
    private int level = 3;                  // Day1, 게임에서 현재 레벨 번호

    // Awake는 Start 함수 전에 호출됨.
    void Awake() {
        // instance가 null이라면 이 스크립트를 추가
        if (instance == null)
            instance = this;
        // 만약 instance가 이 스크립트(this)가 아니라면 gameObject를 삭제, 중복 생성 방지
        else if (instance != this)
            Destroy(gameObject);
        // 다른 씬으로 넘어갈 때 gameObject가 사라지지 않는다.
        DontDestroyOnLoad(gameObject);

        // Call by reference
        // 연결된 boardManager 스크립트에 대한 구성 요소 참조 가져오기
        boardScript = GetComponent<BoardManager>();

        // 첫 번째 레벨을 초기화
        InitGame();
    }

    void InitGame(){
        // 현재 레벨 번호 전달(적이 얼마나 나올지 결정)
        boardScript.SetupScene(level);
    }

    public void GameOver(){
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
