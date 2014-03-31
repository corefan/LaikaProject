﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reflection;

namespace Laika.MessageHandler
{
    /// <summary>
    /// 메시지타입에 대한 메소드 실행 클래스
    /// </summary>
    /// <typeparam name="MA1">메소드에 넘겨줄 arg1</typeparam>
    /// <typeparam name="MR">메소드 실행 후 return 되는 타입</typeparam>
    public class MessageInvokeHandler<MA1, MR>
    {
        /// <summary>
        /// 메소드를 뽑아내어 Dictionary에 적재함.
        /// </summary>
        /// <typeparam name="LC">
        /// 로직 메소드가 정의된 클래스. class이여야 하며, newable이어야 합니다.
        /// 로직 클래스와 메소드는 public이어야 합니다.
        /// instace 메소드, static 메소드 모두 지원합니다.
        /// 메소드를 구현할 때에는 Thread Safety 하게 작성하여야 합니다.
        /// </typeparam>
        public void RegisterHandler<LC>()
            where LC : class, new()
        {
            LC instance = new LC();
            Type type = instance.GetType();
            MethodInfo[] methods = type.GetMethods();
            
            foreach (MethodInfo m in methods)
            {
                object[] attrs = m.GetCustomAttributes(false);

                MessageHandlerAttribute h = null;
                foreach (object o in attrs)
                {
                     h = o as MessageHandlerAttribute;
                    if (h != null)
                        break;
                }

                if (h == null)
                    continue;

                if (_messageMap.ContainsKey(h.TypeNo) == true)
                    throw new Exception("Already contains key.");

                if (m.IsPublic == false)
                    throw new Exception("Method must be public.");

                Func<MA1, MR> executeableMethod = null;
                if (m.IsStatic == true)
                {
                    executeableMethod = GetDelegate(m);
                }
                else
                { 
                    executeableMethod = GetDelegate<LC>(m, instance);
                }
                Add(h.TypeNo, executeableMethod);
            }
        }

        private Func<MA1, MR> GetDelegate(MethodInfo m)
        {
            return (Func<MA1, MR>)Delegate.CreateDelegate(typeof(Func<MA1, MR>), m);
        }

        private Func<MA1, MR> GetDelegate<LC>(MethodInfo m, LC instance)
        {
            return (Func<MA1, MR>)Delegate.CreateDelegate(typeof(Func<MA1, MR>), instance, m);
        }

        private void Add(int type, Func<MA1, MR> func)
        {
            if (_messageMap.ContainsKey(type) == true)
                throw new ArgumentException("Aleady contains key.");

            _messageMap.Add(type, func);
        }
        /// <summary>
        /// 타입에 대한 메소드를 Invoke 합니다.
        /// </summary>
        /// <param name="type">메시지 타입</param>
        /// <param name="arg1">메소드에 넘겨줄 arg1</param>
        /// <returns>메소드 실행 후 return 되는 타입</returns>
        public MR InvokeMethod(int type, MA1 arg)
        {
            if (_messageMap.ContainsKey(type) == false)
                throw new KeyNotFoundException();

            return _messageMap[type](arg);
        }

        /// <summary>
        /// 타입에 대한 메소드를 비동기 방식으로 Invoke 합니다.
        /// </summary>
        /// <param name="type">메시지 타입</param>
        /// <param name="arg1">메소드에 넘겨줄 arg1</param>
        /// <returns>메소드 실행 후 return 되는 타입</returns>
        public Task<MR> InvokeMethodAsync(int type, MA1 arg)
        {
            if (_messageMap.ContainsKey(type) == false)
                throw new KeyNotFoundException();

            Task<MR> task = new Task<MR>(() => { return _messageMap[type](arg); });
            task.Start();
            return task;
        }

        private Dictionary<int, Func<MA1, MR>> _messageMap = new Dictionary<int, Func<MA1, MR>>();
    }
}
