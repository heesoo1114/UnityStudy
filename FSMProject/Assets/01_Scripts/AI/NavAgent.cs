using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NavAgent : MonoBehaviour
{
    private PriorityQueue<Node> _openList;
    private List<Node> _closeList;

    private List<Vector3Int> _routePath;

    public float speed = 5f;
    public bool cornerCheck = false;
    private bool _isMove = false;
    private int _moveIdx = 0; // 라우트 패스의 몇 번째를 진행하고 있는지
    private Vector3 _nextPos; // 다음에 이동할 워드 포지션

    private Vector3Int _currentPosition; // 현재 타일 위치
    private Vector3Int _destination; // 목표 타일 위치

    [SerializeField]
    private Tilemap _tilemap;

    private LineRenderer _lineRenderer;

    private void Awake()
    {
        _openList = new PriorityQueue<Node>();
        _closeList = new List<Node>();
        _routePath = new List<Vector3Int>();
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        Vector3Int cellPos = _tilemap.WorldToCell(transform.position);
        _currentPosition = cellPos;
        transform.position = _tilemap.GetCellCenterWorld(cellPos);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mPos = Input.mousePosition;
            mPos.z = 0;
            Vector3 world = Camera.main.ScreenToWorldPoint(mPos);
            Vector3Int cellPos = MapManager.Instance.GetTilePos(world); // 이걸로 월드를 타일 맵 포지션으로 변경
            
            _destination = cellPos;

            if (CalcuRoute())
            {
                PrintRoute();
                _moveIdx = 0;
                _isMove = true;
                SetNextTarget();
            }
        }

        if (_isMove)
        {
            Vector3 dir = _nextPos - transform.position;
            transform.position += dir.normalized * speed * Time.deltaTime;
            if (dir.magnitude <= 0.05)
            {
                SetNextTarget();
            }
        }
    }

    private void SetNextTarget()
    {
        if (_moveIdx >= _routePath.Count)
        {
            _isMove = false;
            return;
        }

        _currentPosition = _routePath[_moveIdx];    
        _nextPos = MapManager.Instance.GetWorldPos(_currentPosition);
        _moveIdx++;
    }

    private void PrintRoute() // 계산한 경로를 디버그로 찍어본다.
    {
        _lineRenderer.positionCount = _routePath.Count;
        _lineRenderer.SetPositions(_routePath.Select(p => MapManager.Instance.GetWorldPos(p)).ToArray());

        /*for (int i = 0; i < _routePath.Count; i++)
        {
            Vector3 worldPos = MapManager.Instance.GetWorldPos(_routePath[i]);
            _lineRenderer.SetPosition(i, worldPos);
        }*/
    }

    private bool CalcuRoute()
    {
        _openList.Clear();
        _closeList.Clear();

        _openList.Push(new Node { pos = _currentPosition, _parent = null, G = 0, F = CalcH(_currentPosition) });

        bool result = false; // 갈 수 있는 곳인지
        int cnt = 0;  // 안전 코드
        
        while (_openList.Count > 0)
        {
            Node n = _openList.Pop(); // 가장 가깝게 갈 수 있는 녀석을 가져 옴
            FindOpenList(n);
            _closeList.Add(n); // n은 한 번 사용했으니 클로즈 리스트에 넣기
            if (n.pos == _destination) // 마지막 방문했던 녀석이 목적지다. 그럼 나간다.
            {
                result = true;
                break;
            }

            // 안전코드
            cnt++;
            if (cnt >= 10000)
            {
                Debug.Log("while 루프 너무 많이 돌아서 빠갬");
                break;
            }
        }

        if (result) // 길을 찾음
        {
            _routePath.Clear();
            Node last = _closeList[_closeList.Count - 1];
            while (last._parent != null)
            {
                _routePath.Add(last.pos);
                last = last._parent;
            }
            _routePath.Reverse(); // 역순 리스트를 출발점부터 다시 들어오게 뒤집어 줌
        }

        return result;
    }

    // Node n 과 연결된 오픈 리스트를 다 찾아서 _openList에 넣어줌
    private void FindOpenList(Node n)
    {
        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                if (x == y) continue; // 이건 현재 위치한 자리니까 무시
                
                Vector3Int nextPos = n.pos + new Vector3Int(x, y, 0);

                Node temp = _closeList.Find(x => x.pos == nextPos); // 이미 방문
                if (temp != null) continue;

                // 타일에서 진짜 갈 수 있는 곳인지
                if (MapManager.Instance.CanMove(nextPos))
                {
                    float g = (n.pos - nextPos).magnitude + n.G;

                    Node nextOpennode = new Node { pos = nextPos, _parent = n, G = g, F = g + CalcH(nextPos) };

                    // 넣기 전에 검사
                    Node exist = _openList.Contains(nextOpennode);

                    if (exist != null)
                    {
                        if (nextOpennode.G < exist.G)
                        {
                            exist.G = nextOpennode.G;
                            exist.F = nextOpennode.F;
                            exist._parent = nextOpennode._parent;
                        }
                    }
                    else
                    {
                        _openList.Push(nextOpennode);
                    }
                }
            }
        }
    }

    private float CalcH(Vector3Int pos)
    {
        // F = G + H
        Vector3Int distance = _destination - pos;
        return distance.magnitude;
    }
}
