using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SilddingPuzzle
{
    public class Board : MonoBehaviour
    {
        [SerializeField]
        private GameObject tilePrefab;                          // 숫자 프리팹 타일

        [SerializeField]
        private Transform tileParent;                           // 타일에 배치되는 "Board" 오브젝트의 Transform

        private List<Tile> tileList;                            // 생성한 타일 정보 저장

        private Vector2Int puzzleSize = new Vector2Int(4, 4);   // 4 * 4 퍼즐
        private float neighborTileDistance = 102;              // 인접한 타일 사이의 거리. 별도로 계산할 수도 있다.

        public Vector3 EmptyTilePosition { set; get; }          //빈 타일의 위치


        private IEnumerator Start()
        {
            tileList = new List<Tile>();
            SpawnTiles();

            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(tileParent.GetComponent<RectTransform>());
            
            // 현재 프레임이 종료될때까지 대기
            yield return new WaitForEndOfFrame();


            // tileList에 있는 모든 요소의 SetCorrectPosition() 메소드 호출
            tileList.ForEach(x => x.SetCorrectPosition());


            StartCoroutine("OnSuffle");
        }

        private void SpawnTiles()
        {
            for (int y = 0; y < puzzleSize.y; y++)
            {
                for (int x = 0; x < puzzleSize.x; x++) 
                {
                    GameObject clone = Instantiate(tilePrefab, tileParent);
                    Tile tile = clone.GetComponent<Tile>();

                    tile.Setup(this, puzzleSize.x * puzzleSize.y, y * puzzleSize.x + x + 1);

                    tileList.Add(tile);
                }
            }
        }

        private IEnumerator OnSuffle()
        {
            float current = 0;
            float percent = 0;
            float time = 1.5f;

            while(percent <1)
            {
                current += Time.deltaTime;
                percent = current / time;

                int index = Random.Range(0, puzzleSize.x * puzzleSize.y);
                tileList[index].transform.SetAsLastSibling();

                yield return null;
            }

            // 원래 셔플 방식은 다른 방식이었는데 UI, GridLayoutGroup을 사용하다보니 자식의 위치를 바꾸는 것으로 설정
            // 그래서 현재 타일리스트의 마지막에 있는 요소가 무조건 빈 타일
            EmptyTilePosition = tileList[tileList.Count - 1].GetComponent<RectTransform>().localPosition;
        }

        public void IsMoveTile(Tile tile)
        {
            if (Vector3.Distance(EmptyTilePosition, tile.GetComponent<RectTransform>().localPosition) == neighborTileDistance)
            {
                Vector3 goalPosition = EmptyTilePosition;

                EmptyTilePosition = tile.GetComponent<RectTransform>().localPosition;

                tile.OnMoveTo(goalPosition);
            }
                    }
    }
}