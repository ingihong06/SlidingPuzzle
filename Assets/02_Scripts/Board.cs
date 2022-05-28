using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField]
    private GameObject tilePrefab;      // 숫자 타일 프리팹
    [SerializeField]
    private Transform tilesTransform;      // 타일이 배치되는 Board 오브젝트의 Transform
    private Vector2Int puzzleSize = new Vector2Int(4, 4);       // 4 x 4 퍼즐
    private List<Tile> tileList;
    private float neighborTileDistance = 102;
    public Vector3 EmptyTilePosition { set; get; }

    private IEnumerator Start()
    {
        tileList = new List<Tile>();

        SpawnTiles();

        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(tilesTransform.GetComponent<RectTransform>());

        yield return new WaitForEndOfFrame();

        tileList.ForEach(x => x.SetCorrectPosition());

        StartCoroutine("OnSuffle");
    }

    private void SpawnTiles()
    {
        for ( int y = 0; y < puzzleSize.y; ++y)
        {
            for ( int x = 0; x < puzzleSize.x; ++x)
            {
                GameObject clone = Instantiate(tilePrefab, tilesTransform);
                Tile tile = clone.GetComponent<Tile>();

                tile.Setup(this, puzzleSize.x * puzzleSize.y , y * puzzleSize.x + x + 1);

                tileList.Add(tile);
            }
        }
    }
    private IEnumerator OnSuffle()
    {
        float current = 0;
        float percent = 0;
        float time = 1.5f;
        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / time;
            int index = Random.Range(0, puzzleSize.x * puzzleSize.y);
            tileList[index].transform.SetAsLastSibling();

            yield return null;
        }
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
    public void IsGameOver()
    {
        List<Tile> tiles = tileList.FindAll(x => x.IsCorrected == true);
    }
}
