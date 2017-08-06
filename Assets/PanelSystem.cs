using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 0 Nothing (Empty)
// 1 Straight Vertical
// 2 Straight Horizontal
// 3 Curve Left to Up
// 4 Curve Up to Right
// 5 Curve Right to Down
// 6 Curve Down to Left
// 7 Curve (double) Left to Up / Right to Down
// 8 Curve (double) Down to Left / Up to Right
public enum PanelType
{
    Nothing = 0,
    StraightVertical = 1,
    StraightHorizontal,
    CurveLeftUp,
    CurveUpRight,
    CurveRightDown,
    CurveDownLeft,
    CurveLeftUp_RightDown,
    CurveDownLeft_UpRight
}

public enum FuseDirection
{
    Up = 0,
    Right,
    Down,
    Left
}

public enum MoveDirection
{
    None = 0,
    Right = 1,
    Up,
    Left,
    Down
}

//--------------------------------------------------------------------------
// [x, y]  x = 4, y = 4の場合
// [y]
// ↑
// 3
// 2
// 1
// 0
// 0 1 2 3 →[x]

public class PanelMatrix
{
    public long Size;
    public PanelType[,] Matrix;
    public GameObject[,] PanelObject;

    public PanelMatrix(long size)
    {
        Size = size;
        Initialize();
    }

    void Initialize()
    {
        Matrix = new PanelType[Size, Size];
        PanelObject = new GameObject[Size, Size];
    }

}


//--------------------------------------------------------------------------
//--------------------------------------------------------------------------
//--------------------------------------------------------------------------
//--------------------------------------------------------------------------
public class PanelSystem : MonoBehaviour
{

    public GameObject NoPanel;
    public GameObject PlainPanel;
    public GameObject FireRope;

    private float panelWidth;
    private float panelHeight;
    private int panelSize;
    private PanelType[] stage;

    private long currentX;
    private long currentY;
    private PanelMatrix Panel;
    private MoveDirection PanelDirection;
    private float moveDistance;
    private GameObject MovingPanel;
    private GameObject NonPanel;

    private long swapX1, swapX2;
    private long swapY1, swapY2;

    private FuseDirection CurrentFusePanelDirection;

    // Use this for initialization

    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------
    GameObject CreateDoublePanel(PanelType type)
    {
        GameObject obj;
        GameObject child;
        float x, y, r, x1, y1;
        Quaternion rot = Quaternion.Euler(0f, 0f, 0f);

        obj = Instantiate(PlainPanel, transform.position, transform.rotation) as GameObject;

        r = panelWidth / 2f;
        x = 0;

        for (int i = 0; i < 16; i++)
        {
            y = Mathf.Sqrt(-(x * x) + (r * r));
            x += panelWidth / 2f / 16f;
            x1 = x - panelWidth / 2f;
            y1 = y - panelHeight / 2f;
            Vector2 pos = new Vector2(x1, y1);   // Left to Down Curve
            child = Instantiate(FireRope, pos, transform.rotation) as GameObject;
            child.transform.parent = obj.transform;
            child.transform.position = pos;

            x1 = -x1;
            y1 = -y1;
            pos = new Vector2(x1, y1);   // Right to Up Curve
            child = Instantiate(FireRope, pos, transform.rotation) as GameObject;
            child.transform.parent = obj.transform;
            child.transform.position = pos;
        }

        if (type == PanelType.CurveDownLeft_UpRight)
        {
            rot = Quaternion.Euler(0f, 0f, 0f);
        }
        else if (type == PanelType.CurveLeftUp_RightDown)
        {
            rot = Quaternion.Euler(0f, 0f, 90f);
        }
        obj.transform.rotation = rot;

        return obj;
    }

    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------
    GameObject CreateCurvePanel(PanelType type)
    {
        GameObject obj;
        GameObject child;
        float x, y, r, x1, y1;
        Quaternion rot = Quaternion.Euler(0f, 0f, 0f);

        obj = Instantiate(PlainPanel, transform.position, transform.rotation) as GameObject;

        r = panelWidth / 2f;
        x = 0;

        for (int i = 0; i < 16; i++)
        {
            y = Mathf.Sqrt(-(x * x) + (r * r));
            x += panelWidth / 2f / 16f;
            x1 = x - panelWidth / 2f;
            y1 = y - panelHeight / 2f;
            Vector2 pos = new Vector2(x1, y1);   // Left to Down Curve
            child = Instantiate(FireRope, pos, transform.rotation) as GameObject;
            child.transform.parent = obj.transform;
            child.transform.position = pos;

        }

        if (type == PanelType.CurveDownLeft)
        {
            rot = Quaternion.Euler(0f, 0f, 0f);
        }
        else if (type == PanelType.CurveLeftUp)
        {
            rot = Quaternion.Euler(0f, 0f, 270f);
        }
        else if (type == PanelType.CurveRightDown)
        {
            rot = Quaternion.Euler(0f, 0f, 90f);
        }
        else if (type == PanelType.CurveUpRight)
        {
            rot = Quaternion.Euler(0f, 0f, 180f);
        }
        obj.transform.rotation = rot;

        return obj;
    }

    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------
    GameObject CreateLinePanel(PanelType type)
    {
        GameObject obj;
        GameObject child;
        Quaternion rot = Quaternion.Euler(0f, 0f, 0f);

        obj = Instantiate(PlainPanel, transform.position, Quaternion.Euler(0f, 0f, 0f)) as GameObject;
        for (int i = 0; i < 16; i++)
        {
            Vector2 pos = new Vector2((float)i * panelWidth / 16f - panelWidth / 2f, 0f);   // Horizontal Line
            child = Instantiate(FireRope, pos, Quaternion.Euler(0f, 0f, 0f)) as GameObject;
            child.transform.position = pos;
            child.transform.parent = obj.transform;

        }
        if (type == PanelType.StraightHorizontal)
        {
            rot = Quaternion.Euler(0f, 0f, 0f);
        }
        else// Vertical
        {
            rot = Quaternion.Euler(0f, 0f, 90f);
        }
        obj.transform.rotation = rot;

        return obj;
    }

    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------
    GameObject CreateNoPanel()
    {
        GameObject obj;

        obj = Instantiate(NoPanel, transform.position, transform.rotation) as GameObject;
        obj.GetComponent<SpriteRenderer>().enabled = false;
        return obj;
    }


    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------
    // 左下を原点として上方向(Y方向)に指定の大きさ(panelSize)のパネルを並べていく
    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------
    void CreateStage(long stageNum)
    {
        switch (stageNum)
        {
            case 1:     // Stage1
                currentX = 0;
                currentY = 3;
                CurrentFusePanelDirection = FuseDirection.Down;

                // Stage1 [4, 4]
                panelSize = 4;

                stage = new PanelType[]
                {
                    // X 1列目
                    PanelType.CurveDownLeft,    
                    PanelType.CurveLeftUp,
                    PanelType.CurveDownLeft_UpRight,
                    PanelType.CurveRightDown,

                     // X 2列目
                    PanelType.StraightHorizontal,
                    PanelType.CurveLeftUp_RightDown,
                    PanelType.CurveDownLeft_UpRight,
                    PanelType.CurveLeftUp_RightDown,

                     // X 3列目
                    PanelType.CurveLeftUp_RightDown,
                    PanelType.CurveDownLeft_UpRight,
                    PanelType.CurveDownLeft_UpRight,
                    PanelType.CurveRightDown,

                     // X 4列目
                    PanelType.CurveDownLeft,
                    PanelType.StraightHorizontal,
                    PanelType.CurveDownLeft_UpRight,
                    PanelType.Nothing,

                };
                break;

            case 2:     // Stage2
                currentX = 0;
                currentY = 4;
                CurrentFusePanelDirection = FuseDirection.Down;

                // Stage2 [5, 5]
                panelSize = 5;

                stage = new PanelType[]
                {
                    PanelType.CurveDownLeft,
                    PanelType.StraightVertical,
                    PanelType.CurveLeftUp_RightDown,
                    PanelType.CurveDownLeft,
                    PanelType.CurveDownLeft,

                    PanelType.StraightHorizontal,
                    PanelType.CurveLeftUp_RightDown,
                    PanelType.CurveDownLeft,
                    PanelType.CurveDownLeft,
                    PanelType.CurveDownLeft,

                    PanelType.CurveDownLeft,
                    PanelType.CurveDownLeft,
                    PanelType.Nothing,
                    PanelType.CurveDownLeft,
                    PanelType.CurveDownLeft,

                    PanelType.CurveDownLeft,
                    PanelType.CurveDownLeft,
                    PanelType.CurveDownLeft,
                    PanelType.CurveDownLeft,
                    PanelType.Nothing,

                };
                break;

            default:
                break;
        }

        Panel = new PanelMatrix(panelSize);

        int count = 0;
        for (int i = 0; i < Panel.Size; i++)
        {
            for (int j = 0; j < Panel.Size; j++)
            {
                Panel.Matrix[i, j] = stage[count++];

                PanelType panelType = Panel.Matrix[i, j];
                switch (panelType)
                {
                    case PanelType.StraightVertical:
                        Panel.PanelObject[i, j] = CreateLinePanel(panelType);
                        break;

                    case PanelType.StraightHorizontal:
                        Panel.PanelObject[i, j] = CreateLinePanel(panelType);
                        break;

                    case PanelType.CurveLeftUp:
                    case PanelType.CurveUpRight:
                    case PanelType.CurveRightDown:
                    case PanelType.CurveDownLeft:
                        Panel.PanelObject[i, j] = CreateCurvePanel(panelType);
                        break;

                    case PanelType.CurveLeftUp_RightDown:
                    case PanelType.CurveDownLeft_UpRight:
                        Panel.PanelObject[i, j] = CreateDoublePanel(panelType);
                        break;

                    case PanelType.Nothing:
                        Panel.PanelObject[i, j] = CreateNoPanel();
                        NonPanel = Panel.PanelObject[i, j];
                        break;
                }
                Panel.PanelObject[i, j].transform.position = new Vector2(i * panelWidth, j * panelHeight);
            }
        }
    }

    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------
    void Start()
    {

        PanelDirection = MoveDirection.None;
        panelWidth = PlainPanel.GetComponent<SpriteRenderer>().bounds.size.x;  // 0.64 = 64 pixel
        panelHeight = PlainPanel.GetComponent<SpriteRenderer>().bounds.size.y;


        CreateStage(1); // Stage1

    }

    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------
    // Update is called once per frame
    void Update()
    {
        Vector3 clickPosition;
        Vector3 screenToWorldPointClickPosition;
        GameObject hitObject;
        int i = 0;
        int j = 0;
        float delta = (panelHeight + panelWidth) / 2f / 4f;    // 1/60秒ごとのパネル移動距離

        if (PanelDirection != MoveDirection.None)
        {
            moveDistance -= delta;
            if (moveDistance + delta > 0f)
            {
                Vector2 pos;

                switch (PanelDirection)
                {
                    case MoveDirection.Down:
                        pos = MovingPanel.transform.position;
                        pos.y -= delta;
                        MovingPanel.transform.position = pos;
                        break;

                    case MoveDirection.Up:
                        pos = MovingPanel.transform.position;
                        pos.y += delta;
                        MovingPanel.transform.position = pos;
                        break;

                    case MoveDirection.Right:
                        pos = MovingPanel.transform.position;
                        pos.x += delta;
                        MovingPanel.transform.position = pos;
                        break;

                    case MoveDirection.Left:
                        pos = MovingPanel.transform.position;
                        pos.x -= delta;
                        MovingPanel.transform.position = pos;
                        break;
                }
                return;
            }
            else
            {
                // パネルオブジェクトの入れ替え
                GameObject temp_mat;
                temp_mat = Panel.PanelObject[swapX1, swapY1];
                Panel.PanelObject[swapX1, swapY1] = Panel.PanelObject[swapX2, swapY2];
                Panel.PanelObject[swapX2, swapY2] = temp_mat;

                PanelDirection = MoveDirection.None;    // パネル移動中フラグを落とす
            }

        }

        if (Input.GetMouseButtonDown(0))
        {

            // ここでの注意点は座標の引数にVector2を渡すのではなく、Vector3を渡すことである。
            // Vector3でマウスがクリックした位置座標を取得する
            clickPosition = Input.mousePosition;
            // Z軸修正
            clickPosition.z = 10f;
            // スクリーン座標をワールド座標に変換する
            screenToWorldPointClickPosition = Camera.main.ScreenToWorldPoint(clickPosition);

            float x;
            float y;
            for (i = 0; i < Panel.Size; i++)
            {
                for (j = 0; j < Panel.Size; j++)
                {
                    x = Panel.PanelObject[i, j].transform.position.x;
                    y = Panel.PanelObject[i, j].transform.position.y;

                    if ((x - panelWidth / 2 < screenToWorldPointClickPosition.x) && (x + panelWidth / 2 > screenToWorldPointClickPosition.x))
                    {
                        if ((y - panelHeight / 2 < screenToWorldPointClickPosition.y) && (y + panelHeight / 2 > screenToWorldPointClickPosition.y))
                        {
                            // HitしたObjectがあるかどうか
                            hitObject = Panel.PanelObject[i, j];
                            goto FOUND_OBJECT;
                        }
                    }
                }
            }
        }
        return;

        FOUND_OBJECT:
        PanelType temp;

        if (i != 0)
        {
            if (Panel.PanelObject[i - 1, j].name.Substring(0, 2) == "No")   // "NoPanel"
            {
                //Panel.PanelObject[i, j].GetComponent<Rigidbody2D>().AddForce(new Vector2(-4f, 0f), ForceMode2D.Impulse);
                PanelDirection = MoveDirection.Left;
                MovingPanel = Panel.PanelObject[i, j];
                Vector2 pos = Panel.PanelObject[i - 1, j].transform.position;
                pos.x += panelWidth;
                Panel.PanelObject[i - 1, j].transform.position = pos;
                moveDistance = panelWidth;

                temp = Panel.Matrix[i - 1, j];
                Panel.Matrix[i - 1, j] = Panel.Matrix[i, j];
                Panel.Matrix[i, j] = temp;

                swapX1 = i;
                swapX2 = i - 1;
                swapY1 = j;
                swapY2 = j;

                return;
            }
        }
        if (j != 0)
        {
            if (Panel.PanelObject[i, j - 1].name.Substring(0, 2) == "No")   // "NoPanel"
            {
                //Panel.PanelObject[i, j].GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, -4f), ForceMode2D.Impulse);
                PanelDirection = MoveDirection.Down;
                MovingPanel = Panel.PanelObject[i, j];
                Vector2 pos = Panel.PanelObject[i, j - 1].transform.position;
                pos.y += panelHeight;
                Panel.PanelObject[i, j - 1].transform.position = pos;
                moveDistance = panelHeight;

                temp = Panel.Matrix[i, j - 1];
                Panel.Matrix[i, j - 1] = Panel.Matrix[i, j];
                Panel.Matrix[i, j] = temp;

                swapX1 = i;
                swapX2 = i;
                swapY1 = j;
                swapY2 = j - 1;

                return;

            }
        }
        if (i != Panel.Size - 1)
        {
            if (Panel.PanelObject[i + 1, j].name.Substring(0, 2) == "No")   // "NoPanel"
            {
                //Panel.PanelObject[i, j].GetComponent<Rigidbody2D>().AddForce(new Vector2(4f, 0f), ForceMode2D.Impulse);
                PanelDirection = MoveDirection.Right;
                MovingPanel = Panel.PanelObject[i, j];
                Vector2 pos = Panel.PanelObject[i + 1, j].transform.position;
                pos.x -= panelWidth;
                Panel.PanelObject[i + 1, j].transform.position = pos;
                moveDistance = panelWidth;

                temp = Panel.Matrix[i + 1, j];
                Panel.Matrix[i + 1, j] = Panel.Matrix[i, j];
                Panel.Matrix[i, j] = temp;

                swapX1 = i;
                swapX2 = i + 1;
                swapY1 = j;
                swapY2 = j;
                return;

            }
        }
        if (j != Panel.Size - 1)
        {
            if (Panel.PanelObject[i, j + 1].name.Substring(0, 2) == "No")   // "NoPanel"
            {
                //               Panel.PanelObject[i, j].GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, 4f), ForceMode2D.Impulse);
                PanelDirection = MoveDirection.Up;
                MovingPanel = Panel.PanelObject[i, j];
                Vector2 pos = Panel.PanelObject[i, j + 1].transform.position;
                pos.y -= panelHeight;
                Panel.PanelObject[i, j + 1].transform.position = pos;
                moveDistance = panelHeight;

                temp = Panel.Matrix[i, j + 1];
                Panel.Matrix[i, j + 1] = Panel.Matrix[i, j];
                Panel.Matrix[i, j] = temp;

                swapX1 = i;
                swapX2 = i;
                swapY1 = j;
                swapY2 = j + 1;

                return;

            }
        }
        PanelDirection = MoveDirection.None;

        Debug.Log("x:" + i + "y:" + j);

    }

    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------
    //　現在の燃えているパネルのタイプを返却する
    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------
    public PanelType GetCurrentPanelType()
    {
        PanelType currentType = Panel.Matrix[currentX, currentY];

        return currentType;
    }

    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------
    //　現在の燃えているパネルが最終的に燃えていく方向を返却する
    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------
    public FuseDirection GetCurrentFuseDirection()
    {
        return CurrentFusePanelDirection;
    }

    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------
    //　導火線がパネルの中で燃え尽きた時に導火線が繋がっているかどうか判断する
    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------
    public void JudgeFuseConnection()
    {
        PanelType currentType = Panel.Matrix[currentX, currentY];
        FuseDirection nextStartDirection;

        // 移動可能方向
        //    case PanelType.StraightVertical:    // Down, Up  
        //    case PanelType.StraightHorizontal;  // Right, Left
        //    case PanelType.CurveLeftUp:         // Left, Up
        //    case PanelType.CurveUpRight:        // Up, RIght
        //    case PanelType.CurveRightDown:      // Right, Down
        //    case PanelType.CurveDownLeft:       // Down, Left
        //    case PanelType.CurveLeftUp_RightDown:   // Down, Up, RIght, Left
        //    case PanelType.CurveDownLeft_UpRight:   // Down, Up, RIght, Left

        switch (CurrentFusePanelDirection)
        {
            case FuseDirection.Down:
                nextStartDirection = FuseDirection.Up;
                if (currentY > 0)
                {
                    if (Panel.Matrix[currentX, currentY - 1] == PanelType.StraightVertical)
                    {
                        currentY--;
                        CurrentFusePanelDirection = FuseDirection.Down;
                        break;
                    }
                    if (Panel.Matrix[currentX, currentY - 1] == PanelType.CurveLeftUp)
                    {
                        currentY--;
                        CurrentFusePanelDirection = FuseDirection.Left;
                        break;
                    }
                    if (Panel.Matrix[currentX, currentY - 1] == PanelType.CurveUpRight)
                    {
                        currentY--;
                        CurrentFusePanelDirection = FuseDirection.Right;
                        break;
                    }
                    if (Panel.Matrix[currentX, currentY - 1] == PanelType.CurveLeftUp_RightDown)
                    {
                        currentY--;
                        CurrentFusePanelDirection = FuseDirection.Right;
                        break;
                    }
                    if (Panel.Matrix[currentX, currentY - 1] == PanelType.CurveDownLeft_UpRight)
                    {
                        currentY--;
                        CurrentFusePanelDirection = FuseDirection.Left;
                        break;
                    }
                    // gameover
                }
                else
                {
                    // gameover
                }
                break;

            case FuseDirection.Up:
                nextStartDirection = FuseDirection.Down;
                if (currentY < Panel.Size - 1)
                {
                    if (Panel.Matrix[currentX, currentY + 1] == PanelType.StraightVertical)
                    {
                        currentY++;
                        CurrentFusePanelDirection = FuseDirection.Down;
                        break;
                    }
                    if (Panel.Matrix[currentX, currentY + 1] == PanelType.CurveRightDown)
                    {
                        currentY++;
                        CurrentFusePanelDirection = FuseDirection.Right;
                        break;
                    }
                    if (Panel.Matrix[currentX, currentY + 1] == PanelType.CurveDownLeft)
                    {
                        currentY++;
                        CurrentFusePanelDirection = FuseDirection.Left;
                        break;
                    }
                    if (Panel.Matrix[currentX, currentY + 1] == PanelType.CurveLeftUp_RightDown)
                    {
                        currentY++;
                        CurrentFusePanelDirection = FuseDirection.Right;
                        break;
                    }
                    if (Panel.Matrix[currentX, currentY + 1] == PanelType.CurveDownLeft_UpRight)
                    {
                        currentY++;
                        CurrentFusePanelDirection = FuseDirection.Left;
                        break;
                    }
                    // gameover
                }
                else
                {
                    // gameover
                }
                break;

            case FuseDirection.Left:
                nextStartDirection = FuseDirection.Right;
                if (currentX > 0)
                { 
                    if (Panel.Matrix[currentX - 1, currentY] == PanelType.StraightHorizontal)
                    {
                        currentX--;
                        CurrentFusePanelDirection = FuseDirection.Left;
                        break;
                    }
                    if (Panel.Matrix[currentX - 1, currentY] == PanelType.CurveUpRight)
                    {
                        currentX--;
                        CurrentFusePanelDirection = FuseDirection.Up;
                        break;
                    }
                    if (Panel.Matrix[currentX - 1, currentY] == PanelType.CurveRightDown)
                    {
                        currentX--;
                        CurrentFusePanelDirection = FuseDirection.Down;
                        break;
                    }
                    if (Panel.Matrix[currentX - 1, currentY] == PanelType.CurveLeftUp_RightDown)
                    {
                        currentX--;
                        CurrentFusePanelDirection = FuseDirection.Down;
                        break;
                    }
                    if (Panel.Matrix[currentX - 1, currentY] == PanelType.CurveDownLeft_UpRight)
                    {
                        currentX--;
                        CurrentFusePanelDirection = FuseDirection.Up;
                        break;
                    }
                    // gameover
                }
                else
                {
                    // gameover
                }
                break;

            case FuseDirection.Right:
                if ((currentX == Panel.Size - 1) && (currentY == Panel.Size - 1))
                {
                    // game clear!!!!!!!!!
                    // ロケットが右下にあると仮定したら
                }

                nextStartDirection = FuseDirection.Left;
                if (currentX < Panel.Size - 1)
                {
                    if (Panel.Matrix[currentX + 1, currentY] == PanelType.StraightHorizontal)
                    {
                        currentX++;
                        CurrentFusePanelDirection = FuseDirection.Right;
                        break;
                    }
                    if (Panel.Matrix[currentX + 1, currentY] == PanelType.CurveLeftUp)
                    {
                        currentX++;
                        CurrentFusePanelDirection = FuseDirection.Up;
                        break;
                    }
                    if (Panel.Matrix[currentX + 1, currentY] == PanelType.CurveDownLeft)
                    {
                        currentX++;
                        CurrentFusePanelDirection = FuseDirection.Down;
                        break;
                    }
                    if (Panel.Matrix[currentX + 1, currentY] == PanelType.CurveLeftUp_RightDown)
                    {
                        currentX++;
                        CurrentFusePanelDirection = FuseDirection.Up;
                        break;
                    }
                    if (Panel.Matrix[currentX + 1, currentY] == PanelType.CurveDownLeft_UpRight)
                    {
                        currentX++;
                        CurrentFusePanelDirection = FuseDirection.Down;
                        break;
                    }
                    // gameover
                }
                else
                {
                    // gameover
                }

                break;
        }
        // StartBurning(nextStartDirection, Panel.Matrix[currentX, currentY]);

    }

    // StartBurning(Direction, PanelType);
    // これをシステムからパネルのプレハブ(PlainPanel)のクラスメソッドに実装する。
    // パネルオブジェクトは自分のパネルタイプと燃えていく
    // システムから呼ばれる。
    // パネルオブジェクトはアニメーションで3秒くらいで自分のタイミングで燃えるアニメーションを表示して、
    // パネルの燃え終わりのタイミングをパネルオブジェクトから通知する。
    // JudgeFuseConnection()
}
