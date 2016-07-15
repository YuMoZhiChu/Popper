/**
* ======== All the code changes must be recorded below. ========
* 
* ==============================================
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(ThirdPlayerControl))]
public class InputController : MonoBehaviour
{

    // Add public variables here:
    static public InputController instance = null;
    public enum ActionType
    {
        MoveJump,
        MoveHorizontal,
        MoveVertical,

        FireNormal
    }

    // Add private members here:
    private ActionType actionType;
    private List<string> actions = new List<string>();
    private ThirdPlayerControl TPC;

    // Add member functions here:
    #region 数组取出和存入
    string PopAnAction()
    {
        if (actions.Count > 0)
        {
            string action = actions[0];
            actions.RemoveAt(0);
            return action;
        }
        else
        {
            return string.Empty;
        }
    }
    public void PushAnAction(string action)
    {
        actions.Add(action);
    }
    #endregion

    #region action的执行
    string BeAction()
    {
        string action = PopAnAction();
        if (action == string.Empty)
            return string.Empty;
        string[] actParts = action.Split(':');
        TPC.SendMessage(actParts[0], actParts[1]);

        return actParts[0];
    }
    #endregion

    // Use this for initialization
    void Awake()
    {
        if (instance == null)
            instance = this;
        TPC = GetComponent<ThirdPlayerControl>();
    }

    void Start()
    {
        


    }

    // Update is called once per frame
    void Update()
    {

        BeAction();

    }
}
