using System;
using System.Collections;
using System.Collections.Generic;
using Grpc.Core;
using Google.Protobuf;
using UnityEngine;
using System.Linq;

namespace Dongpn.ProtoBuf
{
    public abstract class BaseController
    {
        protected ClientBase client;

        protected void Send<T>(string methodName, IMessage request, DelegateCallBack<T> callback) where T : IMessage, new()
        {
            if (client == null)
                return;

            ThreadController.Instance.StartCoroutine(IESend<T>(methodName,request,callback));
        }

        protected void Streaming<T>(string methodName, IMessage request, DelegateCallBackList<T> callback) where T : IMessage, new()
        {
            if (client == null)
                return;

            ThreadController.Instance.StartCoroutine(IEStreaming<T>(methodName, request, callback));
        }


        protected void Upload<T, R>(string methodName, T message, DelegateCallBack<R> callback) where T : IMessage, new()
            where R : IMessage, new()
        {
            if (client == null)
            {
                return;
            }

            ThreadController.Instance.StartCoroutine(IEUpload<T,R>(methodName, message, callback));
        }

        private IEnumerator IESend<T>(string methodName, IMessage request, DelegateCallBack<T> callback) where T : IMessage, new()
        {
            var call = (Grpc.Core.AsyncUnaryCall<T>)client.GetType().GetMethods()
                        .Where(w => w.Name == methodName)
                        .FirstOrDefault(f => f.GetParameters().Length == 2)
                        .Invoke(client, new object[] { request, AppManager.Instance.Options });

            var task = call.ResponseAsync;
            yield return new WaitUntil(() => task.IsCompleted);

            if(!task.IsFaulted && !task.IsCanceled)
            {
                var status = call.GetStatus();
                if (status.StatusCode == StatusCode.OK)
                {
                    callback(call.GetStatus(), task.Result);
                }
                else
                {
                    callback(call.GetStatus(), new T());
                }
            }
            else
            {
                callback(call.GetStatus(), new T());
            }

            call.Dispose();
        }

        private IEnumerator IEStreaming<T> (string methodName, IMessage request, DelegateCallBackList<T> callback) where T: IMessage, new()
        {
            var call = (Grpc.Core.AsyncServerStreamingCall<T>)client.GetType().GetMethods()
                       .Where(w => w.Name == methodName)
                       .FirstOrDefault(f => f.GetParameters().Length == 2)
                       .Invoke(client, new object[] { request, AppManager.Instance.Options });

            bool operation = true;
            List<T> listItem = new List<T>();
            System.Threading.CancellationToken cancelToken;
            try
            {
                while (operation)
                {
                    var task = call.ResponseStream.MoveNext(cancelToken);
                    task.Wait();
                    if (task.Result)
                    {
                        listItem.Add(call.ResponseStream.Current);
                    }
                    else
                        operation = false;
                }
            }
            catch (AggregateException err)
            {
                Debug.LogError(err);
            }

            yield return cancelToken;
            var status = call.GetStatus();
            if (status.StatusCode == StatusCode.OK)
            {
                if (listItem.Count == 0 && !cancelToken.IsCancellationRequested)
                    callback(call.GetStatus(), listItem);
                else
                    callback(call.GetStatus(), listItem);
            }
            else
            {
                callback(call.GetStatus(), new List<T>());
            }

            call.Dispose();
        }


        private IEnumerator IEUpload<T,R>(string methodName, T requestStream, DelegateCallBack<R> callback) where T : IMessage,new()
            where R : IMessage, new()
        {

            var call = (Grpc.Core.AsyncClientStreamingCall<T,R>)client.GetType().GetMethods()
                        .Where(w => w.Name == methodName)
                        .FirstOrDefault(f => f.GetParameters().Length == 1)
                        .Invoke(client, new object[] { AppManager.Instance.Options });

            var streamTask = call.RequestStream.WriteAsync(requestStream);
            yield return new WaitUntil(() => streamTask.IsCompleted);
            call.RequestStream.CompleteAsync();
            yield return new WaitUntil(() => call.ResponseAsync.IsCompleted);
            if (!streamTask.IsFaulted && !streamTask.IsCanceled)
            {
                callback(call.GetStatus(), call.ResponseAsync.Result);
            }
            else
            {
                callback(call.GetStatus(), new R());
            }

            call.Dispose();
        }

        public virtual void Innit()
        {

        }
    }
}

public delegate void DelegateCallBack<T>(Status error, T response) where T : IMessage;

public delegate void DelegateCallBackList<T>(Status error, List<T> response) where T : IMessage;
