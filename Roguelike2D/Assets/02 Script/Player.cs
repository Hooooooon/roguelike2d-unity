using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MovingObject
{
    public int wallDamage = 1;      // 플레이어의 벽 부술떄 적용할 데미지
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1;

    private Animator animator;
    private int food;
    // Start is called before the first frame update
    protected override void Start()
    {
        animator = GetComponent<Animator>();

        food = GameManager.instance.playerFoodPoints;

        base.Start();
    }
    // 게임 오브젝트가 비활성화 되는 순간 호출
    private void OnDisable() {
        GameManager.instance.playerFoodPoints = food;
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        food--; // 움직일때마다 음식 점수 소모

        base.AttemptMove<T> (xDir, yDir);

        RaycastHit2D hit;
        CheckIfGameOver();
        //GameManager.instance.playerTurn = false;
    }

    private void CheckIfGameOver(){
        if (food <= 0)
            GameManager.instance.GameOver();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Called Update Func");
        if(!GameManager.instance.playerTurn) return;
        // 플레이어 차례가 아니면 갱신 X

        int horizontal = 0;
        int vertical = 0;
        // 키보드, 컨트롤러 입력 GetAxisRaw
        horizontal = (int) Input.GetAxisRaw("Horizontal");
        vertical = (int) Input.GetAxisRaw("Vertical");
        // 대각선 이동 방지
        if(horizontal != 0) // 수평으로 움직이면
            vertical = 0;
        
        if(horizontal != 0 || vertical != 0)    // 0이 아닌값을 가지는거면 움직이려는 뜻
            AttemptMove<Wall> (horizontal, vertical);
            // 제네릭 변수가 Wall 인 이유 
            // generic 키워드를 사용한 이유, 플레이어와 적이 movingObject를 상속할텐데, 적은 플레이어와 상호작용하고, 
            // 플레이어는 벽과 상호작용하기에 나중에 상호작용할 hitComponenet의 종류를 알수없음
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Exit"){
            Invoke("Restart", restartLevelDelay);   // 출구와 충돌후 1초 후 실행
            enabled = false;
        }
        else if (other.tag == "Food"){
             food += pointsPerFood;
             other.gameObject.SetActive(false);  // 음식 오브젝트 비활성화
        }
        else if (other.tag == "Soda"){
            food += pointsPerSoda;
            other.gameObject.SetActive(false);
        }

    }

    // 플레이어가 이동하려는 공간에 벽이 있고, 이에 막히는 경우의 행동    
    protected override void OnCantMove<T>(T component)
    {   // component -> wall로 변환
        Wall hitWall = component as Wall;
        // 플레이어가 벽에 대미지를 줄지 알리기 위해
        hitWall.DamageWall(wallDamage);

        animator.SetTrigger("playerAttack");
        
        throw new System.NotImplementedException();
    }

    private void Restart(){
        // 플레리어가 출구 오브젝트와 충돌할시
        Application.LoadLevel(Application.loadedLevel);
        // restart main Scene
    }

    public void LoseFood(int loss){
        animator.SetTrigger("playerHit");
        food -= loss;
        CheckIfGameOver();
    }
}
