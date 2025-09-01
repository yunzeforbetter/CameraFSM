using System;

namespace CMCameraFramework
{
    public class FSMEvent
    {
//        public FSMEvent ParentEvent;
//        private FSMEvent argEvent1;

        public FSMEvent()
        {

        }

        public FSMEvent(int id)
        {
            Id = id;
        }

//        public FSMEvent(int id, FSMEvent argEvent)
//        {
//            Id = id;
//            ParentEvent = argEvent;
//        }


//        public int LayerDepth
//        {
//            get
//            {
//                if (ParentEvent == null)
//                    return 0;
//                else
//                    return ParentEvent.LayerDepth + 1;
//            }
//        }

        public int Id
        {
            get;
            set;
        }

//        public virtual FSMEvent NextEvent
//        {
//            get
//            {
//                return null;
//            }
//        }

//        public virtual bool KeepForNextState(object state)
//        {
//            return false;
//        }

        public virtual void Reset()
        {

        }
    }
}