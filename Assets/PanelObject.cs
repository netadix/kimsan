using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelObject : MonoBehaviour {

    public Sprite burntSprite;
    public GameObject FireCracker;

    public bool SpeedUpFlag { get; set; }

    private GameObject currentPanelObject;
    private bool FuseBurning;
    private int dummyCount;
    public bool StartFlag { get; set; }

    private int startFuse;
    private int endFuse;
    private int currentFusePosition;
    private int speedUpCount;

    //---------------------------------------------------------
    //---------------------------------------------------------
    //---------------------------------------------------------
    //---------------------------------------------------------
    // Use this for initialization
    void Start () {
        SetFuseBurningCompletion(false);
        dummyCount = 0;
        StartFlag = false;
        speedUpCount = 10;
    }

    //---------------------------------------------------------
    //---------------------------------------------------------
    //---------------------------------------------------------
    //---------------------------------------------------------
    // Update is called once per frame
    void Update () {
        if (!StartFlag) return;

        if (SpeedUpFlag == true)
        {
            speedUpCount = 1;
        }
        else
        {
            speedUpCount = 10;
        }

        if ((dummyCount % speedUpCount) == 0)
        {
            if (startFuse < endFuse)
            {
                currentFusePosition++;
                if (currentFusePosition > endFuse)
                {
                    SetFuseBurningCompletion(true); // 燃え終わった
                    dummyCount = 0;
                    startFuse = endFuse = 0;
                    currentFusePosition--;
                }
            }
            else if (startFuse > endFuse)
            {
                currentFusePosition--;
                if (currentFusePosition < endFuse)
                {
                    SetFuseBurningCompletion(true); // 燃え終わった
                    dummyCount = 0;
                    startFuse = endFuse = 0;
                    currentFusePosition++;
                }
            }
            else
            {
                // Error
            }
            if (currentPanelObject != null)
            {
                if (currentPanelObject.transform.childCount >= 1)
                {
                    // 燃えた導火線にスプライト差し替え
                    currentPanelObject.transform.GetChild(currentFusePosition).GetComponent<SpriteRenderer>().sprite = burntSprite;
                    FireCracker.transform.parent = currentPanelObject.transform;
                    FireCracker.transform.position = currentPanelObject.transform.GetChild(currentFusePosition).position;
                    currentPanelObject.transform.GetChild(currentFusePosition).transform.position += new Vector3(Random.value * 0.010f - 0.005f, Random.value * 0.010f - 0.005f, 0f);
                }

            }
        }

        dummyCount++;
    }


    //---------------------------------------------------------
    //---------------------------------------------------------
    // パネルが燃え終わったらflagをtrueにしてこの関数をコールする
    //---------------------------------------------------------
    //---------------------------------------------------------
    void SetFuseBurningCompletion(bool flag)
    {
        FuseBurning = flag;
    }

    //---------------------------------------------------------
    //---------------------------------------------------------
    // この関数は外部のクラスから呼ばれる
    //---------------------------------------------------------
    //---------------------------------------------------------
    public bool GetFuseBurningCompletion()
    {
        return FuseBurning;
    }

    //---------------------------------------------------------
    //---------------------------------------------------------
    // この関数は外部のクラスから呼ばれる
    //---------------------------------------------------------
    //---------------------------------------------------------
    public void RemoveFireCrackerFromParent()
    {
        FireCracker.transform.parent = null;
        FireCracker.transform.rotation = Quaternion.Euler(0f, 0f, 0f);  // ステージクリア時のパネルの回転をリセット
    }


    //---------------------------------------------------------
    //---------------------------------------------------------
    // この関数は外部のクラスから呼ばれる
    // obj : パネルのゲームオブジェクト
    // direction : 導火線のスタート位置
    // type : 導火線のタイプ
    //---------------------------------------------------------
    //---------------------------------------------------------
    public void StartBurning(GameObject obj, FuseDirection direction, PanelType type)
    {
        SetFuseBurningCompletion(false);
        dummyCount = 0;
        currentPanelObject = obj;
        // StartFlag = true;

        switch (type)
        {
            case PanelType.Nothing:                                // 何もない（唯一のパネル)
            case PanelType.Disabled:                               // もはや一回通ったので通れない
                // Nothing to do. (Bug?)
                break;

            case PanelType.StraightVertical:
                switch (direction)
                {
                    case FuseDirection.Down:
                        startFuse = 0;
                        endFuse = 15;
                        break;

                    case FuseDirection.Up:
                        startFuse = 15;
                        endFuse = 0;
                        break;

                    case FuseDirection.Left:
                    case FuseDirection.Right:
                        // Error
                        startFuse = 0;
                        endFuse = 0;
                        break;

                }
                break;

            case PanelType.StraightHorizontal:
                switch (direction)
                {
                    case FuseDirection.Down:
                    case FuseDirection.Up:
                        // Error
                        startFuse = 0;
                        endFuse = 0;
                        break;

                    case FuseDirection.Left:
                        startFuse = 0;
                        endFuse = 15;
                        break;

                    case FuseDirection.Right:
                        startFuse = 15;
                        endFuse = 0;
                        break;

                }
                break;

            case PanelType.CurveLeftUp:
                switch (direction)
                {
                    case FuseDirection.Down:
                    case FuseDirection.Right:
                        // Error
                        startFuse = 0;
                        endFuse = 0;
                        break;

                    case FuseDirection.Up:
                        startFuse = 15;
                        endFuse = 0;
                        break;

                    case FuseDirection.Left:
                        startFuse = 0;
                        endFuse = 15;
                        break;
                }
                break;

            case PanelType.CurveUpRight:
                switch (direction)
                {
                    case FuseDirection.Down:
                    case FuseDirection.Left:
                        // Error
                        startFuse = 0;
                        endFuse = 0;
                        break;

                    case FuseDirection.Up:
                        startFuse = 0;
                        endFuse = 15;
                        break;

                    case FuseDirection.Right:
                        startFuse = 15;
                        endFuse = 0;
                        break;

                }
                break;

            case PanelType.CurveRightDown:
                switch (direction)
                {
                    case FuseDirection.Down:
                        startFuse = 15;
                        endFuse = 0;
                        break;

                    case FuseDirection.Up:
                    case FuseDirection.Left:
                        // Error
                        startFuse = 0;
                        endFuse = 0;
                        break;


                    case FuseDirection.Right:
                        startFuse = 0;
                        endFuse = 15;
                        break;

                }
                break;

            case PanelType.CurveDownLeft:
                switch (direction)
                {
                    case FuseDirection.Down:
                        startFuse = 0;
                        endFuse = 15;
                        break;

                    case FuseDirection.Up:
                    case FuseDirection.Right:
                        // Error
                        startFuse = 0;
                        endFuse = 0;
                        break;

                    case FuseDirection.Left:
                        startFuse = 15;
                        endFuse = 0;
                        break;
                }
                break;

            case PanelType.CurveLeftUp_RightDown:
                switch (direction)
                {
                    case FuseDirection.Down:
                        startFuse = 15;
                        endFuse = 0;
                        break;

                    case FuseDirection.Up:
                        startFuse = 31;
                        endFuse = 16;
                        break;

                    case FuseDirection.Left:
                        startFuse = 16;
                        endFuse = 31;
                        break;

                    case FuseDirection.Right:
                        startFuse = 0;
                        endFuse = 15;
                        break;

                }
                break;

            case PanelType.CurveDownLeft_UpRight:
                switch (direction)
                {
                    case FuseDirection.Down:
                        startFuse = 0;
                        endFuse = 15;
                        break;

                    case FuseDirection.Up:
                        startFuse = 16;
                        endFuse = 31;
                        break;

                    case FuseDirection.Left:
                        startFuse = 15;
                        endFuse = 0;
                        break;

                    case FuseDirection.Right:
                        startFuse = 31;
                        endFuse = 16;
                        break;

                }
                break;

            case PanelType.CurveLeftUp_RightDown_LeftUpOnlyEnabled:    // 一回右下通ったので左上のみ通れる
                switch (direction)
                {
                    case FuseDirection.Down:
                    case FuseDirection.Right:
                        // Error
                        startFuse = 0;
                        endFuse = 0;
                        break;

                    case FuseDirection.Up:
                        startFuse = 31;
                        endFuse = 16;
                        break;

                    case FuseDirection.Left:
                        startFuse = 16;
                        endFuse = 31;
                        break;


                }
                break;

            case PanelType.CurveLeftUp_RightDown_RightDownOnlyEnabled: // 一回左上通ったので右下のみ通れる
                switch (direction)
                {
                    case FuseDirection.Down:
                        startFuse = 15;
                        endFuse = 0;
                        break;

                    case FuseDirection.Up:
                    case FuseDirection.Left:
                        // Error
                        startFuse = 0;
                        endFuse = 0;
                        break;

                    case FuseDirection.Right:
                        startFuse = 0;
                        endFuse = 15;
                        break;

                }
                break;

            case PanelType.CurveDownLeft_UpRight_DownLeftOnlyEnabled:  // 一回右上通ったので左下のみ通れる
                switch (direction)
                {
                    case FuseDirection.Down:
                        startFuse = 0;
                        endFuse = 15;
                        break;

                    case FuseDirection.Up:
                    case FuseDirection.Right:
                        // Error
                        startFuse = 0;
                        endFuse = 0;
                        break;

                    case FuseDirection.Left:
                        startFuse = 15;
                        endFuse = 0;
                        break;
                }
                break;

            case PanelType.CurveDownLeft_UpRight_UpRightOnlyEnabled:   // 一回左下通ったので右上のみ通れる
                switch (direction)
                {
                    case FuseDirection.Down:
                    case FuseDirection.Left:
                        // Error
                        startFuse = 0;
                        endFuse = 0;
                        break;

                    case FuseDirection.Up:
                        startFuse = 16;
                        endFuse = 31;
                        break;

                    case FuseDirection.Right:
                        startFuse = 31;
                        endFuse = 16;
                        break;

                }
                break;

        }
        currentFusePosition = startFuse;
        if (currentPanelObject.transform.childCount >= 1)
        {
            currentPanelObject.transform.GetChild(currentFusePosition).GetComponent<SpriteRenderer>().sprite = burntSprite;
            FireCracker.transform.position = currentPanelObject.transform.GetChild(currentFusePosition).position;
        }

    }

}
