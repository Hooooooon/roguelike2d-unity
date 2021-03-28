using System.Collections;
using System.Collections.Generic; // LIST 사용
using UnityEngine;
using System;      // 직렬화를 사용하기 위한 namespacee
using Random = UnityEngine.Random; // Random 객체 사용 

public class BoardManager : MonoBehaviour
{

    // 직렬화를 사용하면 하위 속성에 있는 클래스를 속성에 포함시킬 수 있다.
    [Serializable]
    public class Count{
        public int minimum;     // Count 클래스의 최소값
        public int maxinum;     // Count 클래스의 최대값

        // 할당 생성자
        public Count(int min, int max){
            minimum = min;
            maxinum = max;
        }
    }

    public int columns = 8;             // 게임보드 열의 개수
    public int rows = 8;                // 게임보드 행의 개수

    public Count wallCount = new Count(5,9);    // 스테이지(레벨)에 따른 벽의 개수
    public Count foodCount = new Count(1,5);    // 스테이지(레벨)에 따른 음식의 개수
    public GameObject exit;                     // 출구 타일 1개
    public GameObject[] floorTiles;             // 바닥 타일
    public GameObject[] wallTiles;              // 벽 타일
    public GameObject[] foodTiles;              // 음식 타일
    public GameObject[] enemyTile;              // 적 타일
    public GameObject[] outerWallTiles;         // 외벽 타일

    private Transform boardHolder;      // 사용할 객체들을 자식으로 받음
    private List<Vector3> gridPositions = new List<Vector3>();
    //  gridPositions 리스트 재설정
    void InitaliseList(){
        // gridPositions 초기화
        gridPositions.Clear();

        // 리스트 채우기
        // 열에 1을 뺀 이유는 floor 타일 가장자리를 남겨두기 위해(탈출이 불가능해 질 수 있어서.)
        for(int x = 1; x < columns - 1; x++){
            for(int  y = 1; y < rows - 1; y++){
                // 게임 상에서 벽, 아이템, 몬스터 등이 있을 수 있는 위치
                // gridPositions 리스트에 추가
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }
    
    // 외벽과 바닥 타일을 생성
    void BoardSetup(){
        // boardHolder 생성
        boardHolder = new GameObject("Board").transform;

        for(int x = -1; x < columns + 1; x++){
            for(int y = -1; y < rows + 1; y++){
                // floorTiles 랜덤 선택
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];

                // 외벽이면 outerWallTiles 랜덤 선택
                if (x == -1 || x == columns || y == -1 || y == rows)
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];

                // 타일 인스턴스화
                GameObject instance = 
                    Instantiate(toInstantiate, new Vector3(x, y, 0f)
                    , Quaternion.identity);
                
                // boardHolder의 자식으로 만듬
                instance.transform.SetParent(boardHolder);
            }
        }
    }

    // 위치 랜덤 변환
    Vector3 RandomPosition(){
        // 0 ~ gridPositions 리스트에 저장된 위치 값의 개수
        int randomIndex = Random.Range(0, gridPositions.Count);

        // randomIndex에 따른 gridPositions 대입
        Vector3 randomPosition = gridPositions[randomIndex];

        // 같은 장소에 두개 이상의 오브젝트를 만들지 않기 위해서 사용한 각자 위치를 리스트에서 제거
        gridPositions.RemoveAt(randomIndex);

        // 무작위로 선택된 랜덤 포지션 반환
        return randomPosition;
    }
    // 생성할 객체(벽, 음식, 적)을 랜덤 생성
    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum){
        // 최소값과 최대값을 기준으로 랜덤하게 생성
        int objectCount = Random.Range(minimum, maximum + 1);

        // objectCount에 도달할때까지 객체의 인스턴스화
        for (int i =0  ; i < objectCount; i++){
            // 랜덤 위치 지정
            Vector3 randomPosition = RandomPosition();

            // tileArray 배열로 부터, 소환할 랜덤 타일 가져오기
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];

            // 타일 생성
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    // 레벨에 따른 게임 구성 셋팅
    public void SetupScene(int level){
        // 외벽과 바닥 타일 생성
        BoardSetup();

        // gridPosition 리스트를 재설정
        InitaliseList();

        // 벽 타일을 최소값과 최대값 기준으로 랜덤하게 생성
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maxinum);

        // 음식 타일을 최소값과 최대값 기준으로 랜덤하게 생성
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maxinum);

        // 현재 레벨을 기준으로 적의 수를 결정
        int enemyCount = (int)Mathf.Log(level, 2f);

        // enemyCount를 기준으로 적 생성
        LayoutObjectAtRandom(enemyTile, enemyCount, enemyCount);

        // 우측 상단에 출구타일 생성
        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);

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
