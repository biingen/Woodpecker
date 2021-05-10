using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;           //support SafeFileHandle
using System.Runtime.InteropServices;      //support DIIImport
using System.Reflection;                                //support BindingFlags
using Woodpecker;

namespace ModuleLayer
{
    //public enum eStates { Operating, Measuring };

    public class StateEventArgs : EventArgs
    {
        public int state_id { get; set; }
        public StateEventArgs(int sid)
        {
            state_id = sid;
        }
    }

    public class State
    {
        public string strState { get; set; }
        public State(string sState)
        {
            strState = sState;
        }
        protected virtual void ChangeState(object sender, StateEventArgs eventArgs) { }

        public override string ToString()
        {
            return strState;
        }

    }

    public delegate void StateAction(object sender, StateEventArgs eventArgs);
    //public event StateAction sa;
    public class Transition
    {
        public State OperatingState { get; set; }
        public State MeasuringState { get; set; }

        public StateEventArgs eventArgs;

        public Transition(State prev, StateEventArgs eventArgs, State last, StateAction sa)
        {
            //if (prev == State.ToString())

        }
        /*
        public override int GetHashCode()
        {
            return base.GetHashCode(prev, eventArgs);
        }

        public static int GetHashCode(State state, StateEventArgs eventArgs)
        {

        }*/
    }

    public class Transitions : Dictionary<int, Transition>
    {
        public void Add(Transition transition)
        {
            // The Add method throws an exception if the new key is already in the dictionary.
            try
            {
                base.Add(transition.GetHashCode(), transition);
            }
            catch (ArgumentException)
            {
                throw new ArgumentException("A transition with the key (Initials state " +
                        //transition.initialState + ", Event " +
                        transition.eventArgs + ") already exists.");
            }
        }
        /*
        public Transition this[State state, StateEventArgs sevent]
        {
            get
            {
                try
                {
                    //return this[Transition.GetHashCode(state, sevent)];
                }
                catch (KeyNotFoundException)
                {
                    throw new KeyNotFoundException("The given transition was not found.");
                }
            }
            set
            {
                //this[Transition.GetHashCode(state, sevent)] = value;
            }
        }
        
        public bool Remove(State state, StateEventArgs sevent)
        {
            return base.Remove(Transition.GetHashCode(state, sevent));
        }*/
    }

    public interface IStateManager : IDisposable
    {
        void ChangeState(object sender, StateEventArgs eventArgs);
        bool CheckState(object sender, StateEventArgs eventArgs);
    }

    public abstract class StatesManager : IStateManager
    {
        public virtual void ChangeState(object sender, StateEventArgs eventArgs)
        {

        }
        
        public virtual bool CheckState(object sender, StateEventArgs eventArgs)
        {
            bool chk = false;
            return chk;
        }

        public virtual void Dispose()
        {
            //virtual Dispose
        }
    }
}
