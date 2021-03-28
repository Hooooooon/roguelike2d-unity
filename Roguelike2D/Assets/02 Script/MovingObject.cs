using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{
    // 수 초 동안 오브젝트를 움직이게 할 시간 단위
    public float moveTime = 0.1f;
    // 이동할 레이어가 열려있고, 그 곳으로 이동하려 할때, 충돌이 일어났는지 체크할 장소
    public LayerMask blockingLayer;

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;               // 움직일 유닛의 RigidBoxy2D 컴퍼넌트의 레퍼런스 저장
    private float inverseMoveTime;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f / moveTime;    // movetime의 역수를 주면서, 나누기 보다 효율적인 계산(곱셈)을 수행함

    }

    // 입력받은 방향 값들(xDir, yDir)을 기반으로 끝나는 위치를 계산한다.
    protected bool Move(int xDir, int yDir, out RaycastHit2D hit){ // out 키워드는 입력을 참조로 받는다
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir , yDir);

        boxCollider.enabled = false;        // Ray 를 사용할때 자기 자신과 부딫치지 않게 하기위해 해제한다.
        hit = Physics2D.Linecast(start, end, blockingLayer); // blockingLayer와 충돌 검사
        boxCollider.enabled = true;

        if(hit.transform == null){          // hit.transfor이 null 이라면 라인으로 검사한 공간이 열려있고(blockingLayer가 없다), 그곳으로 이동할 수 있다는 뜻
            StartCoroutine(SmoothMovement(end));
            return true;                    // 이동 할 수 있음
        }
        return false;                       // 이동 할 수 없음
    }


    protected IEnumerator SmoothMovement(Vector3 end){ // 반복 사용 IEnumerator(열거자)
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude; // 현재위치 - 목적지위치 -> 가야할 거리 Magnitude - Vector 길이, sqrMagnitude 벡터 길이 제곱

        while(sqrRemainingDistance > float.Epsilon){        // float.Epsilon (0에 가까운 수)
            // MoveTowards 함수는 현재 위치를 직선상에 목표 포인트로 이동
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
            rb2D.MovePosition(newPosition); // 이동
            // 이동 이후에 남은 거리를 재계산
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }
    }
    // generic 키워드를 사용한 이유, 플레이어와 적이 movingObject를 상속할텐데, 적은 플레이어와 상호작용하고, 
    // 플레이어는 벽과 상호작용하기에 나중에 상호작용할 hitComponenet의 종류를 알수없음
    protected virtual void AttemptMove <T> (int xDir, int yDir)
        where T : Component                     // T는 유닛이 막혔을때, 유닛이 반응할 컴포넌트 타입을 가리킨다, 적일 경우 막힌 유닛은(플레이어), 플레이어일 경우 막힌 유닛은(벽돌)
    {
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit); // 이동하는데 성공하면 True, 아니면 false

        if(hit.transform == null)               // out 키워드로 참조받아서 null 인지 아닌지 확인.
            return;                             // 만약 Move 에서 라인캐스트가 다른 것과 부딫지지 않았다면, 종료

        // 충돌한 오브젝트의 컴포넌트 레퍼런스를 T타입의 컴포넌트로 할당
        T hitComponent = hit.transform.GetComponent<T>();
        if(!canMove && hitComponent != null)    // 움직이던 오브젝트가 막혔고, 상호작용할 수 있는 오브젝트와 충돌했을시
            OnCantMove(hitComponent);
    }
    protected abstract void OnCantMove <T> (T component) // 상속한 자식클래스에서 완성, 추상화 함수이기에 괄호는 사용 안함
        where T : Component;            
}
