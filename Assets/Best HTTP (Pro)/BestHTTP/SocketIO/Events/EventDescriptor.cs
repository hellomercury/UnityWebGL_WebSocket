/*


CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

#if !BESTHTTP_DISABLE_SOCKETIO

using System;
using System.Collections.Generic;

namespace BestHTTP.SocketIO.Events
{
    public delegate void SocketIOCallback(Socket socket, Packet packet, params object[] args);
    public delegate void SocketIOAckCallback(Socket socket, Packet packet, params object[] args);

    /// <summary>
    /// A class to describe an event, and its metadatas.
    /// </summary>
    internal sealed class EventDescriptor
    {
        #region Public Properties

        /// <summary>
        /// List of callback delegates.
        /// </summary>
        public List<SocketIOCallback> Callbacks { get; private set; }

        /// <summary>
        /// If this property is true, callbacks are removed automatically after the event dispatch.
        /// </summary>
        public bool OnlyOnce { get; private set; }

        /// <summary>
        /// If this property is true, the dispatching packet's Payload will be decoded using the Manager's Encoder.
        /// </summary>
        public bool AutoDecodePayload { get; private set; }

        #endregion

        /// <summary>
        /// Cache an array on a hot-path.
        /// </summary>
        private SocketIOCallback[] CallbackArray;

        /// <summary>
        /// Constructor to create an EventDescriptor instance and set the meta-datas.
        /// </summary>
        public EventDescriptor(bool onlyOnce, bool autoDecodePayload, SocketIOCallback callback)
        {
            this.OnlyOnce = onlyOnce;
            this.AutoDecodePayload = autoDecodePayload;
            this.Callbacks = new List<SocketIOCallback>(1);

            if (callback != null)
                Callbacks.Add(callback);
        }

        /// <summary>
        /// Will call the callback delegates with the given parameters and remove the callbacks if this descriptor marked with a true OnlyOnce property.
        /// </summary>
        public void Call(Socket socket, Packet packet, params object[] args)
        {
            if (CallbackArray == null || CallbackArray.Length < Callbacks.Count)
                Array.Resize(ref CallbackArray, Callbacks.Count);

            // Copy the callback delegetes to an array, becouse in one of the callbacks we can modify the list(by calling On/Once/Off in an event handler)
            // This way we can prevent some strange bug
            Callbacks.CopyTo(CallbackArray);

            // Go through the delegates and call them
            for (int i = 0; i < CallbackArray.Length; ++i)
            {
                try
                {
                    // Call the delegate.
                    CallbackArray[i](socket, packet, args);
                }
                catch (Exception ex)
                {
                    (socket as ISocket).EmitError(SocketIOErrors.User, ex.Message + " " + ex.StackTrace);

                    HTTPManager.Logger.Exception("EventDescriptor", "Call", ex);
                }

                // If these callbacks has to be called only once, remove them from the main list
                if (this.OnlyOnce)
                    Callbacks.Remove(CallbackArray[i]);

                // Don't keep any reference avoiding memory leaks
                CallbackArray[i] = null;
            }
        }
    }
}

#endif