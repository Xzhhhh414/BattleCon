﻿using System.Reflection;
using ParadoxNotion;
using ParadoxNotion.Design;
using ParadoxNotion.Serialization;
using UnityEngine;

namespace FlowCanvas.Nodes
{
    ///<summary>Wraps a ConstructorInfo into a FlowGraph node</summary>
    public class ReflectedConstructorNodeWrapper : ReflectedMethodBaseNodeWrapper
    {

        [SerializeField]
        private SerializedConstructorInfo _constructor;

        private BaseReflectedConstructorNode reflectedConstructorNode { get; set; }
        private ConstructorInfo constructor => _constructor;
        protected override ISerializedMethodBaseInfo serializedMethodBase => _constructor;

        public override string name {
            get
            {
                if ( constructor != null ) { return string.Format("New {0} ()", constructor.DeclaringType.FriendlyName()); }
                if ( _constructor != null ) { return _constructor.AsString().FormatError(); }
                return "NOT SET";
            }
        }

#if UNITY_EDITOR
        public override string description {
            get { return constructor != null ? XMLDocs.GetMemberSummary(constructor.DeclaringType) : "Missing Constructor"; }
        }
#endif

        ///<summary>Set a new ConstructorInfo to be used by ReflectedConstructorNode</summary>
        public override void SetMethodBase(MethodBase newMethod, object instance = null) {
            if ( newMethod is ConstructorInfo ) {
                SetConstructor((ConstructorInfo)newMethod);
            }
        }

        ///<summary>Set a new ConstructorInfo to be used by ReflectedConstructorNode</summary>
        void SetConstructor(ConstructorInfo newConstructor) {
            _constructor = new SerializedConstructorInfo(newConstructor);
            GatherPorts();

            base.SetDefaultParameterValues(newConstructor);
        }

        protected override void RegisterPorts() {
            if ( constructor == null ) {
                return;
            }

            var options = new ReflectedMethodRegistrationOptions();
            options.callable = callable;
            options.exposeParams = exposeParams;
            options.exposedParamsCount = exposedParamsCount;

            reflectedConstructorNode = BaseReflectedConstructorNode.GetConstructorNode(constructor, options);
            if ( reflectedConstructorNode != null ) {
                reflectedConstructorNode.RegisterPorts(this, options);
            }
        }
    }
}