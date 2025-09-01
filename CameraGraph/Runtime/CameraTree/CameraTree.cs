using System;
using System.Collections.Generic;
using Cinemachine;
using CMCameraFramework;
using UnityEngine;

[CreateAssetMenu(menuName = "相机节点图")]
public class CameraTree : ScriptableObject
{
    public RootNode rootNode;

    public List<CameraNode> nodes = new List<CameraNode>();

    public Action<int> DoCameraAction;

    [HideInInspector] public StateDrivenCamera StatCamera;
    private CinemachineBlendDefinition m_blend;
    public CinemachineBlendDefinition FinishEventBlend
    {
        get
        {
            return m_blend;
        }
    }
    private bool m_IsFinishChaseBack;
    public bool IsFinishChaseBack
    {
        get
        {
            return m_IsFinishChaseBack;
        }
    }

    public bool IsChangeZoom;
    public float ZoomTime;
    public float ZoomValue;

    
    private CameraAction _curCameraAciton;

    public void Init(StateDrivenCamera statCamera)
    {
        this.StatCamera = statCamera;
        m_blend = new CinemachineBlendDefinition();
        m_blend.m_Style = CinemachineBlendDefinition.Style.Cut;
    }

    public void Release()
    {
        this.StatCamera = null;
    }

    public CameraAction GetStartNode(int id)
    {
        foreach (var child in rootNode.Children)
        {
            if (child is StartCameraNode startNode && startNode.Id == id)
            {
                return startNode;
            }
        }

        return null;
    }


    /// <summary>
    /// 设置相机事件
    /// </summary>
    /// <param name="事件ID"></param>
    /// <returns></returns>
    public bool SetCameraAction(int id)
    {
        if (StatCamera == null) return false;
        var node = GetStartNode(id);
        if (node == null) return false;

        if (_curCameraAciton != null)
            _curCameraAciton.LinkRelease();
        _curCameraAciton = node;

        return node.DoAction();
    }

    public void SetFinishBlend(CinemachineBlendDefinition blend, bool isChaseBack,bool isChangeZoom,float zoomTime, float zoomValue)
    {
        m_blend = blend;
        m_IsFinishChaseBack = isChaseBack;
        IsChangeZoom = isChangeZoom;
        ZoomValue = zoomValue;
        ZoomTime = zoomTime;
    }

    public void Reset()
    {
        m_IsFinishChaseBack = false;
        IsChangeZoom = false;
        ZoomValue = 0;
        ZoomTime = 0;
    }
    
} 