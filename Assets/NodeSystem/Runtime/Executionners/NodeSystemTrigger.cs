﻿using System;
using NodeSystem.Runtime.References;
using UnityEngine;

namespace NodeSystem.Runtime.Executionners
{
    public class NodeSystemTrigger : MonoBehaviour, INodeSystemExecutioner
    {
        [SerializeField] private GameObject _gameObjectToTrigger;
        [SerializeField] private NodeSystemAsset _graph;
        [SerializeField] private bool _canBeExecutedMultipleTimes;
        
        private NodeSystemAsset _graphInstance;

        private string m_currentExecNodeId;
        private NodeSystemNode _nodeToPlay;

        private void Start()
        {
            _graphInstance = NodeSystemBank.GetGraphInstance(_graph);
            if (_gameObjectToTrigger == null) _gameObjectToTrigger = gameObject;
            _nodeToPlay = _graphInstance.GetNodeToPlay(_gameObjectToTrigger);
            if (_nodeToPlay == null) return;
            m_currentExecNodeId = _nodeToPlay.id;
        }

        public void Trigger()
        {
            TickProcess();
            // nodeToPlay.OnProcess(new ExecInfo(graphInstance, this));
        }
        
        public NodeSystemNode GetCurrentNode()
        {
            return _graphInstance == null ? null : _graphInstance.GetNode(m_currentExecNodeId);
        }

        public void TickProcess()
        {
            if (_graphInstance == null || m_currentExecNodeId == "") return;
            ProcessInfo processInfo = GetCurrentNode().OnProcess(new ExecInfo(_graphInstance, this));
            Debug.Log("Ticking!");
            switch (processInfo.FlowType)
            {
                case ProcessInfo.ExecutionFlowType.ExecuteNext:
                    m_currentExecNodeId = processInfo.NextNodeId;
                    TickProcess();
                    break;
                case ProcessInfo.ExecutionFlowType.Wait:
                    m_currentExecNodeId = processInfo.NextNodeId;
                    break;
                case ProcessInfo.ExecutionFlowType.EndExecution:
                    Debug.Log("Stopped execution at " + m_currentExecNodeId);

                    // Allows for rerunning the script
                    m_currentExecNodeId = _canBeExecutedMultipleTimes ? _nodeToPlay.id : "";
;                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void ExecuteFromNode(string nodeId)
        {
            throw new NotImplementedException();
        }

        public MonoBehaviour GetObject()
        {
            return this;
        }
    }
}